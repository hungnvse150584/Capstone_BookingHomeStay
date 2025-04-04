﻿using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class CheckOutService :ICheckOutService
    {
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly IHomeStayRentalRepository _homeStayTypeRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IBookingServiceRepository _bookingServiceRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IPricingRepository _pricingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ICommissionRateRepository _commissionRateRepository;
        private readonly IBookingDetailRepository _bookingDetailRepository;
        private readonly IBookingServiceDetailRepository _bookingServiceDetailRepository;
        private readonly ICancellationPolicyRepository _cancelltaionRepository;

        public CheckOutService(IMapper mapper, IBookingRepository bookingRepository,
                              IHomeStayRentalRepository homeStayTypeRepository,
                              IServiceRepository serviceRepository,
                              IBookingServiceRepository bookingServiceRepository,
                              IRoomTypeRepository roomTypeRepository,
                              IRoomRepository roomRepository,
                              IPricingRepository pricingRepository,
                              ICommissionRateRepository commissionRateRepository,
                              IBookingDetailRepository bookingDetailRepository,
                              IBookingServiceDetailRepository bookingServiceDetailRepository,
                              ICancellationPolicyRepository cancelltaionRepository)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _homeStayTypeRepository = homeStayTypeRepository;
            _serviceRepository = serviceRepository;
            _bookingServiceRepository = bookingServiceRepository;
            _roomTypeRepository = roomTypeRepository;
            _roomRepository = roomRepository;
            _pricingRepository = pricingRepository;
            _commissionRateRepository = commissionRateRepository;
            _bookingDetailRepository = bookingDetailRepository;
            _bookingServiceDetailRepository = bookingServiceDetailRepository;
            _cancelltaionRepository = cancelltaionRepository;
        }

        public async Task<BaseResponse<string>> CreateBooking(CreateBookingRequest createBookingRequest, PaymentMethod paymentMethod)
        {
            var UnpaidBooking = await _bookingRepository.GetBookingStatusByAccountId(createBookingRequest.AccountID);
            if (UnpaidBooking != null)
            {
                return new BaseResponse<string>("There still have a booking need to pay, complete before start a new Booking",
                    StatusCodeEnum.Conflict_409, null);
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
                HomeStayID = createBookingRequest.HomeStayID,
                BookingDetails = new List<BookingDetail>()
            };

            foreach (var bookingDetail in createBookingRequest.BookingDetails)
            {
                var homeStayType = await _homeStayTypeRepository.GetHomeStayTypesByIdAsync(bookingDetail.homeStayTypeID);
                if (homeStayType == null)
                {
                    return new BaseResponse<string>("Cannot find your HomeStayRental, please try again!",
                        StatusCodeEnum.Conflict_409, null);
                }
                if (bookingDetail.CheckOutDate.Date <= bookingDetail.CheckInDate.Date)
                {
                    return new BaseResponse<string>("Check-out date must be after check-in date.",
                        StatusCodeEnum.Conflict_409, null);
                }

                if (homeStayType.RentWhole == true)
                {
                    if (bookingDetail.roomTypeID > 0 || bookingDetail.roomID > 0)
                    {
                        return new BaseResponse<string>("You cannot select RoomTypeID or RoomID when renting the whole homestay.",
                            StatusCodeEnum.Conflict_409, null);
                    }

                    var total = await _pricingRepository.GetTotalPrice
                        (bookingDetail.CheckInDate, bookingDetail.CheckOutDate,
                         homeStayType.HomeStayRentalID);

                    var bookingRentWholeDetail = new BookingDetail
                    {
                        CheckInDate = bookingDetail.CheckInDate,
                        CheckOutDate = bookingDetail.CheckOutDate,
                        HomeStayRentalID = homeStayType.HomeStayRentalID,
                        UnitPrice = total.totalUnitPrice,
                        rentPrice = total.totalRentPrice,
                        TotalAmount = total.totalRentPrice
                    };
                    booking.BookingDetails.Add(bookingRentWholeDetail);
                }
                else
                {
                    var roomType = await _roomTypeRepository.GetRoomTypesByIdAsync(bookingDetail.roomTypeID);
                    if (roomType == null)
                    {
                        return new BaseResponse<string>("Cannot find your Type, please try again!",
                            StatusCodeEnum.Conflict_409, null);
                    }
                    if (homeStayType.HomeStayRentalID != roomType.HomeStayRentalID)
                    {
                        return new BaseResponse<string>("This roomType is not belong to this HomeStayRental, please try again!",
                            StatusCodeEnum.Conflict_409, null);
                    }

                    // ✅ Lấy danh sách phòng trống thuộc RoomType đó
                    var availableRooms = await _roomRepository.GetAvailableRoomFilter(
                        bookingDetail.CheckInDate, bookingDetail.CheckOutDate);

                    // ✅ Lọc danh sách chỉ lấy phòng thuộc RoomType
                    // Kiểm tra phòng khách đã chọn có trống không
                    var selectedRoom = availableRooms.FirstOrDefault(r => r.RoomID == bookingDetail.roomID);



                    var total = await _pricingRepository.GetTotalPrice
                        (bookingDetail.CheckInDate, bookingDetail.CheckOutDate,
                        homeStayType.HomeStayRentalID, roomType.RoomTypesID);

                    var bookingDetails = new BookingDetail
                    {
                        CheckInDate = bookingDetail.CheckInDate,
                        CheckOutDate = bookingDetail.CheckOutDate,
                        HomeStayRentalID = homeStayType.HomeStayRentalID,
                        RoomID = bookingDetail.roomID,
                        UnitPrice = total.totalUnitPrice,
                        rentPrice = total.totalRentPrice,
                        TotalAmount = total.totalRentPrice
                    };
                    booking.BookingDetails.Add(bookingDetails);
                }
            }

            var totalPriceBooking = booking.BookingDetails.Sum(detail => detail.TotalAmount);

            if (createBookingRequest.HomeStayID == null || createBookingRequest.HomeStayID <= 0)
            {
                return new BaseResponse<string>("Invalid HomeStayID, please try again!",
                            StatusCodeEnum.Conflict_409, null);
            }

            var commissionrate = await _commissionRateRepository.GetCommissionRateByHomeStay(createBookingRequest.HomeStayID);
            if (commissionrate == null)
            {
                return new BaseResponse<string>("Cannot find the HomeStay Commission, please try again!",
                            StatusCodeEnum.Conflict_409, null);
            }
            if (commissionrate.PlatformShare <= 0 || commissionrate.PlatformShare > 1)
            {
                return new BaseResponse<string>("Invalid PlatformShare value, please check commission settings!",
                            StatusCodeEnum.Conflict_409, null);
            }

            //For Add BookingServices, it can be null if user don't want to add Services
            if (createBookingRequest.BookingOfServices != null && createBookingRequest.BookingOfServices.BookingServicesDetails?.Any() == true)
            {
                var bookingServices = new BookingServices
                {
                    BookingServicesDate = DateTime.Now,
                    Status = BookingServicesStatus.Pending,
                    PaymentServicesMethod = PaymentServicesMethod.VnPay,
                    PaymentServiceStatus = PaymentServicesStatus.Pending,
                    AccountID = createBookingRequest.AccountID,
                    BookingServicesDetails = new List<BookingServicesDetail>()
                };

                var duplicateService = createBookingRequest.BookingOfServices.BookingServicesDetails
                                        .GroupBy(d => d.ServicesID)
                                        .Where(g => g.Count() > 1)
                                        .Select(g => g.Key)
                                        .FirstOrDefault();

                if (duplicateService != 0) // Nếu có service trùng nhau
                {
                    return new BaseResponse<string>(
                        $"Service with ID {duplicateService} is duplicated. Please choose different services or adjust the quantity.",
                        StatusCodeEnum.Conflict_409, null);
                }

                foreach (var serviceDetailRequest in createBookingRequest.BookingOfServices.BookingServicesDetails)
                {
                    var service = await _serviceRepository.GetByIdAsync(serviceDetailRequest.ServicesID);
                    if (service == null)
                    {
                        return new BaseResponse<string>("Service not found, please check the service ID.",
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
            double totalPriceServices = (booking.BookingServices?.Sum(bs => bs.Total) ?? 0);
            var totalAmount = totalPriceBooking + totalPriceServices;
            var deposit = commissionrate.PlatformShare * totalAmount;
            var remaining = totalAmount - deposit;
            booking.TotalRentPrice = totalPriceBooking;
            booking.Total = totalAmount;
            booking.bookingDeposit = deposit;
            booking.remainingBalance = remaining;

            await _bookingRepository.AddBookingAsync(booking);
            return new BaseResponse<string>("Create Booking Successfully!!!", StatusCodeEnum.Created_201, null);
        }

        public async Task<BaseResponse<Booking>> ChangeBookingStatus(int bookingId, int? bookingServiceID, BookingStatus status, PaymentStatus paymentStatus, BookingServicesStatus servicesStatus, PaymentServicesStatus statusPayment)
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

                    var bookingService = await _bookingServiceRepository.ChangeBookingServicesStatus(bookingServiceExist.BookingServicesID, servicesStatus, statusPayment);
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

            bool isPaid = existingBooking.paymentStatus == PaymentStatus.Deposited;
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
                return new BaseResponse<UpdateBookingRequest>("This booking has already been deposite and cannot be modified.",
             StatusCodeEnum.BadRequest_400, null);
            }
            else
            {
                var existingDetails = existingBooking.BookingDetails.ToList();

                var updatedDetailIds = request.BookingDetails
                                        .Select(d => d.BookingDetailID)
                                        .Where(id => id.HasValue)
                                        .Select(id => id.Value)
                                        .ToList();

                var detailsToRemove = await _bookingDetailRepository.GetBookingDetailsToRemoveAsync(bookingID, updatedDetailIds);

                if (detailsToRemove.Any())
                {
                    await _bookingDetailRepository.DeleteBookingDetailAsync(detailsToRemove);
                }

                foreach (var updatedBookingDetails in request.BookingDetails)
                {
                    /*var existingBookingDetail = existingBooking.BookingDetails
                        .FirstOrDefault(d => d.BookingDetailID == updatedBookingDetails.BookingDetailID.Value);
                    if (existingBookingDetail == null)
                    {
                        return new BaseResponse<UpdateBookingRequest>($"Cannot find existing booking detail with ID {updatedBookingDetails.BookingDetailID}", StatusCodeEnum.BadRequest_400, null);
                    }*/
                    var homeStayType = await _homeStayTypeRepository.GetHomeStayTypesByIdAsync(updatedBookingDetails.homeStayTypeID);
                    if (homeStayType == null)
                    {
                        return new BaseResponse<UpdateBookingRequest>("Cannot Find Your HomeStayRental!",
                                StatusCodeEnum.BadRequest_400, null);
                    }

                    if (updatedBookingDetails.BookingDetailID.HasValue)
                    {
                        var existingBookingDetail = existingBooking.BookingDetails
                        .FirstOrDefault(d => d.BookingDetailID == updatedBookingDetails.BookingDetailID.Value);
                        if (existingBookingDetail == null)
                        {
                            return new BaseResponse<UpdateBookingRequest>($"Cannot find existing booking detail with ID {updatedBookingDetails.BookingDetailID}", StatusCodeEnum.BadRequest_400, null);
                        }

                        // 🔹 CẬP NHẬT: Tìm BookingDetail theo ID
                        var existingDetail = existingDetails
                            .FirstOrDefault(d => d.BookingDetailID == updatedBookingDetails.BookingDetailID.Value);

                        if (existingDetail != null)
                        {
                            if (homeStayType.RentWhole == true)
                            {
                                if (updatedBookingDetails.roomTypeID > 0 || updatedBookingDetails.roomID > 0)
                                {
                                    return new BaseResponse<UpdateBookingRequest>("You cannot select RoomTypeID or RoomID when renting the whole homestay.",
                                        StatusCodeEnum.Conflict_409, null);
                                }

                                var total = await _pricingRepository.GetTotalPrice
                                            (updatedBookingDetails.CheckInDate, updatedBookingDetails.CheckOutDate,
                                             homeStayType.HomeStayRentalID);

                                existingDetail.HomeStayRentalID = updatedBookingDetails.homeStayTypeID;
                                existingDetail.RoomID = null;
                                existingDetail.CheckInDate = updatedBookingDetails.CheckInDate;
                                existingDetail.CheckOutDate = updatedBookingDetails.CheckOutDate;
                                existingDetail.UnitPrice = total.totalUnitPrice;
                                existingDetail.rentPrice = total.totalRentPrice;
                                existingDetail.TotalAmount = total.totalRentPrice;

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

                                // ✅ Lấy danh sách phòng trống thuộc RoomType đó
                                var availableRooms = await _roomRepository.GetAvailableRoomFilter(
                                    updatedBookingDetails.CheckInDate, updatedBookingDetails.CheckOutDate);

                                var room = await _roomRepository.GetRoomByIdAsync(updatedBookingDetails.roomID);
                                if (room == null || room.RoomTypesID != roomType.RoomTypesID)
                                {
                                    return new BaseResponse<UpdateBookingRequest>("The selected room does not belong to the specified RoomType!",
                                            StatusCodeEnum.BadRequest_400, null);
                                }
                                // ✅ Lọc danh sách chỉ lấy phòng thuộc RoomType
                                // Kiểm tra phòng khách đã chọn có trống không
                                var selectedRoom = availableRooms.FirstOrDefault(r => r.RoomID == updatedBookingDetails.roomID);

                                var total = await _pricingRepository.GetTotalPrice
                                           (updatedBookingDetails.CheckInDate, updatedBookingDetails.CheckOutDate,
                                           homeStayType.HomeStayRentalID, roomType.RoomTypesID);

                                existingDetail.HomeStayRentalID = updatedBookingDetails.homeStayTypeID;
                                existingDetail.RoomID = updatedBookingDetails.roomID;
                                existingDetail.CheckInDate = updatedBookingDetails.CheckInDate;
                                existingDetail.CheckOutDate = updatedBookingDetails.CheckOutDate;
                                existingDetail.UnitPrice = total.totalUnitPrice;
                                existingDetail.rentPrice = total.totalRentPrice;
                                existingDetail.TotalAmount = total.totalRentPrice;
                            }
                        }
                    }
                    else
                    {
                        bool isHomeStayRentalExists = existingBooking.BookingDetails
                        .Any(d => d.HomeStayRentalID == updatedBookingDetails.homeStayTypeID
                        && d.HomeStayRentals.RentWhole == true);

                        bool isRoomExists = existingBooking.BookingDetails
                        .Any(d => d.RoomID == updatedBookingDetails.roomID);

                        if (isHomeStayRentalExists)
                        {
                            return new BaseResponse<UpdateBookingRequest>("This HomeStayRental is already choosen, Please chooose another HomeStayRental!",
                                            StatusCodeEnum.BadRequest_400, null);
                        }

                        if (isRoomExists)
                        {
                            return new BaseResponse<UpdateBookingRequest>("This Room is already choosen, Please chooose another RoomType or change the quantity of that RoomType!",
                                            StatusCodeEnum.BadRequest_400, null);
                        }

                        // 🔹 THÊM MỚI: Nếu không có BookingDetailID, nghĩa là thêm mới
                        if (homeStayType.RentWhole == true)
                        {
                            if (updatedBookingDetails.roomTypeID > 0 || updatedBookingDetails.roomID > 0)
                            {
                                return new BaseResponse<UpdateBookingRequest>("You cannot select RoomTypeID or RoomID when renting the whole homestay.",
                                    StatusCodeEnum.Conflict_409, null);
                            }

                            var total = await _pricingRepository.GetTotalPrice
                                            (updatedBookingDetails.CheckInDate, updatedBookingDetails.CheckOutDate,
                                             homeStayType.HomeStayRentalID);

                            existingBooking.BookingDetails.Add(new BookingDetail
                            {
                                HomeStayRentalID = updatedBookingDetails.homeStayTypeID,
                                RoomID = null,
                                CheckInDate = updatedBookingDetails.CheckInDate,
                                CheckOutDate = updatedBookingDetails.CheckOutDate,
                                UnitPrice = total.totalUnitPrice,
                                rentPrice = total.totalRentPrice,
                                TotalAmount = total.totalRentPrice
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

                            // ✅ Lấy danh sách phòng trống thuộc RoomType đó
                            var availableRooms = await _roomRepository.GetAvailableRoomFilter(
                                updatedBookingDetails.CheckInDate, updatedBookingDetails.CheckOutDate);

                            var room = await _roomRepository.GetRoomByIdAsync(updatedBookingDetails.roomID);
                            if (room == null || room.RoomTypesID != roomType.RoomTypesID)
                            {
                                return new BaseResponse<UpdateBookingRequest>("The selected room does not belong to the specified RoomType!",
                                        StatusCodeEnum.BadRequest_400, null);
                            }

                            // ✅ Lọc danh sách chỉ lấy phòng thuộc RoomType
                            // Kiểm tra phòng khách đã chọn có trống không
                            var selectedRoom = availableRooms.FirstOrDefault(r => r.RoomID == updatedBookingDetails.roomID);

                            var total = await _pricingRepository.GetTotalPrice
                                       (updatedBookingDetails.CheckInDate, updatedBookingDetails.CheckOutDate,
                                       homeStayType.HomeStayRentalID, roomType.RoomTypesID);

                            existingBooking.BookingDetails.Add(new BookingDetail
                            {
                                HomeStayRentalID = updatedBookingDetails.homeStayTypeID,
                                CheckInDate = updatedBookingDetails.CheckInDate,
                                CheckOutDate = updatedBookingDetails.CheckOutDate,
                                RoomID = updatedBookingDetails.roomID,
                                UnitPrice = total.totalUnitPrice,
                                rentPrice = total.totalRentPrice,
                                TotalAmount = total.totalRentPrice
                            });
                        }
                    }
                }
                var commissionrate = await _commissionRateRepository.GetCommissionByHomeStayAsync(existingBooking.HomeStayID);
                if (commissionrate == null)
                {
                    return new BaseResponse<UpdateBookingRequest>("Cannot find the HomeStay Commission, please try again!",
                                StatusCodeEnum.Conflict_409, null);
                }
                if (commissionrate.PlatformShare <= 0 || commissionrate.PlatformShare > 1)
                {
                    return new BaseResponse<UpdateBookingRequest>("Invalid PlatformShare value, please check commission settings!",
                                StatusCodeEnum.Conflict_409, null);
                }

                var totalPriceExistBooking = existingBooking.BookingDetails.Sum(detail => detail.TotalAmount);
                var existService = await _bookingServiceRepository.GetBookingServicesByBookingIdAsync(existingBooking.BookingID);

                var totalPriceExistService = existService != null ? existService.Total : 0;
                var totalPriceAmount = totalPriceExistBooking + totalPriceExistService;
                var deposit = commissionrate.PlatformShare * totalPriceAmount;
                var remaining = totalPriceAmount - deposit;
                existingBooking.bookingDeposit = deposit;
                existingBooking.remainingBalance = remaining;
                existingBooking.Total = totalPriceAmount;

            }
            existingBooking.numberOfAdults = request.numberOfAdults;
            existingBooking.numberOfChildren = request.numberOfChildren;

            await _bookingRepository.UpdateBookingAsync(existingBooking);
            return new BaseResponse<UpdateBookingRequest>("Booking updated successfully!", StatusCodeEnum.OK_200, request);
        }

        public async Task<Booking> CreateBookingPayment(int? bookingID, int? bookingServiceID, Transaction transaction)
        {
            var booking = await _bookingRepository.GetBookingsByIdAsync(bookingID);
            if (booking == null)
            {
                throw new Exception("Booking not found");
            }
            double totalAmount = booking.Total;  // Thay bằng cách tính tổng số tiền thanh toán của booking
            double amountPaid = booking.Transactions.Sum(t => t.Amount); // Tính tổng số tiền đã thanh toán từ tất cả các giao dịch

            // Kiểm tra trạng thái thanh toán
            if (amountPaid >= totalAmount)
            {
                booking.paymentStatus = PaymentStatus.FullyPaid; // Thanh toán đầy đủ
            }
            else if (amountPaid > 0)
            {
                booking.paymentStatus = PaymentStatus.Deposited; // Đặt cọc
            }

            if (bookingServiceID.HasValue && bookingServiceID.Value > 0)
            {
                var bookingService = await _bookingServiceRepository.GetBookingServiceByIdAsync(bookingServiceID);
                if (bookingService == null)
                {
                    throw new Exception("BookingService not found");
                }

                if (amountPaid >= totalAmount)
                {
                    bookingService.PaymentServiceStatus = PaymentServicesStatus.FullyPaid; // Thanh toán đầy đủ
                }
                else if (amountPaid > 0)
                {
                    bookingService.PaymentServiceStatus = PaymentServicesStatus.Deposited; // Đặt cọc
                }

                bookingService.Status = BookingServicesStatus.Confirmed;

                bookingService.Transactions ??= new List<Transaction>();

                bookingService.Transactions.Add(transaction);
                await _bookingServiceRepository.UpdateBookingServicesAsync(bookingService);
            }
            booking.Status = BookingStatus.Confirmed;

            booking.Transactions ??= new List<Transaction>();

            // Thêm transaction vào trong danh sách Transactions
            booking.Transactions.Add(transaction);

            // Lưu booking vào cơ sở dữ liệu nếu cần
            await _bookingRepository.UpdateBookingAsync(booking);
            return booking;
        }
    }
}
