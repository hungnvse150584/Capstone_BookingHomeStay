using AutoMapper;
using Azure;
using BusinessObject.Model;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Booking;
using Service.RequestAndResponse.Request.BookingDetail;
using Service.RequestAndResponse.Request.BookingServices;
using Service.RequestAndResponse.Request.District;
using Service.RequestAndResponse.Response.Bookings;
using Service.RequestAndResponse.Response.Districts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class BookingService : IBookingService
    {
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly IHomeStayTypeRepository _homeStayTypeRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IBookingServiceRepository _bookingServiceRepository;


        public BookingService(IMapper mapper, IBookingRepository bookingRepository, 
                              IHomeStayTypeRepository homeStayTypeRepository,
                              IServiceRepository serviceRepository,
                              IBookingServiceRepository bookingServiceRepository)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _homeStayTypeRepository = homeStayTypeRepository;
            _serviceRepository = serviceRepository;
            _bookingServiceRepository = bookingServiceRepository;
        }

        public async Task<BaseResponse<IEnumerable<GetAllBookings>>> GetAllBooking(string? search, DateTime? date = null, BookingStatus? status = null)
        {
            IEnumerable<Booking> booking = await _bookingRepository.GetAllBookingAsync(search, date, status);
            if (booking == null || !booking.Any())
            {
                return new BaseResponse<IEnumerable<GetAllBookings>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var bookings = _mapper.Map<IEnumerable<GetAllBookings>>(booking);
            if (bookings == null || !bookings.Any())
            {
                return new BaseResponse<IEnumerable<GetAllBookings>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllBookings>>("Get all bookings as base success",
                StatusCodeEnum.OK_200, bookings);
        }

        public async Task<BaseResponse<Booking>> CreateBooking(CreateBookingRequest createBookingRequest, PaymentMethod paymentMethod)
        {
            var UnpaidBooking = await _bookingRepository.GetBookingStatusByAccountId(createBookingRequest.AccountID);
            if (UnpaidBooking != null)
            {
                return new BaseResponse<Booking>("There still have a booking need to pay, complete before start a new Booking",
                    StatusCodeEnum.Conflict_409, UnpaidBooking);
            }

            Booking booking = new Booking
            {
                BookingDate = DateTime.Now,
                numberOfAdults = createBookingRequest.numberOfAdults,
                numberOfChildren = createBookingRequest.numberOfChildren,
                Status = BookingStatus.ToPay,
                PaymentMethod = paymentMethod == PaymentMethod.Cod ? PaymentMethod.Cod : PaymentMethod.PayOS,
                AccountID = createBookingRequest.AccountID,
                BookingDetails = new List<BookingDetail>()
            };

            foreach (var bookingDetail in createBookingRequest.BookingDetails)
            {
                var homeStayType = await _homeStayTypeRepository.GetHomeStayTypesByIdAsync(bookingDetail.homeStayTypeID);
                if (homeStayType == null)
                {
                    return new BaseResponse<Booking>("Cannot find your Type, please try again!",
                        StatusCodeEnum.Conflict_409, null);
                }
                if (bookingDetail.CheckOutDate.Date <= bookingDetail.CheckInDate.Date)
                {
                    return new BaseResponse<Booking>("Check-out date must be after check-in date.",
                        StatusCodeEnum.Conflict_409, null);
                }
                var dateLiving = bookingDetail.CheckOutDate - bookingDetail.CheckInDate;
                int numberOfDays = dateLiving.Days;
                if (numberOfDays <= 0)
                {
                    return new BaseResponse<Booking>("Invalid booking dates. Check-out date must be after check-in date.",
                        StatusCodeEnum.Conflict_409, null);
                }

                var bookingDetails = new BookingDetail
                {
                    CheckInDate = bookingDetail.CheckInDate,
                    CheckOutDate = bookingDetail.CheckOutDate,
                    Quantity = bookingDetail.Quantity,
                    HomeStayTypesID = homeStayType.HomeStayTypesID,
                    rentPrice = homeStayType.RentPrice,
                    TotalAmount = homeStayType.RentPrice * numberOfDays * bookingDetail.Quantity
                };
                booking.BookingDetails.Add(bookingDetails);
            }

            booking.Total = booking.BookingDetails.Sum(detail => detail.TotalAmount);


            //For Add BookingServices, it can be null if user don't want to add Services
            if (createBookingRequest.BookingOfServices != null && createBookingRequest.BookingOfServices.BookingServicesDetails?.Any() == true)
            {
                var bookingServices = new BookingServices
                {
                    BookingServicesDate = DateTime.Now,
                    Status = BookingServicesStatus.ToPay,
                    AccountID = createBookingRequest.AccountID,
                    BookingServicesDetails = new List<BookingServicesDetail>()
                };
                foreach (var serviceDetailRequest in createBookingRequest.BookingOfServices.BookingServicesDetails)
                {
                    var service = await _serviceRepository.GetByIdAsync(serviceDetailRequest.ServicesID);
                    if (service == null)
                    {
                        return new BaseResponse<Booking>("Service not found, please check the service ID.",
                            StatusCodeEnum.NotFound_404, null);
                    }

                    var bookingServiceDetail = new BookingServicesDetail
                    {
                        Quantity = serviceDetailRequest.Quantity,
                        unitPrice = service.servicesPrice,
                        TotalAmount = serviceDetailRequest.Quantity * service.servicesPrice,
                        ServicesID = serviceDetailRequest.ServicesID
                    };
                    bookingServices.BookingServicesDetails.Add(bookingServiceDetail); 
                }
                bookingServices.Total = bookingServices.BookingServicesDetails.Sum(detail => detail.TotalAmount);

                if (bookingServices.BookingServicesDetails.Any())
                {
                    booking.BookingServices ??= new List<BookingServices>(); 
                    booking.BookingServices.Add(bookingServices); 
                }
            }

            await _bookingRepository.AddBookingAsync(booking);
            return new BaseResponse<Booking>("Create Booking Successfully!!!", StatusCodeEnum.Created_201, booking);

        }

        public async Task<BaseResponse<Booking>> ChangeBookingStatus(int bookingId, int? bookingServiceID, BookingStatus status, BookingServicesStatus servicesStatus)
        {
            var bookingExist = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (bookingExist == null)
            {
                return new BaseResponse<Booking>("Cannot find your Booking!",
                         StatusCodeEnum.NotFound_404, null);
            }
            else
            {
                if (bookingServiceID != null || bookingServiceID > 0)
                {
                    var bookingServiceExist = await _bookingServiceRepository.FindBookingServicesByIdAsync(bookingServiceID);
                    if (bookingServiceExist == null)
                    {
                        return new BaseResponse<Booking>("Cannot find your BookingServices!",
                                 StatusCodeEnum.NotFound_404, null);
                    }

                    var bookingService = await _bookingServiceRepository.ChangeBookingServicesStatus(bookingServiceExist.BookingServicesID, servicesStatus);
                }
                var booking = await _bookingRepository.ChangeBookingStatus(bookingExist.BookingID, status);

                return new BaseResponse<Booking>("Change status ok", StatusCodeEnum.OK_200, booking);
            }
        }

        public async Task<BaseResponse<UpdateBookingRequest>> UpdateBooking(int bookingID, UpdateBookingRequest request)
        {
            var existingBooking = await _bookingRepository.GetBookingByIdAsync(bookingID);
            if (existingBooking == null)
            {
                return new BaseResponse<UpdateBookingRequest>("Cannot find your Booking!",
                         StatusCodeEnum.NotFound_404, null);
            }

            bool isPaid = !string.IsNullOrEmpty(existingBooking.transactionID);
            bool isCompleted = existingBooking.Status == BookingStatus.Completed;
            bool isCancelled = existingBooking.Status == BookingStatus.Cancelled;

            if (isCompleted)
            {
                return new BaseResponse<UpdateBookingRequest>("This booking is already completed and cannot be modified.",
                           StatusCodeEnum.NotFound_404, null);
            }

            if (isCancelled)
            {
                return new BaseResponse<UpdateBookingRequest>("This booking is already cancelled and cannot be modified.",
                           StatusCodeEnum.NotFound_404, null);
            }
            if (isPaid)
            {
                foreach (var updatedBookingDetails in request.BookingDetails)
                {
                    var existingDetail = existingBooking.BookingDetails
                    .FirstOrDefault(d => d.HomeStayTypesID == updatedBookingDetails.homeStayTypeID);

                    if (existingDetail != null)
                    {   //Tính số đêm thay đồi ngày
                        int newNumberOfNights = (updatedBookingDetails.CheckOutDate - updatedBookingDetails.CheckInDate).Days;

                        // Tính số đêm cũ (trước khi update)
                        int oldNumberOfNights = (existingDetail.CheckOutDate - existingDetail.CheckInDate).Days;

                        // Kiểm tra nếu số đêm bị thay đổi
                        if (newNumberOfNights != oldNumberOfNights)
                        {
                            return new BaseResponse<UpdateBookingRequest>("You should change check-in/check-out dates as it affects the paid amount.",
                                StatusCodeEnum.BadRequest_400, null);
                        }
                        existingDetail.CheckInDate = updatedBookingDetails.CheckInDate;
                        existingDetail.CheckOutDate = updatedBookingDetails.CheckOutDate;
                    }
                }
            }
            else
            {
                var existingDetails = existingBooking.BookingDetails.ToList();
                foreach (var updatedBookingDetails in request.BookingDetails)
                {
                    var homeStayType = await _homeStayTypeRepository.GetHomeStayTypesByIdAsync(updatedBookingDetails.homeStayTypeID);
                    if (homeStayType == null)
                    {
                        return new BaseResponse<UpdateBookingRequest>("Cannot Find Your Type",
                                StatusCodeEnum.BadRequest_400, null);
                    }
                    int numberOfDays = (updatedBookingDetails.CheckOutDate - updatedBookingDetails.CheckInDate).Days;
                    if (updatedBookingDetails.BookingDetailID.HasValue)
                    {
                        // 🔹 CẬP NHẬT: Tìm BookingDetail theo ID
                        var existingDetail = existingDetails
                            .FirstOrDefault(d => d.BookingDetailID == updatedBookingDetails.BookingDetailID.Value);

                        if (existingDetail != null)
                        {
                            existingDetail.HomeStayTypesID = updatedBookingDetails.homeStayTypeID;
                            existingDetail.CheckInDate = updatedBookingDetails.CheckInDate;
                            existingDetail.CheckOutDate = updatedBookingDetails.CheckOutDate;
                            existingDetail.Quantity = updatedBookingDetails.Quantity;
                            existingDetail.rentPrice = homeStayType.RentPrice;
                            existingDetail.TotalAmount = homeStayType.RentPrice * numberOfDays * updatedBookingDetails.Quantity;
                        }
                    }

                    else
                    {
                        // 🔹 THÊM MỚI: Nếu không có BookingDetailID, nghĩa là thêm mới
                        existingBooking.BookingDetails.Add(new BookingDetail
                        {
                            HomeStayTypesID = updatedBookingDetails.homeStayTypeID,
                            CheckInDate = updatedBookingDetails.CheckInDate,
                            CheckOutDate = updatedBookingDetails.CheckOutDate,
                            Quantity = updatedBookingDetails.Quantity,
                            rentPrice = homeStayType.RentPrice,
                            TotalAmount = homeStayType.RentPrice * numberOfDays * updatedBookingDetails.Quantity
                        });
                    }
                }
                existingBooking.Total = existingBooking.BookingDetails.Sum(detail => detail.TotalAmount);
            }
            existingBooking.numberOfAdults = request.numberOfAdults;
            existingBooking.numberOfChildren = request.numberOfChildren;
            await _bookingRepository.UpdateBookingAsync(existingBooking);
            return new BaseResponse<UpdateBookingRequest>("Booking updated successfully!", StatusCodeEnum.OK_200, request);
        }

        public async Task<BaseResponse<BookingServices>> CreateServiceBooking(CreateBookingServices bookingServiceRequest, PaymentServicesMethod paymentServicesMethod)
        {
            var bookingExist = await _bookingRepository.GetBookingByIdAsync(bookingServiceRequest.BookingID);
            if (bookingExist == null)
            {
                /*if (bookingExist.Status == BookingStatus.Cancelled)
                {
                    return new BaseResponse<BookingServices>("This Booking was canceled, cannot add more services!",
                    StatusCodeEnum.Conflict_409, null);
                }
                if (bookingExist.Status == BookingStatus.Completed)
                {
                    return new BaseResponse<BookingServices>("This Booking was completed, cannot add more services!",
                    StatusCodeEnum.Conflict_409, null);
                }
                if (bookingExist.Status == BookingStatus.ReturnRefund)
                {
                    return new BaseResponse<BookingServices>("This Booking was refunded, cannot add more services!",
                    StatusCodeEnum.Conflict_409, null);
                }
                if (bookingExist.Status == BookingStatus.RequestReturn)
                {
                    return new BaseResponse<BookingServices>("You are trying to take the refund of this booking, cannot add more services!",
                    StatusCodeEnum.Conflict_409, null);
                }*/
                

                return new BaseResponse<BookingServices>("Cannot Find any Booking!",
                    StatusCodeEnum.NotFound_404, null);
            }
            
                switch (bookingExist.Status)
                {
                    case BookingStatus.Cancelled:
                        return new BaseResponse<BookingServices>("This booking was canceled, cannot add more services!", StatusCodeEnum.Conflict_409, null);
                    case BookingStatus.Completed:
                        return new BaseResponse<BookingServices>("This booking was completed, cannot add more services!", StatusCodeEnum.Conflict_409, null);
                    case BookingStatus.ReturnRefund:
                        return new BaseResponse<BookingServices>("This booking was refunded, cannot add more services!", StatusCodeEnum.Conflict_409, null);
                    case BookingStatus.RequestReturn:
                        return new BaseResponse<BookingServices>("You are trying to take a refund for this booking, cannot add more services!", StatusCodeEnum.Conflict_409, null);
                    default:
                        break;
                }
            

            var UnpaidServices = await _bookingServiceRepository.GetUnpaidServicesByAccountId(bookingServiceRequest.AccountID);
            if (UnpaidServices != null)
            {
                return new BaseResponse<BookingServices>("There still have a booking services need to pay, complete before start a new BookingServices",
                    StatusCodeEnum.Conflict_409, UnpaidServices);
            }

            var bookingServices = new BookingServices
            {
                BookingServicesDate = DateTime.Now,
                AccountID = bookingServiceRequest.AccountID,
                BookingID = bookingServiceRequest.BookingID,
                Status = BookingServicesStatus.ToPay,
                BookingServicesDetails = new List<BookingServicesDetail>()
            };
            foreach (var serviceDetailRequest in bookingServiceRequest.BookingServicesDetails)
            {
                var service = await _serviceRepository.GetByIdAsync(serviceDetailRequest.ServicesID);
                if (service == null)
                {
                    return new BaseResponse<BookingServices>("Service not found, please check the service ID.",
                        StatusCodeEnum.NotFound_404, null);
                }

                var bookingServiceDetail = new BookingServicesDetail
                {
                    Quantity = serviceDetailRequest.Quantity,
                    unitPrice = service.servicesPrice,
                    TotalAmount = serviceDetailRequest.Quantity * service.servicesPrice,
                    ServicesID = serviceDetailRequest.ServicesID
                };
                bookingServices.BookingServicesDetails.Add(bookingServiceDetail);
            }
            bookingServices.Total = bookingServices.BookingServicesDetails.Sum(detail => detail.TotalAmount);
            await _bookingServiceRepository.AddBookingServicesAsync(bookingServices);
            return new BaseResponse<BookingServices>("Booking Services Successfully!!!", StatusCodeEnum.Created_201, bookingServices);
        }


        // For Admin and Owner DashBoard
        public async Task<BaseResponse<GetStaticBookings>> GetStaticBookings()
        {
            var booking = await _bookingRepository.GetStaticBookings();
            var response = new GetStaticBookings
            {
                bookings = booking.bookings,
                bookingsReturnOrCancell = booking.bookingsReturnOrCancell,
                bookingsCancell = booking.bookingsCancell,
                bookingsComplete = booking.bookingsComplete,
                bookingsReport = booking.bookingsReport,
                bookingsReturnRefund = booking.bookingsReturnRefund
            };
            if (response == null)
            {
                return new BaseResponse<GetStaticBookings>("Get All Fail", StatusCodeEnum.BadGateway_502, response);
            }
            return new BaseResponse<GetStaticBookings>("Get All Success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<GetTopHomeStayBookingInMonth>> GetTopHomeStayBookingInMonth()
        {
            var bookings = await _bookingRepository.GetTopHomeStayBookingInMonthAsync();
            var response = new GetTopHomeStayBookingInMonth
            {
                topHomeStays = bookings.Select(o => (o.homeStayName, o.QuantityOfBooking)).ToList()
            };
            if (response == null)
            {
                return new BaseResponse<GetTopHomeStayBookingInMonth>("Get All Fail", StatusCodeEnum.BadGateway_502, response);
            }
            return new BaseResponse<GetTopHomeStayBookingInMonth>("Get All Success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<List<GetTotalBookingsTotalBookingsAmount>>> GetTotalBookingsTotalBookingsAmount(DateTime startDate, DateTime endDate, string? timeSpanType)
        {
            if (startDate == default(DateTime).Date || endDate == default(DateTime).Date)
            {
                return new BaseResponse<List<GetTotalBookingsTotalBookingsAmount>>("Please input time", StatusCodeEnum.NotImplemented_501, null);
            }

            if (startDate >= endDate)
            {
                return new BaseResponse<List<GetTotalBookingsTotalBookingsAmount>>("Please input endDate > startDate", StatusCodeEnum.NotAcceptable_406, null);
            }

            var total = await _bookingRepository.GetTotalBookingsTotalBookingsAmount(startDate, endDate, timeSpanType);
            var response = total.Select(p => new GetTotalBookingsTotalBookingsAmount
            {
                span = p.span,
                totalBookings = p.totalBookings,
                totalBookingsAmount = p.totalBookingsAmount
            }).ToList();
            if (response == null || !response.Any())
            {
                return new BaseResponse<List<GetTotalBookingsTotalBookingsAmount>>("Get Total Fail", StatusCodeEnum.BadRequest_400, null);
            }
            return new BaseResponse<List<GetTotalBookingsTotalBookingsAmount>>("Get All Success", StatusCodeEnum.OK_200, response);
        }



        /*public async Task<BaseResponse<Booking>> CanncelledBooking(int bookingID, int bookingServiceID)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingID);
            if (booking == null)
            {
                return new BaseResponse<Booking>("Cannot find your Booking!",
                        StatusCodeEnum.NotFound_404, null);
            }




        }*/
    }
}
