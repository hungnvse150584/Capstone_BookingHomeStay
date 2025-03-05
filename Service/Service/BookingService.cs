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
using Service.RequestAndResponse.Response.Bookings;
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
        private readonly IHomeStayRentalRepository _homeStayTypeRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IBookingServiceRepository _bookingServiceRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;


        public BookingService(IMapper mapper, IBookingRepository bookingRepository,
                              IHomeStayRentalRepository homeStayTypeRepository,
                              IServiceRepository serviceRepository,
                              IBookingServiceRepository bookingServiceRepository,
                              IRoomTypeRepository roomTypeRepository)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _homeStayTypeRepository = homeStayTypeRepository;
            _serviceRepository = serviceRepository;
            _bookingServiceRepository = bookingServiceRepository;
            _roomTypeRepository = roomTypeRepository;
        }

        public async Task<BaseResponse<IEnumerable<GetAllBookings>>> GetAllBooking(string? search, DateTime? date = null, BookingStatus? status = null, PaymentStatus? paymentStatus = null)
        {
            IEnumerable<Booking> booking = await _bookingRepository.GetAllBookingAsync(search, date, status, paymentStatus);
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
                ExpiredTime = DateTime.Now.AddHours(1),
                numberOfAdults = createBookingRequest.numberOfAdults,
                numberOfChildren = createBookingRequest.numberOfChildren,
                Status = BookingStatus.Pending,
                paymentStatus = PaymentStatus.Pending,
                PaymentMethod = paymentMethod == PaymentMethod.Cod ? PaymentMethod.Cod : PaymentMethod.VnPay,
                AccountID = createBookingRequest.AccountID,
                BookingDetails = new List<BookingDetail>()
            };

            foreach (var bookingDetail in createBookingRequest.BookingDetails)
            {
                var homeStayType = await _homeStayTypeRepository.GetHomeStayTypesByIdAsync(bookingDetail.homeStayTypeID);
                if (homeStayType == null)
                {
                    return new BaseResponse<Booking>("Cannot find your HomeStayRental, please try again!",
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
                if(homeStayType.RentWhole == true)
                {
                    var bookingRentWholeDetail = new BookingDetail
                    {
                        CheckInDate = bookingDetail.CheckInDate,
                        CheckOutDate = bookingDetail.CheckOutDate,
                        Quantity = 1,
                        HomeStayRentalID = homeStayType.HomeStayRentalID,
                        rentPrice = homeStayType.RentPrice,
                        TotalAmount = homeStayType.RentPrice * numberOfDays
                    };
                    booking.BookingDetails.Add(bookingRentWholeDetail);
                }
                else
                {
                    var roomType = await _roomTypeRepository.GetRoomTypesByIdAsync(bookingDetail.roomTypeID);
                    if (roomType == null)
                    {
                        return new BaseResponse<Booking>("Cannot find your Type, please try again!",
                            StatusCodeEnum.Conflict_409, null);
                    }
                    if (homeStayType.HomeStayRentalID != roomType.HomeStayRentalID)
                    {
                        return new BaseResponse<Booking>("This roomType is not belong to this HomeStayRental, please try again!",
                            StatusCodeEnum.Conflict_409, null);
                    }
                    if(bookingDetail.Quantity <= 0)
                    {
                        return new BaseResponse<Booking>("Quantity must be > 0, please try again!",
                            StatusCodeEnum.Conflict_409, null);
                    }
                    var bookingDetails = new BookingDetail
                    {
                        CheckInDate = bookingDetail.CheckInDate,
                        CheckOutDate = bookingDetail.CheckOutDate,
                        Quantity = bookingDetail.Quantity,
                        HomeStayRentalID = homeStayType.HomeStayRentalID,
                        RoomTypesID = roomType.RoomTypesID,
                        rentPrice = roomType.RentPrice,
                        TotalAmount = roomType.RentPrice * numberOfDays * bookingDetail.Quantity
                    };
                    booking.BookingDetails.Add(bookingDetails);
                }
                
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

        public async Task<BaseResponse<Booking>> ChangeBookingStatus(int bookingId, int? bookingServiceID, BookingStatus status, PaymentStatus paymentStatus, BookingServicesStatus servicesStatus)
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
                var booking = await _bookingRepository.ChangeBookingStatus(bookingExist.BookingID, status, paymentStatus);

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
                    .FirstOrDefault(d => d.HomeStayRentalID == updatedBookingDetails.homeStayTypeID);

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
                    var existingBookingDetail = existingBooking.BookingDetails
                        .FirstOrDefault(d => d.BookingDetailID == updatedBookingDetails.BookingDetailID.Value);
                    if (existingBookingDetail == null)
                    {
                        return new BaseResponse<UpdateBookingRequest>($"Cannot find existing booking detail with ID {updatedBookingDetails.BookingDetailID}", StatusCodeEnum.BadRequest_400, null);
                    }
                    var homeStayType = await _homeStayTypeRepository.GetHomeStayTypesByIdAsync(updatedBookingDetails.homeStayTypeID);
                    if (homeStayType == null)
                    {
                        return new BaseResponse<UpdateBookingRequest>("Cannot Find Your HomeStayRental!",
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
                            if (homeStayType.RentWhole == true)
                            {
                                existingDetail.HomeStayRentalID = updatedBookingDetails.homeStayTypeID;
                                existingDetail.CheckInDate = updatedBookingDetails.CheckInDate;
                                existingDetail.CheckOutDate = updatedBookingDetails.CheckOutDate;
                                existingDetail.Quantity = 1;
                                existingDetail.rentPrice = homeStayType.RentPrice;
                                existingDetail.TotalAmount = homeStayType.RentPrice * numberOfDays;
                            }
                            else
                            {
                                var roomType = await _roomTypeRepository.GetRoomTypesByIdAsync(updatedBookingDetails.roomTypeID);
                                if (roomType == null)
                                {
                                    return new BaseResponse<UpdateBookingRequest>("Cannot Find Your Type!",
                                            StatusCodeEnum.BadRequest_400, null);
                                }
                                if (homeStayType.HomeStayRentalID != roomType.HomeStayRentalID)
                                {
                                    return new BaseResponse<UpdateBookingRequest>("This roomType is not belong to this HomeStayRental, please try again!",
                                            StatusCodeEnum.BadRequest_400, null);
                                }
                                if(updatedBookingDetails.Quantity <=0)
                                {
                                    return new BaseResponse<UpdateBookingRequest>("Quantity must be >0, please try again!",
                                            StatusCodeEnum.BadRequest_400, null);
                                }

                                existingDetail.HomeStayRentalID = updatedBookingDetails.homeStayTypeID;
                                existingDetail.RoomTypesID = roomType.RoomTypesID;
                                existingDetail.CheckInDate = updatedBookingDetails.CheckInDate;
                                existingDetail.CheckOutDate = updatedBookingDetails.CheckOutDate;
                                existingDetail.Quantity = updatedBookingDetails.Quantity;
                                existingDetail.rentPrice = roomType.RentPrice;
                                existingDetail.TotalAmount = roomType.RentPrice * numberOfDays * updatedBookingDetails.Quantity;
                            }
                        }
                    }

                    else
                    {
                        bool isHomeStayRentalExists = existingBooking.BookingDetails
                        .Any(d => d.HomeStayRentalID == updatedBookingDetails.homeStayTypeID);

                        bool isRoomTypeExists = existingBooking.BookingDetails
                        .Any(d => d.RoomTypesID == updatedBookingDetails.roomTypeID);

                        if (isHomeStayRentalExists)
                        {
                            return new BaseResponse<UpdateBookingRequest>("This HomeStayRental is already choosen, Please chooose another HomeStayRental!",
                                            StatusCodeEnum.BadRequest_400, null);
                        }

                        if(isRoomTypeExists)
                        {
                            return new BaseResponse<UpdateBookingRequest>("This RoomType is already choosen, Please chooose another RoomType or change the quantity of that RoomType!",
                                            StatusCodeEnum.BadRequest_400, null);
                        }

                        // 🔹 THÊM MỚI: Nếu không có BookingDetailID, nghĩa là thêm mới
                        if (homeStayType.RentWhole == true)
                        {
                            existingBooking.BookingDetails.Add(new BookingDetail
                            {
                                HomeStayRentalID = updatedBookingDetails.homeStayTypeID,
                                CheckInDate = updatedBookingDetails.CheckInDate,
                                CheckOutDate = updatedBookingDetails.CheckOutDate,
                                Quantity = 1,
                                rentPrice = homeStayType.RentPrice,
                                TotalAmount = homeStayType.RentPrice * numberOfDays
                            });
                        }
                        // 🔹 THÊM MỚI: Nếu không có BookingDetailID, nghĩa là thêm mới
                        else
                        {
                            var roomType = await _roomTypeRepository.GetRoomTypesByIdAsync(updatedBookingDetails.roomTypeID);
                            if (roomType == null)
                            {
                                return new BaseResponse<UpdateBookingRequest>("Cannot Find Your Type!",
                                        StatusCodeEnum.BadRequest_400, null);
                            }
                            if (homeStayType.HomeStayRentalID != roomType.HomeStayRentalID)
                            {
                                return new BaseResponse<UpdateBookingRequest>("This roomType is not belong to this HomeStayRental, please try again!",
                                        StatusCodeEnum.BadRequest_400, null);
                            }
                            if (updatedBookingDetails.Quantity <= 0)
                            {
                                return new BaseResponse<UpdateBookingRequest>("Quantity must be >0, please try again!",
                                        StatusCodeEnum.BadRequest_400, null);
                            }
                            existingBooking.BookingDetails.Add(new BookingDetail
                            {
                                HomeStayRentalID = updatedBookingDetails.homeStayTypeID,
                                RoomTypesID = roomType.RoomTypesID,
                                CheckInDate = updatedBookingDetails.CheckInDate,
                                CheckOutDate = updatedBookingDetails.CheckOutDate,
                                Quantity = updatedBookingDetails.Quantity,
                                rentPrice = roomType.RentPrice,
                                TotalAmount = roomType.RentPrice * numberOfDays * updatedBookingDetails.Quantity
                            });
                        }
                            
                        
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
                default:
                    break;
            }

            switch (bookingExist.paymentStatus)
            {
                case PaymentStatus.Refunded:
                    return new BaseResponse<BookingServices>("This booking was refunded, cannot add more services!", StatusCodeEnum.Conflict_409, null);
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

                if(serviceDetailRequest.Quantity <= 0)
                {
                    return new BaseResponse<BookingServices>("Quantity must be > 0, please check again.",
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

        public async Task<BaseResponse<UpdateBookingService>> UpdateBookingServices(int bookingServiceID, UpdateBookingService request)
        {
            var bookingExist = await _bookingRepository.GetBookingByIdAsync(request.BookingID);
            if (bookingExist == null)
            {
                return new BaseResponse<UpdateBookingService>("Cannot Find any Booking!", StatusCodeEnum.NotFound_404, null);
            }

            switch (bookingExist.Status)
            {
                case BookingStatus.Cancelled:
                    return new BaseResponse<UpdateBookingService>("This booking was canceled, cannot update more services!", StatusCodeEnum.Conflict_409, null);
                case BookingStatus.Completed:
                    return new BaseResponse<UpdateBookingService>("This booking was completed, cannot update more services!", StatusCodeEnum.Conflict_409, null);
            }

            switch (bookingExist.paymentStatus)
            {
                case PaymentStatus.Refunded:
                    return new BaseResponse<UpdateBookingService>("This booking was refunded, cannot add more services!", StatusCodeEnum.Conflict_409, null);
                default:
                    break;
            }

            var existingBookingService = await _bookingServiceRepository.GetBookingServicesByIdAsync(bookingServiceID);
            if (existingBookingService == null)
            {
                return new BaseResponse<UpdateBookingService>("Cannot find the specified booking service!", StatusCodeEnum.NotFound_404, null);
            }

            if (existingBookingService.BookingID != bookingExist.BookingID)
            {
                return new BaseResponse<UpdateBookingService>("The booking service does not belong to this booking!", StatusCodeEnum.Conflict_409, null);
            }

            if (existingBookingService.Status != BookingServicesStatus.ToPay)
            {
                return new BaseResponse<UpdateBookingService>("This booking service is not eligible for updates!", StatusCodeEnum.Conflict_409, null);
            }

            if (request.BookingServicesDetails == null || !request.BookingServicesDetails.Any())
            {
                return new BaseResponse<UpdateBookingService>("No service details provided for update!", StatusCodeEnum.BadRequest_400, null);
            }

            foreach (var updatedServiceDetails in request.BookingServicesDetails)
            {
                var service = await _serviceRepository.GetByIdAsync(updatedServiceDetails.ServicesID);
                if (service == null)
                {
                    return new BaseResponse<UpdateBookingService>($"Cannot find service with ID {updatedServiceDetails.ServicesID}", StatusCodeEnum.BadRequest_400, null);
                }

                if (updatedServiceDetails.Quantity <= 0)
                {
                    return new BaseResponse<UpdateBookingService>($"Quantity must be > 0, Please Check again!", StatusCodeEnum.BadRequest_400, null);
                }
                // Check if we are updating an existing service detail
                if (updatedServiceDetails.ServiceDetailID.HasValue)
                {
                    var existingDetail = existingBookingService.BookingServicesDetails
                        .FirstOrDefault(d => d.BookingServicesDetailID == updatedServiceDetails.ServiceDetailID.Value);
                    if (existingDetail == null)
                    {
                        return new BaseResponse<UpdateBookingService>($"Cannot find existing service detail with ID {updatedServiceDetails.ServiceDetailID}", StatusCodeEnum.BadRequest_400, null);
                    }

                    if (existingDetail != null)
                    {
                        existingDetail.Quantity = updatedServiceDetails.Quantity;
                        existingDetail.unitPrice = service.UnitPrice;
                        existingDetail.TotalAmount = updatedServiceDetails.Quantity * service.UnitPrice;
                        existingDetail.ServicesID = updatedServiceDetails.ServicesID;
                    }
                }
                else // Adding a new service detail
                {
                    bool isServiceExists = existingBookingService.BookingServicesDetails
                    .Any(d => d.ServicesID == updatedServiceDetails.ServicesID);

                    if (isServiceExists)
                    {
                        return new BaseResponse<UpdateBookingService>(
                            "This Service is already choosen, Please chooose another Services or change the quantity of that Service ",
                            StatusCodeEnum.Conflict_409, null);
                    }

                    existingBookingService.BookingServicesDetails.Add(new BookingServicesDetail
                    {
                        ServicesID = updatedServiceDetails.ServicesID,
                        unitPrice = service.UnitPrice,
                        Quantity = updatedServiceDetails.Quantity,
                        TotalAmount = updatedServiceDetails.Quantity * service.UnitPrice,
                    });
                }
            }

            existingBookingService.Total = existingBookingService.BookingServicesDetails.Sum(detail => detail.TotalAmount);

            await _bookingServiceRepository.UpdateBookingServicesAsync(existingBookingService);

            return new BaseResponse<UpdateBookingService>("Booking updated successfully!", StatusCodeEnum.OK_200, request);
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
