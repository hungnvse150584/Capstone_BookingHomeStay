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
    public class CheckOutService : ICheckOutService
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

        public async Task<BaseResponse<int>> CreateBooking(CreateBookingRequest createBookingRequest, PaymentMethod paymentMethod)
        {
            var unpaidBooking = await _bookingRepository.GetBookingStatusByAccountId(createBookingRequest.AccountID);
            if (unpaidBooking != null)
                return new BaseResponse<int>("There is an unpaid booking. Please complete it before creating a new one.", StatusCodeEnum.Conflict_409, 0);

            if (createBookingRequest.BookingDetails == null || !createBookingRequest.BookingDetails.Any())
                return new BaseResponse<int>("BookingDetails is required.", StatusCodeEnum.BadRequest_400, 0);

            if (createBookingRequest.HomeStayID == null || createBookingRequest.HomeStayID <= 0)
                return new BaseResponse<int>("Invalid HomeStayID, please try again!", StatusCodeEnum.Conflict_409, 0);

            var commissionRate = await _commissionRateRepository.GetCommissionRateByHomeStay(createBookingRequest.HomeStayID);
            if (commissionRate == null || commissionRate.PlatformShare <= 0 || commissionRate.PlatformShare > 1)
                return new BaseResponse<int>("Invalid commission settings, please check!", StatusCodeEnum.Conflict_409, 0);

            // Lấy danh sách ID hợp lệ
            var homeStayTypeIds = createBookingRequest.BookingDetails
                .Where(d => d.homeStayTypeID.HasValue && d.homeStayTypeID > 0)
                .Select(d => d.homeStayTypeID.Value)
                .ToList();

            var roomTypeIds = createBookingRequest.BookingDetails
                .Where(d => d.roomTypeID.HasValue && d.roomTypeID > 0)
                .Select(d => d.roomTypeID.Value)
                .ToList();

            // Chỉ gọi khi có ID hợp lệ
            var homeStayTypes = homeStayTypeIds.Any()
                ? (await _homeStayTypeRepository.GetHomeStayTypesByIdsAsync(homeStayTypeIds.Cast<int?>().ToList())).ToList()
                : new List<HomeStayRentals>();

            var roomTypes = roomTypeIds.Any()
                ? (await _roomTypeRepository.GetRoomTypesByIdsAsync(roomTypeIds.Cast<int?>().ToList())).ToList()
                : new List<RoomTypes>();

            var bookingDetails = new List<BookingDetail>();

            foreach (var detail in createBookingRequest.BookingDetails)
            {
                if (detail.CheckOutDate.Date <= detail.CheckInDate.Date)
                    return new BaseResponse<int>("Check-out date must be after check-in date.", StatusCodeEnum.Conflict_409, 0);

                HomeStayRentals? homeStayType = null;

                if (detail.homeStayTypeID.HasValue && detail.homeStayTypeID > 0)
                {
                    homeStayType = homeStayTypes.FirstOrDefault(h => h.HomeStayRentalID == detail.homeStayTypeID);
                    if (homeStayType == null)
                        return new BaseResponse<int>("Invalid HomeStayRental selection, please try again!", StatusCodeEnum.Conflict_409, 0);
                }

                var total = await _pricingRepository.GetTotalPrice(detail.CheckInDate, detail.CheckOutDate, detail.homeStayTypeID, detail.roomTypeID);

                if (homeStayType?.RentWhole == true)
                {
                    if (detail.roomTypeID > 0 || detail.roomID > 0)
                        return new BaseResponse<int>("RoomTypeID and RoomID should not be selected when renting the whole homestay.", StatusCodeEnum.Conflict_409, 0);
                }
                else
                {
                    if (detail.roomTypeID.HasValue && detail.roomTypeID > 0)
                    {
                        var roomType = roomTypes.FirstOrDefault(rt => rt.RoomTypesID == detail.roomTypeID);
                    }

                    var availableRooms = await _roomRepository.GetAvailableRoomFilter(detail.CheckInDate, detail.CheckOutDate);
                    var selectedRoom = availableRooms.FirstOrDefault(r => r.RoomID == detail.roomID);

                    if (selectedRoom == null)
                        return new BaseResponse<int>("Selected room is not available.", StatusCodeEnum.Conflict_409, 0);
                }

                bookingDetails.Add(new BookingDetail
                {
                    CheckInDate = detail.CheckInDate,
                    CheckOutDate = detail.CheckOutDate,
                    HomeStayRentalID = detail.homeStayTypeID,
                    RoomID = detail.roomID,
                    UnitPrice = total.totalUnitPrice,
                    rentPrice = total.totalRentPrice,
                    TotalAmount = total.totalRentPrice
                });
            }

            var booking = new Booking
            {
                BookingDate = DateTime.Now,
                ExpiredTime = DateTime.Now.AddHours(1),
                numberOfAdults = createBookingRequest.numberOfAdults,
                numberOfChildren = createBookingRequest.numberOfChildren,
                Status = BookingStatus.Pending,
                paymentStatus = PaymentStatus.Pending,
                PaymentMethod = paymentMethod,
                AccountID = createBookingRequest.AccountID,
                HomeStayID = createBookingRequest.HomeStayID,
                BookingDetails = bookingDetails
            };

            // Xử lý dịch vụ nếu có
            double totalPriceServices = 0;
            if (createBookingRequest.BookingOfServices?.BookingServicesDetails?.Any() == true)
            {
                var serviceIds = createBookingRequest.BookingOfServices.BookingServicesDetails.Select(s => s.ServicesID).ToList();

                if (serviceIds.Count != serviceIds.Distinct().Count())
                    return new BaseResponse<int>("Duplicate services are not allowed.", StatusCodeEnum.Conflict_409, 0);

                

                var services = await _serviceRepository.GetServicesByIdsAsync(serviceIds);
                if (services.Count() != createBookingRequest.BookingOfServices.BookingServicesDetails.Count)
                    return new BaseResponse<int>("Some selected services are invalid.", StatusCodeEnum.NotFound_404, 0);

                var bookingServiceDetails = createBookingRequest.BookingOfServices.BookingServicesDetails.Select(s =>
                {
                    
                    var service = services.FirstOrDefault(x => x.ServicesID == s.ServicesID);

                    if (service == null)
                        throw new Exception($"Service with ID {s.ServicesID} not found.");

                    if (s.Quantity <= 0)
                        throw new Exception($"Service '{service.servicesName}' must have quantity greater than 0.");

                    double unitPrice = service.UnitPrice;
                    double servicePrice = service.servicesPrice;
                    double totalAmount = 0;

                    switch (service.ServiceType)
                    {
                        case ServiceType.Quantity:
                            if (s.RentHour.HasValue || s.StartDate.HasValue || s.EndDate.HasValue)
                                throw new Exception($"Service '{service.servicesName}' is type Quantity, must not have RentHour or Date.");

                            totalAmount = servicePrice * s.Quantity;
                            break;

                        case ServiceType.Hour:
                            if (!s.RentHour.HasValue)
                                throw new Exception($"Service '{service.servicesName}' is type Hour, requires RentHour.");
                            if (s.StartDate.HasValue || s.EndDate.HasValue)
                                throw new Exception($"Service '{service.servicesName}' is type Hour, must not have StartDate or EndDate.");

                            totalAmount = servicePrice * s.Quantity * s.RentHour.Value;
                            break;

                        case ServiceType.Day:
                            if (!s.StartDate.HasValue || !s.EndDate.HasValue)
                                throw new Exception($"Service '{service.servicesName}' is type Day, requires StartDate and EndDate.");
                            if (s.StartDate.Value.Date >= s.EndDate.Value.Date)
                                throw new Exception($"Service '{service.servicesName}' has invalid date range. EndDate must be after StartDate.");
                            int rentalDays = (s.EndDate.Value.Date - s.StartDate.Value.Date).Days;
                            if (rentalDays <= 0)
                                rentalDays = 1;

                            totalAmount = servicePrice * s.Quantity * rentalDays;
                            break;
                    }

                    return new BookingServicesDetail
                    {
                        Quantity = s.Quantity,
                        unitPrice = unitPrice,
                        TotalAmount = totalAmount,
                        ServicesID = s.ServicesID,
                        RentHour = service.ServiceType == ServiceType.Hour ? s.RentHour : null,
                        StartDate = service.ServiceType == ServiceType.Day ? s.StartDate : null,
                        EndDate = service.ServiceType == ServiceType.Day ? s.EndDate : null
                    };
                }).ToList();

                totalPriceServices = bookingServiceDetails.Sum(d => d.TotalAmount);
                var bookingServices = new BookingServices
                {
                    BookingServicesDate = DateTime.Now,
                    Status = BookingServicesStatus.Pending,
                    PaymentServicesMethod = PaymentServicesMethod.VnPay,
                    PaymentServiceStatus = PaymentServicesStatus.Pending,
                    AccountID = createBookingRequest.AccountID,
                    HomeStayID = createBookingRequest.HomeStayID,
                    BookingServicesDetails = bookingServiceDetails,
                    Total = totalPriceServices,
                    isPaidWithBooking = true,
                    bookingServiceDeposit = commissionRate.PlatformShare * totalPriceServices,
                    remainingBalance = totalPriceServices - (commissionRate.PlatformShare * totalPriceServices)
                };
                booking.BookingServices = new List<BookingServices> { bookingServices };
            }

            double totalPriceBooking = bookingDetails.Sum(d => d.TotalAmount);
            double totalAmount = totalPriceBooking + totalPriceServices;

            booking.TotalRentPrice = totalPriceBooking;
            var depositBooking = commissionRate.PlatformShare * totalPriceBooking;
            booking.Total = totalAmount;
            double bookingServiceDeposit = booking.BookingServices?.FirstOrDefault()?.bookingServiceDeposit ?? 0;
            booking.bookingDeposit = depositBooking + bookingServiceDeposit;
            booking.remainingBalance = totalAmount - booking.bookingDeposit;

            await _bookingRepository.AddBookingAsync(booking);

            return new BaseResponse<int>("Booking created successfully!", StatusCodeEnum.Created_201, booking.BookingID);
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

                var duplicatedRoomIDs = request.BookingDetails
                    .Where(d => d.roomID.HasValue)
                    .GroupBy(d => d.roomID)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicatedRoomIDs.Any())
                {
                    return new BaseResponse<UpdateBookingRequest>($"RoomID(s) duplicated in request: {string.Join(", ", duplicatedRoomIDs)}", StatusCodeEnum.Conflict_409, null);
                }

                // ✅ Check duplicated HomeStayTypeID (for RentWhole)
                var duplicatedHomeStayIDs = request.BookingDetails
                    .Where(d => d.homeStayTypeID > 0)
                    .GroupBy(d => d.homeStayTypeID)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicatedHomeStayIDs.Any())
                {
                    return new BaseResponse<UpdateBookingRequest>($"HomeStayRentalID(s) duplicated in request: {string.Join(", ", duplicatedHomeStayIDs)}", StatusCodeEnum.Conflict_409, null);
                }

                foreach (var updatedBookingDetails in request.BookingDetails)
                {
                    bool isRentWhole = false;

                    if (updatedBookingDetails.homeStayTypeID > 0)
                    {
                        var homeStayType = await _homeStayTypeRepository.GetHomeStayTypesByIdAsync(updatedBookingDetails.homeStayTypeID);
                        if (homeStayType == null)
                        {
                            return new BaseResponse<UpdateBookingRequest>("Cannot Find Your HomeStayRental!",
                                    StatusCodeEnum.BadRequest_400, null);
                        }
                        isRentWhole = homeStayType.RentWhole;

                        if (isRentWhole && (updatedBookingDetails.roomTypeID > 0 || updatedBookingDetails.roomID > 0))
                        {
                            return new BaseResponse<UpdateBookingRequest>("Cannot select Room or RoomType when renting whole homestay.", StatusCodeEnum.Conflict_409, null);
                        }
                    }
                    else if (updatedBookingDetails.roomTypeID > 0)
                    {
                        var roomType = await _roomTypeRepository.GetRoomTypesByIdAsync(updatedBookingDetails.roomTypeID);
                        if (roomType == null)
                        {
                            return new BaseResponse<UpdateBookingRequest>("Invalid RoomType.", StatusCodeEnum.BadRequest_400, null);
                        }

                        var room = await _roomRepository.GetRoomByIdAsync(updatedBookingDetails.roomID.Value);
                        if (room == null || room.RoomTypesID != updatedBookingDetails.roomTypeID)
                        {
                            return new BaseResponse<UpdateBookingRequest>("Room does not belong to the specified RoomType.", StatusCodeEnum.BadRequest_400, null);
                        }

                        var availableRooms = await _roomRepository.GetAvailableRoomFilter(updatedBookingDetails.CheckInDate, updatedBookingDetails.CheckOutDate);
                        if (!availableRooms.Any(r => r.RoomID == updatedBookingDetails.roomID && r.RoomTypesID == updatedBookingDetails.roomTypeID))
                        {
                            return new BaseResponse<UpdateBookingRequest>("Room is not available.", StatusCodeEnum.Conflict_409, null);
                        }
                    }
                    else
                    {
                        return new BaseResponse<UpdateBookingRequest>("Must provide either HomeStayTypeID or RoomTypeID.", StatusCodeEnum.BadRequest_400, null);
                    }

                    var pricing = await _pricingRepository.GetTotalPrice(updatedBookingDetails.CheckInDate,
                        updatedBookingDetails.CheckOutDate, updatedBookingDetails.homeStayTypeID,
                        isRentWhole ? null : updatedBookingDetails.roomTypeID);

                    if (updatedBookingDetails.BookingDetailID.HasValue)
                    {
                        var existingDetail = existingBooking.BookingDetails
                            .FirstOrDefault(d => d.BookingDetailID == updatedBookingDetails.BookingDetailID.Value);
                        if (existingDetail == null)
                            return new BaseResponse<UpdateBookingRequest>("Booking detail not found.", StatusCodeEnum.BadRequest_400, null);

                        existingDetail.HomeStayRentalID = updatedBookingDetails.homeStayTypeID;
                        existingDetail.RoomID = isRentWhole ? null : updatedBookingDetails.roomID;
                        existingDetail.CheckInDate = updatedBookingDetails.CheckInDate;
                        existingDetail.CheckOutDate = updatedBookingDetails.CheckOutDate;
                        existingDetail.UnitPrice = pricing.totalUnitPrice;
                        existingDetail.rentPrice = pricing.totalRentPrice;
                        existingDetail.TotalAmount = pricing.totalRentPrice;
                    }
                    else
                    {
                        existingBooking.BookingDetails.Add(new BookingDetail
                        {
                            HomeStayRentalID = updatedBookingDetails.homeStayTypeID,
                            RoomID = isRentWhole ? null : updatedBookingDetails.roomID,
                            CheckInDate = updatedBookingDetails.CheckInDate,
                            CheckOutDate = updatedBookingDetails.CheckOutDate,
                            UnitPrice = pricing.totalUnitPrice,
                            rentPrice = pricing.totalRentPrice,
                            TotalAmount = pricing.totalRentPrice
                        });
                    }
                }
            }

            var commissionrate = await _commissionRateRepository.GetCommissionByHomeStayAsync(existingBooking.HomeStayID);
            if (commissionrate == null || commissionrate.PlatformShare <= 0 || commissionrate.PlatformShare > 1)
                return new BaseResponse<UpdateBookingRequest>("Invalid commission setting!", StatusCodeEnum.Conflict_409, null);

            var totalPriceExistBooking = existingBooking.BookingDetails.Sum(detail => detail.TotalAmount);
            var existService = await _bookingServiceRepository.GetBookingServicesByBookingIdAsync(existingBooking.BookingID);

            var totalPriceExistService = existService != null ? existService.Total : 0;
            var depositeExistService = existService != null ? existService.bookingServiceDeposit : 0;
            var totalPriceAmount = totalPriceExistBooking + totalPriceExistService;
            var depositBooking = commissionrate.PlatformShare * totalPriceExistBooking;
            var deposit = depositBooking + depositeExistService;
            var remaining = totalPriceAmount - deposit;
            existingBooking.bookingDeposit = deposit;
            existingBooking.remainingBalance = remaining;
            existingBooking.Total = totalPriceAmount;
            existingBooking.TotalRentPrice = totalPriceExistBooking;
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
            booking.Transactions ??= new List<Transaction>();

            transaction.HomeStay = booking.HomeStay;
            transaction.Account = booking.Account;

            // Thêm transaction vào trong danh sách Transactions
            booking.Transactions.Add(transaction);

            double totalAmount = booking.Total;  // Thay bằng cách tính tổng số tiền thanh toán của booking
            double amountPaid = booking.Transactions.Sum(t => t.Amount) / 100; // Tính tổng số tiền đã thanh toán từ tất cả các giao dịch

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

                var detail = bookingService.BookingServicesDetails.FirstOrDefault();
                if (detail != null)
                {
                    var service = detail.Services;
                    if (service != null)
                    {
                        if (service.Quantity.HasValue && detail.Quantity >= 0)
                        {
                            service.Quantity -= detail.Quantity;
                            await _serviceRepository.UpdateAsync(service);
                        }
                    }
                }

                bookingService.Status = BookingServicesStatus.Confirmed;

                transaction.HomeStay = bookingService.HomeStay;
                transaction.Account = bookingService.Account;

                bookingService.Transactions ??= new List<Transaction>();

                bookingService.Transactions.Add(transaction);
                await _bookingServiceRepository.UpdateBookingServicesAsync(bookingService);
            }
            booking.Status = BookingStatus.Confirmed;

            // Lưu booking vào cơ sở dữ liệu nếu cần
            await _bookingRepository.UpdateBookingAsync(booking);
            return booking;
        }

        public async Task<Booking> CreateBookingRefundPayment(int? bookingID, int? bookingServiceID, Transaction transaction)
        {
            var booking = await _bookingRepository.GetBookingsByIdAsync(bookingID);
            if (booking == null)
            {
                throw new Exception("Booking not found");
            }

            booking.Transactions ??= new List<Transaction>();

            transaction.HomeStay = booking.HomeStay;
            transaction.Account = booking.HomeStay.Account;

            // Thêm transaction vào trong danh sách Transactions
            booking.Transactions.Add(transaction);

            var bookingServices = await _bookingServiceRepository.GetConfirmedBookingServiceByBookingId(bookingID);
            if (bookingServices != null)
            {
                foreach (var service in bookingServices)
                {
                    service.Status = BookingServicesStatus.Cancelled;
                    service.PaymentServiceStatus = PaymentServicesStatus.Refunded;
                    service.Status = BookingServicesStatus.Cancelled;
                    service.Transactions ??= new List<Transaction>();
                    transaction.HomeStay = service.HomeStay;
                    transaction.Account = service.HomeStay.Account;
                    service.Transactions.Add(transaction);
                    await _bookingServiceRepository.UpdateBookingServicesAsync(service);
                    if (service.BookingServicesDetails != null)
                    {
                        foreach (var detail in service.BookingServicesDetails)
                        {
                            var s = detail.Services;
                            if (s != null)
                            {
                                // Kiểm tra xem dịch vụ có Quantity hợp lệ không (Có giá trị và không âm)
                                if (s.Quantity.HasValue && detail.Quantity >= 0)
                                {
                                    s.Quantity += detail.Quantity;  // Tăng số lượng dịch vụ
                                    await _serviceRepository.UpdateAsync(s);  // Cập nhật dịch vụ trong cơ sở dữ liệu
                                }
                            }
                        }
                    }
                }
            }

            booking.paymentStatus = PaymentStatus.Refunded;
            booking.Status = BookingStatus.Cancelled;

            // Lưu booking vào cơ sở dữ liệu nếu cần
            await _bookingRepository.UpdateBookingAsync(booking);
            return booking;
        }

        public async Task<BookingServices> CreateBookingServicePayment(int? bookingServiceID, Transaction transaction)
        {
            var bookingService = await _bookingServiceRepository.GetBookingServiceByIdAsync(bookingServiceID);
            if (bookingService == null)
            {
                throw new Exception("BookingService not found");
            }

            bookingService.Transactions ??= new List<Transaction>();

            var service = bookingService.BookingServicesDetails.FirstOrDefault()?.Services;
            if (service != null)
            {
                if (service.Quantity < bookingService.BookingServicesDetails.FirstOrDefault()?.Quantity)
                {
                    throw new Exception("Not enough service quantity available.");
                }

                // Trừ số lượng dịch vụ
                service.Quantity -= bookingService.BookingServicesDetails.FirstOrDefault()?.Quantity ?? 0;
                await _serviceRepository.UpdateAsync(service);  // Cập nhật lại số lượng dịch vụ
            }

            bookingService.Transactions.Add(transaction);

            double totalAmount = bookingService.Total;  // Thay bằng cách tính tổng số tiền thanh toán của booking
            double amountPaid = bookingService.Transactions.Sum(t => t.Amount) / 100; // Tính tổng số tiền đã thanh toán từ tất cả các giao dịch
                                                                                      // Kiểm tra trạng thái thanh toán
            if (amountPaid >= totalAmount)
            {
                bookingService.PaymentServiceStatus = PaymentServicesStatus.FullyPaid; // Thanh toán đầy đủ
            }
            else if (amountPaid > 0)
            {
                bookingService.PaymentServiceStatus = PaymentServicesStatus.Deposited; // Đặt cọc
            }

            bookingService.Status = BookingServicesStatus.Confirmed;

            if (bookingService.BookingID.HasValue)
            {
                var booking = bookingService.Booking;
                if (booking != null)
                {

                    if (bookingService.PaymentServiceStatus == PaymentServicesStatus.FullyPaid)
                    {
                        booking.Total += bookingService.Total;
                    }
                    if (bookingService.PaymentServiceStatus == PaymentServicesStatus.Deposited)
                    {
                        booking.Total += bookingService.Total;
                        booking.bookingDeposit += bookingService.bookingServiceDeposit;
                    }
                    await _bookingRepository.UpdateBookingAsync(booking);  // Cập nhật lại Booking
                }
            }

            transaction.HomeStay = bookingService.HomeStay;
            transaction.Account = bookingService.Account;

            await _bookingServiceRepository.UpdateBookingServicesAsync(bookingService);
            // Lưu booking vào cơ sở dữ liệu nếu cần

            return bookingService;
        }

        public async Task<BookingServices> CreateBookingServiceRefundPayment(int? bookingServiceID, Transaction transaction)
        {
            var bookingService = await _bookingServiceRepository.GetBookingServiceByIdAsync(bookingServiceID);
            if (bookingService == null)
            {
                throw new Exception("BookingService not found");
            }

            bookingService.PaymentServiceStatus = PaymentServicesStatus.Refunded;

            bookingService.Status = BookingServicesStatus.Cancelled;

            bookingService.Transactions ??= new List<Transaction>();

            bookingService.Transactions.Add(transaction);

            var service = bookingService.BookingServicesDetails.FirstOrDefault()?.Services;
            if (service != null)
            {
                service.Quantity += bookingService.BookingServicesDetails.FirstOrDefault()?.Quantity ?? 0;  // Khôi phục số lượng dịch vụ
                await _serviceRepository.UpdateAsync(service);  // Cập nhật lại dịch vụ
            }

            if (bookingService.BookingID.HasValue)
            {
                var booking = bookingService.Booking;
                if (booking != null)
                {
                    // Cập nhật tổng tiền thanh toán cho Booking
                    if (bookingService.PaymentServiceStatus == PaymentServicesStatus.FullyPaid)
                    {
                        booking.Total -= (transaction.Amount/100);
                    }
                    else if (bookingService.PaymentServiceStatus == PaymentServicesStatus.Deposited)
                    {
                        booking.Total -= bookingService.Total;
                        booking.bookingDeposit -= bookingService.bookingServiceDeposit;
                    }
                    await _bookingRepository.UpdateBookingAsync(booking);  // Cập nhật lại Booking
                }
            }

            transaction.HomeStay = bookingService.HomeStay;

            transaction.Account = bookingService.HomeStay.Account;

            await _bookingServiceRepository.UpdateBookingServicesAsync(bookingService);

            return bookingService;
        }
    }
}

