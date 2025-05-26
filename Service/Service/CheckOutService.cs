using AutoMapper;
using BusinessObject.Model;
using Google.Api;
using GreenRoam.Ultilities;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Booking;
using Service.RequestAndResponse.Response.RoomType;
using Service.RequestAndResponse.Response.Services;
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
        private readonly IRoomChangeHistoryRepository _roomchangeHistoryRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;

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
                              ICancellationPolicyRepository cancelltaionRepository, IRoomChangeHistoryRepository roomchangeHistoryRepository,
                              ITransactionRepository transactionRepository,
                              IAccountRepository accountRepository)
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
            _roomchangeHistoryRepository = roomchangeHistoryRepository;
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        //Tạo mã code 8 ký tự ko trùng lập
        private string GenerateShortBookingCode(int length = 8)
        {
            var guidBytes = Guid.NewGuid().ToByteArray();
            var base64 = Convert.ToBase64String(guidBytes);

            var safeCode = base64
                .Replace("/", "")
                .Replace("+", "")
                .Replace("=", "")
                .ToUpper();

            return safeCode.Substring(0, length);
        }

        private async Task<string> GenerateUniqueBookingCodeAsync()
        {
            string code;
            do
            {
                code = GenerateShortBookingCode();
            }
            while (await _bookingRepository.ExistsBookingCodeAsync(code));

            return code;
        }

        public async Task<BaseResponse<int>> CreateBooking(CreateBookingRequest createBookingRequest, PaymentMethod paymentMethod)
        {
            var nowVN = DateTimeHelper.NowVietnamTime();
            TimeZoneInfo vnZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

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

            var homeStayTypeIds = createBookingRequest.BookingDetails
                .Where(d => d.homeStayTypeID.HasValue && d.homeStayTypeID > 0)
                .Select(d => d.homeStayTypeID.Value)
                .ToList();

            var roomTypeIds = createBookingRequest.BookingDetails
                .Where(d => d.roomTypeID.HasValue && d.roomTypeID > 0)
                .Select(d => d.roomTypeID.Value)
                .ToList();

            var homeStayTypes = homeStayTypeIds.Any()
                ? (await _homeStayTypeRepository.GetHomeStayTypesByIdsAsync(homeStayTypeIds.Cast<int?>().ToList())).ToList()
                : new List<HomeStayRentals>();

            var roomTypes = roomTypeIds.Any()
                ? (await _roomTypeRepository.GetRoomTypesByIdsAsync(roomTypeIds.Cast<int?>().ToList())).ToList()
                : new List<RoomTypes>();

            var bookingDetails = new List<BookingDetail>();

            foreach (var detail in createBookingRequest.BookingDetails)
            {
                var checkInLocal = DateTimeHelper.ConvertToVietnamTime(detail.CheckInDate);
                var checkOutLocal = DateTimeHelper.ConvertToVietnamTime(detail.CheckOutDate);

                if (checkOutLocal.Date <= checkInLocal.Date)
                    return new BaseResponse<int>("Check-out date must be after check-in date.", StatusCodeEnum.Conflict_409, 0);

                HomeStayRentals? homeStayType = null;

                if (detail.homeStayTypeID.HasValue && detail.homeStayTypeID > 0)
                {
                    homeStayType = homeStayTypes.FirstOrDefault(h => h.HomeStayRentalID == detail.homeStayTypeID);
                    if (homeStayType == null)
                        return new BaseResponse<int>("Invalid HomeStayRental selection, please try again!", StatusCodeEnum.Conflict_409, 0);
                }

                var total = await _pricingRepository.GetTotalPrice(checkInLocal, checkOutLocal, detail.homeStayTypeID, detail.roomTypeID);
                if (total <= 0)
                {
                    return new BaseResponse<int>("Pricing not found or invalid.", StatusCodeEnum.Conflict_409, 0);
                }

                if (homeStayType?.RentWhole == true)
                {
                    if (detail.roomTypeID > 0 || detail.roomID > 0)
                        return new BaseResponse<int>("RoomTypeID and RoomID should not be selected when renting the whole homestay.", StatusCodeEnum.Conflict_409, 0);
                }
                else
                {
                    var availableRooms = await _roomRepository.GetAvailableRoomFilter(checkInLocal, checkOutLocal);
                    var selectedRoom = availableRooms.FirstOrDefault(r => r.RoomID == detail.roomID);
                    if (selectedRoom == null)
                        return new BaseResponse<int>("Selected room is not available.", StatusCodeEnum.Conflict_409, 0);
                }

                bookingDetails.Add(new BookingDetail
                {
                    CheckInDate = checkInLocal,
                    CheckOutDate = checkOutLocal,
                    HomeStayRentalID = detail.homeStayTypeID,
                    RoomID = detail.roomID,
                    rentPrice = total,
                    TotalAmount = total
                });
            }

            var booking = new Booking
            {
                BookingCode = await GenerateUniqueBookingCodeAsync(),
                BookingDate = nowVN,
                ExpiredTime = nowVN.AddHours(1),
                numberOfAdults = createBookingRequest.numberOfAdults,
                numberOfChildren = createBookingRequest.numberOfChildren,
                Status = BookingStatus.Pending,
                paymentStatus = PaymentStatus.Pending,
                PaymentMethod = paymentMethod,
                AccountID = createBookingRequest.AccountID,
                HomeStayID = createBookingRequest.HomeStayID,
                BookingDetails = bookingDetails
            };

            double totalPriceBooking = bookingDetails.Sum(d => d.TotalAmount);
            double totalAmount = totalPriceBooking;

            booking.TotalRentPrice = totalPriceBooking;
            var depositBooking = commissionRate.PlatformShare * totalPriceBooking;
            booking.Total = totalAmount;

            booking.bookingDeposit = depositBooking;
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
                if (bookingServiceID != null && bookingServiceID > 0)
                {
                    var bookingServiceExist = await _bookingServiceRepository.FindBookingServicesByIdAsync(bookingServiceID);
                    if (bookingServiceExist == null)
                    {
                        return new BaseResponse<Booking>("Cannot find your BookingServices!",
                                 StatusCodeEnum.NotFound_404, null);
                    }

                    if (servicesStatus == BookingServicesStatus.Cancelled)
                    {
                        foreach (var serviceDetail in bookingServiceExist.BookingServicesDetails)
                        {
                            if (serviceDetail?.Services != null && bookingExist.HomeStayID.HasValue)
                            {
                                var cancellationPolicy = await _cancelltaionRepository.GetCancellationPolicyByHomeStayAsync(bookingExist.HomeStayID.Value);
                                var bookingDetail = bookingExist.BookingDetails.FirstOrDefault();

                                if (bookingDetail != null && cancellationPolicy != null)
                                {
                                    var now = DateTime.Now;
                                    var daysBeforeCheckIn = (bookingDetail.CheckInDate - now).TotalDays;

                                    bool isRefundAllowed = daysBeforeCheckIn >= cancellationPolicy.DayBeforeCancel &&
                                        (cancellationPolicy.RefundPercentage > 0 && cancellationPolicy.RefundPercentage <= 1);

                                    if (!isRefundAllowed)
                                    {
                                        serviceDetail.Services.Quantity += serviceDetail.Quantity;
                                        await _serviceRepository.UpdateAsync(serviceDetail.Services);

                                        var serviceTransaction = await _transactionRepository.GetTransactionByBookingServiceId(bookingServiceExist.BookingServicesID);

                                        if (serviceTransaction != null && serviceTransaction.StatusTransaction == StatusOfTransaction.Pending)
                                        {
                                            serviceTransaction.StatusTransaction = StatusOfTransaction.Completed;
                                            await _transactionRepository.UpdateAsync(serviceTransaction);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    await _bookingServiceRepository.ChangeBookingServicesStatus(bookingServiceExist.BookingServicesID, servicesStatus, statusPayment);
                }

                if (status == BookingStatus.Cancelled)
                {
                    var bookingDetails = bookingExist.BookingDetails.FirstOrDefault();
                    if (bookingExist.HomeStayID.HasValue && bookingDetails != null)
                    {
                        var cancellationPolicy = await _cancelltaionRepository.GetCancellationPolicyByHomeStayAsync(bookingExist.HomeStayID.Value);

                        if (cancellationPolicy != null)
                        {
                            var now = DateTime.Now;
                            var daysBeforeCheckIn = (bookingDetails.CheckInDate - now).TotalDays;

                            bool hasRefund = daysBeforeCheckIn >= cancellationPolicy.DayBeforeCancel &&
                                        (cancellationPolicy.RefundPercentage > 0 && cancellationPolicy.RefundPercentage <= 1);

                            // Nếu được hoàn tiền thì đổi sang trạng thái RequestRefund
                            if (!hasRefund)
                            {
                                var transaction = await _transactionRepository.GetTransactionByBookingId(bookingExist.BookingID);
                                if (transaction != null)
                                {

                                    transaction.StatusTransaction = StatusOfTransaction.Completed;
                                    await _transactionRepository.UpdateAsync(transaction);
                                }
                            }
                        }
                    }

                    if (bookingExist.BookingServices != null && bookingExist.BookingServices.Any())
                    {
                        foreach (var service in bookingExist.BookingServices)
                        {
                            if (service.Status == BookingServicesStatus.Completed)
                            {
                                continue;
                            }
                          
                            if (service.Status != BookingServicesStatus.Cancelled &&
                                service.PaymentServiceStatus != PaymentServicesStatus.Refunded &&
                                service.Status != BookingServicesStatus.Pending)
                            {
                                foreach (var serviceDetail in service.BookingServicesDetails)
                                {
                                    if (serviceDetail?.Services != null && bookingExist.HomeStayID.HasValue)
                                    {
                                        var cancellationPolicy = await _cancelltaionRepository.GetCancellationPolicyByHomeStayAsync(bookingExist.HomeStayID.Value);
                                        var bookingDetail = bookingExist.BookingDetails.FirstOrDefault();

                                        if (bookingDetail != null && cancellationPolicy != null)
                                        {
                                            var now = DateTime.Now;
                                            var daysBeforeCheckIn = (bookingDetail.CheckInDate - now).TotalDays;

                                            bool isRefundAllowed = daysBeforeCheckIn >= cancellationPolicy.DayBeforeCancel &&
                                                (cancellationPolicy.RefundPercentage > 0 && cancellationPolicy.RefundPercentage <= 1);

                                            if (!isRefundAllowed)
                                            {
                                                serviceDetail.Services.Quantity += serviceDetail.Quantity;
                                                await _serviceRepository.UpdateAsync(serviceDetail.Services);

                                                var serviceTransaction = await _transactionRepository.GetTransactionByBookingServiceId(service.BookingServicesID);

                                                if (serviceTransaction != null && serviceTransaction.StatusTransaction == StatusOfTransaction.Pending)
                                                {
                                                    serviceTransaction.StatusTransaction = StatusOfTransaction.Completed;
                                                    await _transactionRepository.UpdateAsync(serviceTransaction);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            await _bookingServiceRepository.ChangeBookingServicesStatus(service.BookingServicesID, BookingServicesStatus.Cancelled, service.PaymentServiceStatus);
                        }
                    }
                }

                if (status == BookingStatus.Completed) // Điều kiện cho trạng thái checkout
                {
                    var transaction = await _transactionRepository.GetTransactionByBookingId(bookingExist.BookingID);
                    if (transaction != null)
                    {

                        transaction.StatusTransaction = StatusOfTransaction.Completed;
                        await _transactionRepository.UpdateAsync(transaction);
                    }
                    else
                    {
                        return new BaseResponse<Booking>("The booking does not have a transaction!",
                                     StatusCodeEnum.BadGateway_502, null);
                    }

                    if (bookingExist.BookingServices != null && bookingExist.BookingServices.Any())
                    {
                        foreach (var service in bookingExist.BookingServices)
                        {
                            if (service.Status == BookingServicesStatus.Completed)
                            {
                                continue;
                            }
                            if (service.Status != BookingServicesStatus.Cancelled &&
                                service.PaymentServiceStatus != PaymentServicesStatus.Refunded &&
                                service.Status != BookingServicesStatus.Pending)
                            {
                                foreach (var serviceDetail in service.BookingServicesDetails)
                                {
                                    if (serviceDetail?.Services != null && bookingExist.HomeStayID.HasValue) 
                                    {
                                        serviceDetail.Services.Quantity += serviceDetail.Quantity;
                                        await _serviceRepository.UpdateAsync(serviceDetail.Services);
                                    }
                                }
                            }
                            var serviceTransaction = await _transactionRepository.GetTransactionByBookingServiceId(service.BookingServicesID);

                            if (serviceTransaction != null && serviceTransaction.StatusTransaction == StatusOfTransaction.Pending)
                            {
                                serviceTransaction.StatusTransaction = StatusOfTransaction.Completed;
                                await _transactionRepository.UpdateAsync(serviceTransaction);
                            }
                            await _bookingServiceRepository.ChangeBookingServicesStatus(service.BookingServicesID, BookingServicesStatus.Completed, service.PaymentServiceStatus);
                        }
                    }
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
                        existingDetail.rentPrice = pricing;
                        existingDetail.TotalAmount = pricing;
                    }
                    else
                    {
                        existingBooking.BookingDetails.Add(new BookingDetail
                        {
                            HomeStayRentalID = updatedBookingDetails.homeStayTypeID,
                            RoomID = isRentWhole ? null : updatedBookingDetails.roomID,
                            CheckInDate = updatedBookingDetails.CheckInDate,
                            CheckOutDate = updatedBookingDetails.CheckOutDate,
                            rentPrice = pricing,
                            TotalAmount = pricing
                        });
                    }
                }
            }

            var commissionrate = await _commissionRateRepository.GetCommissionByHomeStayAsync(existingBooking.HomeStayID);
            if (commissionrate == null || commissionrate.PlatformShare <= 0 || commissionrate.PlatformShare > 1)
                return new BaseResponse<UpdateBookingRequest>("Invalid commission setting!", StatusCodeEnum.Conflict_409, null);

            var totalPriceExistBooking = existingBooking.BookingDetails.Sum(detail => detail.TotalAmount);
            var totalPriceAmount = totalPriceExistBooking;
            var depositBooking = commissionrate.PlatformShare * totalPriceExistBooking;
            var deposit = depositBooking;
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

        public async Task<BaseResponse<UpdateBookingForRoomRequest>> ChangeRoomForBooking(int bookingID, UpdateBookingForRoomRequest request)
        {
            var existingBooking = await _bookingRepository.GetBookingByIdAsync(bookingID);
            if (existingBooking == null)
            {
                return new BaseResponse<UpdateBookingForRoomRequest>("Cannot find your Booking!",
                         StatusCodeEnum.NotFound_404, null);
            }

            bool isCompleted = existingBooking.Status == BookingStatus.Completed;
            bool isCancelled = existingBooking.Status == BookingStatus.Cancelled;

            if (isCompleted)
            {
                return new BaseResponse<UpdateBookingForRoomRequest>("This booking is already completed and cannot be modified.",
                           StatusCodeEnum.NotFound_404, null);
            }

            if (isCancelled)
            {
                return new BaseResponse<UpdateBookingForRoomRequest>("This booking is already cancelled and cannot be modified.",
                           StatusCodeEnum.NotFound_404, null);
            }

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
                return new BaseResponse<UpdateBookingForRoomRequest>($"RoomID(s) duplicated in request: " +
                    $"{string.Join(", ", duplicatedRoomIDs)}", StatusCodeEnum.Conflict_409, null);
            }


            var originalBookingDetail = existingBooking.BookingDetails.FirstOrDefault();
            if (originalBookingDetail == null)
            {
                return new BaseResponse<UpdateBookingForRoomRequest>("Booking detail not found.", StatusCodeEnum.BadRequest_400, null);
            }
            var originalCheckout = originalBookingDetail.CheckOutDate;

            var today = DateTime.UtcNow;

            foreach (var updatedBookingDetails in request.BookingDetails)
            {
                if (updatedBookingDetails.roomTypeID > 0)
                {
                    if (updatedBookingDetails.CheckOutDate != originalCheckout)
                    {
                        if (updatedBookingDetails.CheckOutDate <= today)
                        {
                            return new BaseResponse<UpdateBookingForRoomRequest>("Cannot change room because the new CheckOutDate has already passed or is today.", StatusCodeEnum.BadRequest_400, null);
                        }

                        return new BaseResponse<UpdateBookingForRoomRequest>("Changing CheckOutDate is not allowed. Please use the original CheckOutDate.", StatusCodeEnum.BadRequest_400, null);
                    }
                    
                    var roomType = await _roomTypeRepository.GetRoomTypesByIdAsync(updatedBookingDetails.roomTypeID);
                    if (roomType == null)
                    {
                        return new BaseResponse<UpdateBookingForRoomRequest>("Invalid RoomType.", StatusCodeEnum.BadRequest_400, null);
                    }

                    if (updatedBookingDetails.homeStayTypeID != roomType.HomeStayRentalID)
                    {
                        return new BaseResponse<UpdateBookingForRoomRequest>("RoomType does not belong to this HomeStayRental.", StatusCodeEnum.BadRequest_400, null);
                    }

                    var room = await _roomRepository.GetRoomByIdAsync(updatedBookingDetails.roomID.Value);
                    if (room == null || room.RoomTypesID != updatedBookingDetails.roomTypeID)
                    {
                        return new BaseResponse<UpdateBookingForRoomRequest>("Room does not belong to the specified RoomType.", StatusCodeEnum.BadRequest_400, null);
                    }

                    var availableRooms = await _roomRepository.GetAvailableRoomFilter(updatedBookingDetails.CheckInDate, updatedBookingDetails.CheckOutDate);
                    if (!availableRooms.Any(r => r.RoomID == updatedBookingDetails.roomID && r.RoomTypesID == updatedBookingDetails.roomTypeID))
                    {
                        return new BaseResponse<UpdateBookingForRoomRequest>("Room is not available.", StatusCodeEnum.Conflict_409, null);
                    }
                }
                else
                {
                    return new BaseResponse<UpdateBookingForRoomRequest>("Must provide RoomTypeID.", StatusCodeEnum.BadRequest_400, null);
                }

                if (updatedBookingDetails.BookingDetailID.HasValue)
                {
                    var existingDetail = existingBooking.BookingDetails
                        .FirstOrDefault(d => d.BookingDetailID == updatedBookingDetails.BookingDetailID.Value);
                    if (existingDetail == null)
                        return new BaseResponse<UpdateBookingForRoomRequest>("Booking detail not found.", StatusCodeEnum.BadRequest_400, null);
                    if (existingDetail.RoomID != updatedBookingDetails.roomID)
                    {
                        var roomChange = new RoomChangeHistory
                        {
                            BookingDetailID = existingDetail.BookingDetailID,
                            OldRoomID = existingDetail.RoomID,
                            NewRoomID = updatedBookingDetails.roomID,
                            UsagedDate = updatedBookingDetails.CheckInDate,
                            ChangedDate = DateTime.UtcNow,
                            AccountID = request.AccountID,// bạn cần truyền field này từ request
                        };
                        await _roomchangeHistoryRepository.AddRoomHistory(roomChange);
                    }

                    existingDetail.HomeStayRentalID = updatedBookingDetails.homeStayTypeID;
                    existingDetail.RoomID = updatedBookingDetails.roomID;
                    existingDetail.CheckInDate = updatedBookingDetails.CheckInDate;
                    existingDetail.CheckOutDate = updatedBookingDetails.CheckOutDate;

                }
                else
                {
                    existingBooking.BookingDetails.Add(new BookingDetail
                    {
                        HomeStayRentalID = updatedBookingDetails.homeStayTypeID,
                        RoomID = updatedBookingDetails.roomID,
                        CheckInDate = updatedBookingDetails.CheckInDate,
                        CheckOutDate = updatedBookingDetails.CheckOutDate,
                    });
                }
            }
            await _bookingRepository.UpdateBookingAsync(existingBooking);
            return new BaseResponse<UpdateBookingForRoomRequest>("Booking updated successfully!", StatusCodeEnum.OK_200, request);
        }

        public async Task<Booking> CreateBookingPayment(int? bookingID, int? bookingServiceID, Transaction transaction)
        {
            var booking = await _bookingRepository.GetBookingsByIdAsync(bookingID);
            if (booking == null)
            {
                throw new Exception("Booking not found");
            }
            booking.Transactions ??= new List<Transaction>();

            bool alreadyExists = booking.Transactions.Any(t => t.ResponseId == transaction.ResponseId && transaction.ResponseId != null);

            if (alreadyExists)
            {
                throw new Exception("Duplicate transaction detected.");
            }

            var commissionRate = await _commissionRateRepository.GetCommissionByHomeStayAsync(booking.HomeStayID);
            if (commissionRate == null)
            {
                throw new Exception("commissionRate not found");
            }

            transaction.HomeStay = booking.HomeStay;
            transaction.Account = booking.Account;

            // Thêm transaction vào trong danh sách Transactions
            booking.Transactions.Add(transaction);

            double totalAmount = booking.Total;  // Thay bằng cách tính tổng số tiền thanh toán của booking
            double amountPaid = booking.Transactions.Sum(t => t.Amount); // Tính tổng số tiền đã thanh toán từ tất cả các giao dịch

            // Kiểm tra trạng thái thanh toán
            if (amountPaid >= totalAmount)
            {
                booking.paymentStatus = PaymentStatus.FullyPaid; // Thanh toán đầy đủ
                transaction.TransactionKind = TransactionKind.FullPayment;
            }
            else if (amountPaid > 0)
            {
                booking.paymentStatus = PaymentStatus.Deposited; // Đặt cọc
                transaction.TransactionKind = TransactionKind.Deposited;
            }
            transaction.AdminAmount = commissionRate.PlatformShare * amountPaid;
            transaction.OwnerAmount = commissionRate.HostShare * amountPaid;
            transaction.StatusTransaction = StatusOfTransaction.Pending;
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

            var commissionRate = await _commissionRateRepository.GetCommissionByHomeStayAsync(booking.HomeStayID);
            if (commissionRate == null)
            {
                throw new Exception("CommissionRate not found");
            }

            var originalTransaction = booking.Transactions
                      .FirstOrDefault(t => (t.TransactionKind == TransactionKind.FullPayment || t.TransactionKind == TransactionKind.Deposited)
                      && (t.StatusTransaction == StatusOfTransaction.Pending || t.StatusTransaction == StatusOfTransaction.RequestCancel ||
                      t.StatusTransaction == StatusOfTransaction.RequestRefund));

            if (originalTransaction == null)
                throw new Exception("No pending or request cancel payment transaction found to refund.");

            bool wasRequestCancel = originalTransaction.StatusTransaction == StatusOfTransaction.RequestCancel;

            originalTransaction.StatusTransaction = StatusOfTransaction.Cancelled;
            await _transactionRepository.UpdateAsync(originalTransaction);

            booking.Transactions ??= new List<Transaction>();

            bool alreadyExists = booking.Transactions.Any(t =>
            t.ResponseId == transaction.ResponseId && transaction.ResponseId != null);

            if (alreadyExists)
            {
                throw new Exception("Duplicate transaction detected.");
            }

            transaction.HomeStay = booking.HomeStay;
            transaction.TransactionKind = TransactionKind.Refund;
            transaction.StatusTransaction = StatusOfTransaction.Refunded;

            // Thêm transaction vào trong danh sách Transactions
            booking.Transactions.Add(transaction);
            double amountPaid = transaction.Amount;

            var bookingServices = await _bookingServiceRepository.GetConfirmedBookingServiceByBookingId(bookingID);
            if (bookingServices != null)
            {
                foreach (var service in bookingServices)
                {
                    var originalServiceTransaction = service.Transactions?
                                .FirstOrDefault(t => (t.TransactionKind == TransactionKind.FullPayment || t.TransactionKind == TransactionKind.Deposited)
                                 && (t.StatusTransaction == StatusOfTransaction.Pending || t.StatusTransaction == StatusOfTransaction.RequestCancel ||
                                 t.StatusTransaction == StatusOfTransaction.RequestRefund));

                    if (originalServiceTransaction != null)
                    {
                        /* if (amountPaid < booking.Total + bookingServices.Sum(s => s.Total))
                         {
                             originalServiceTransaction.StatusTransaction = StatusOfTransaction.Cancelled;
                             await _transactionRepository.UpdateAsync(originalServiceTransaction);
                         }

                         if (amountPaid == booking.Total + bookingServices.Sum(s => s.Total))
                         {
                             originalServiceTransaction.StatusTransaction = StatusOfTransaction.Completed;
                             await _transactionRepository.UpdateAsync(originalServiceTransaction);
                         }*/

                        originalServiceTransaction.StatusTransaction = StatusOfTransaction.Cancelled;
                        await _transactionRepository.UpdateAsync(originalServiceTransaction);
                    }

                    service.Status = BookingServicesStatus.Cancelled;
                    service.PaymentServiceStatus = PaymentServicesStatus.Refunded;
                    service.Transactions ??= new List<Transaction>();

                    transaction.HomeStay = service.HomeStay;
                    transaction.TransactionKind = TransactionKind.Refund;
                    transaction.StatusTransaction = StatusOfTransaction.Refunded;

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

            /*if (amountPaid < booking.Total + bookingServices.Sum(s => s.Total))
            {
                originalTransaction.StatusTransaction = StatusOfTransaction.Cancelled;
                await _transactionRepository.UpdateAsync(originalTransaction);
            }

            if (amountPaid == booking.Total + bookingServices.Sum(s => s.Total))
            {
                originalTransaction.StatusTransaction = StatusOfTransaction.Completed;
                await _transactionRepository.UpdateAsync(originalTransaction);
            }*/

            if (wasRequestCancel)
            {
                var recipientEmail = booking.Account?.Email;
                if (!string.IsNullOrEmpty(recipientEmail))
                {
                    string subject = "Yêu cầu hủy đặt phòng đã được xử lý";
                    string body = $@"
                    <html>
                    <body>
                        <p>Xin chào {booking.Account?.Name ?? "quý khách"},</p>
                        <p>Do sự cố phát sinh từ phía HomeStay <strong>{booking.HomeStay.Name}</strong>, 
                        đơn đặt phòng của bạn (mã: <strong>{booking.BookingCode}</strong>) đã được hủy và hoàn tiền.</p>
                        <p>Số tiền đã hoàn: <strong>{transaction.Amount:N0} VND</strong>.</p>
                        <p>Chúng tôi rất lấy làm tiếc về sự việc này, rất mong quý khách thông cảm cho bên HomeStay</p>
                        <br>
                        <p>Trân trọng,</p>
                        <p><strong>ChoTot-Travel-CTT</strong></p>
                    </body>
                    </html>";

                    _accountRepository.SendEmail(recipientEmail, subject, body);
                }
            }

            transaction.AdminAmount = (-commissionRate.PlatformShare) * amountPaid;
            transaction.OwnerAmount = (-commissionRate.HostShare) * amountPaid;
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

            bool alreadyExists = bookingService.Transactions.Any(t =>
            t.ResponseId == transaction.ResponseId && transaction.ResponseId != null);

            if (alreadyExists)
            {
                throw new Exception("Duplicate transaction detected.");
            }

            var commissionRate = await _commissionRateRepository.GetCommissionByHomeStayAsync(bookingService.HomeStayID);
            if (commissionRate == null)
            {
                throw new Exception("commissionRate not found");
            }
            if (bookingService.BookingServicesDetails != null)
            {
                // Trừ số lượng cho tất cả các dịch vụ trong BookingServicesDetails
                foreach (var detail in bookingService.BookingServicesDetails)
                {
                    var service = detail.Services;
                    if (service != null)
                    {
                        if (service.Quantity < detail.Quantity)
                        {
                            throw new Exception($"Not enough quantity available for service '{service.servicesName}'.");
                        }

                        service.Quantity -= detail.Quantity;
                        await _serviceRepository.UpdateAsync(service);  // Cập nhật lại số lượng dịch vụ
                    }
                }
            }

            bookingService.Transactions.Add(transaction);

            double totalAmount = bookingService.Total;  // Thay bằng cách tính tổng số tiền thanh toán của booking
            double amountPaid = bookingService.Transactions.Sum(t => t.Amount); // Tính tổng số tiền đã thanh toán từ tất cả các giao dịch
                                                                                // Kiểm tra trạng thái thanh toán
            if (amountPaid >= totalAmount)
            {
                bookingService.PaymentServiceStatus = PaymentServicesStatus.FullyPaid; // Thanh toán đầy đủ
                transaction.TransactionKind = TransactionKind.FullPayment;
            }
            else if (amountPaid > 0)
            {
                bookingService.PaymentServiceStatus = PaymentServicesStatus.Deposited; // Đặt cọc
                transaction.TransactionKind = TransactionKind.Deposited;
            }

            bookingService.Status = BookingServicesStatus.Confirmed;

            transaction.AdminAmount = commissionRate.PlatformShare * amountPaid;
            transaction.OwnerAmount = commissionRate.HostShare * amountPaid;
            transaction.StatusTransaction = StatusOfTransaction.Pending;
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

            var commissionRate = await _commissionRateRepository.GetCommissionByHomeStayAsync(bookingService.HomeStayID);
            if (commissionRate == null)
            {
                throw new Exception("commissionRate not found");
            }

            var originalServiceTransaction = bookingService.Transactions?
                                .FirstOrDefault(t => (t.TransactionKind == TransactionKind.FullPayment || t.TransactionKind == TransactionKind.Deposited)
                                 && (t.StatusTransaction == StatusOfTransaction.Pending || t.StatusTransaction == StatusOfTransaction.RequestCancel ||
                                 t.StatusTransaction == StatusOfTransaction.RequestRefund));

            if (originalServiceTransaction == null)
                throw new Exception("No pending or request cancel payment transaction found to refund.");

            bool wasRequestCancel = originalServiceTransaction.StatusTransaction == StatusOfTransaction.RequestCancel;

            double amountPaid = transaction.Amount;

            if (originalServiceTransaction != null)
            {
                /* if(amountPaid < bookingService.Total)
                 {
                     originalServiceTransaction.StatusTransaction = StatusOfTransaction.Cancelled;
                     await _transactionRepository.UpdateAsync(originalServiceTransaction);
                 }

                 if(amountPaid == bookingService.Total)
                 {
                     originalServiceTransaction.StatusTransaction = StatusOfTransaction.Completed;
                     await _transactionRepository.UpdateAsync(originalServiceTransaction);
                 } */

                originalServiceTransaction.StatusTransaction = StatusOfTransaction.Cancelled;
                await _transactionRepository.UpdateAsync(originalServiceTransaction);
            }

            var oldPaymentServiceStatus = bookingService.PaymentServiceStatus;

            bookingService.PaymentServiceStatus = PaymentServicesStatus.Refunded;

            bookingService.Status = BookingServicesStatus.Cancelled;

           

            transaction.TransactionKind = TransactionKind.Refund;
            transaction.StatusTransaction = StatusOfTransaction.Refunded;

            bookingService.Transactions ??= new List<Transaction>();

            bool alreadyExists = bookingService.Transactions.Any(t =>
            t.ResponseId == transaction.ResponseId && transaction.ResponseId != null);

            if (alreadyExists)
            {
                throw new Exception("Duplicate transaction detected.");
            }

            if (wasRequestCancel)
            {
                var recipientEmail = bookingService.Account?.Email;
                if (!string.IsNullOrEmpty(recipientEmail))
                {
                    string subject = "Yêu cầu hủy đặt phòng đã được xử lý";
                    string body = $@"
                    <html>
                    <body>
                        <p>Xin chào {bookingService.Account?.Name ?? "quý khách"},</p>
                        <p>Do sự cố phát sinh từ phía HomeStay <strong>{bookingService.HomeStay.Name}</strong>, 
                        đơn đặt phòng của bạn (mã: <strong>{bookingService.BookingServiceCode}</strong>) đã được hủy và hoàn tiền.</p>
                        <p>Số tiền đã hoàn: <strong>{transaction.Amount:N0} VND</strong>.</p>
                        <p>Chúng tôi rất lấy làm tiếc về sự việc này, rất mong quý khách thông cảm cho bên HomeStay</p>
                        <br>
                        <p>Trân trọng,</p>
                        <p><strong>ChoTot-Travel-CTT</strong></p>
                    </body>
                    </html>";

                    _accountRepository.SendEmail(recipientEmail, subject, body);
                }
            }

            transaction.HomeStay = bookingService.HomeStay;
            transaction.AdminAmount = (-commissionRate.PlatformShare) * amountPaid;
            transaction.OwnerAmount = (-commissionRate.HostShare) * amountPaid;

            bookingService.Transactions.Add(transaction);

            if (bookingService.BookingServicesDetails != null)
            {
                foreach (var detail in bookingService.BookingServicesDetails)
                {
                    var service = detail.Services;
                    if (service != null)
                    {
                        service.Quantity += detail.Quantity;
                        await _serviceRepository.UpdateAsync(service);
                    }
                }
            }

            await _bookingServiceRepository.UpdateBookingServicesAsync(bookingService);

            return bookingService;
        }

        public async Task<BaseResponse<List<GetRoomTypeStats>>> GetRoomTypeUsageStatsAsync(int homestayId)
        {
            var rawStats = await _bookingDetailRepository.GetRoomTypeUsageStatsAsync(homestayId);

            var stats = rawStats.Select(tuple => new GetRoomTypeStats
            {
                RoomTypeName = tuple.roomTypeName,
                Count = tuple.count,

            }).ToList();

            if (stats == null || !stats.Any())
            {
                return new BaseResponse<List<GetRoomTypeStats>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<List<GetRoomTypeStats>>("Get all bookings as base success",
                StatusCodeEnum.OK_200, stats);
        }

        public async Task<BaseResponse<Transaction?>> OwnerAcceptRefundAsync(int? bookingId, int? bookingServiceId)
        {
            if (bookingId.HasValue)
            {
                var booking = await _bookingRepository.GetBookingsByIdAsync(bookingId.Value);

                if (booking is null)
                {
                    return new BaseResponse<Transaction?>("Booking not found.", StatusCodeEnum.NotFound_404, null);
                }

                if (booking.Status != BookingStatus.RequestRefund)
                {
                    return new BaseResponse<Transaction?>("This booking is not requesting a refund.", StatusCodeEnum.BadRequest_400, null);
                }

                var transaction = await _transactionRepository.ChangeTransactionStatusForBooking(bookingId.Value, StatusOfTransaction.RequestRefund);

                if (transaction is null)
                {
                    return new BaseResponse<Transaction?>("Failed to update transaction status.", StatusCodeEnum.InternalServerError_500, null);
                }

                /*if (booking.BookingServices?.Any() == true)
                {
                    foreach (var serviceBooking in booking.BookingServices)
                    {
                        var bookingService = await _bookingServiceRepository.GetBookingServiceByIdAsync(serviceBooking.BookingServicesID);

                        if (bookingService == null)
                        {
                            continue;
                        }

                        if (bookingService.Status == BookingServicesStatus.Completed || bookingService.Status == BookingServicesStatus.Cancelled || bookingService.Status == BookingServicesStatus.Pending)
                        {
                            continue;
                        }

                        var transactionService = await _transactionRepository.ChangeTransactionStatusForBookingService(serviceBooking.BookingServicesID, StatusOfTransaction.Pending);

                        if (transactionService is null)
                        {
                            return new BaseResponse<Transaction?>("Failed to update transaction status.", StatusCodeEnum.InternalServerError_500, null);
                        }
                    }
                }*/

                await _bookingRepository.ChangeBookingStatus(booking.BookingID, BookingStatus.AcceptedRefund, booking.paymentStatus);

                return new BaseResponse<Transaction?>("Transaction status updated to RequestRefund successfully.", StatusCodeEnum.OK_200, transaction);
            }

            if (bookingServiceId.HasValue)
            {
                var bookingService = await _bookingServiceRepository.GetBookingServiceByIdAsync(bookingServiceId.Value);

                if (bookingService is null)
                {
                    return new BaseResponse<Transaction?>("Booking service not found.", StatusCodeEnum.NotFound_404, null);
                }

                if (bookingService.Status != BookingServicesStatus.RequestRefund)
                {
                    return new BaseResponse<Transaction?>("This booking service is not requesting a refund.", StatusCodeEnum.BadRequest_400, null);
                }

                var transaction = await _transactionRepository.ChangeTransactionStatusForBookingService(bookingServiceId.Value, StatusOfTransaction.RequestRefund);

                if (transaction is null)
                {
                    return new BaseResponse<Transaction?>("Failed to update transaction status.", StatusCodeEnum.InternalServerError_500, null);
                }

                await _bookingServiceRepository.ChangeBookingServicesStatus(bookingService.BookingServicesID, BookingServicesStatus.AcceptedRefund, bookingService.PaymentServiceStatus);

                return new BaseResponse<Transaction?>("Transaction status updated to RequestRefund successfully.", StatusCodeEnum.OK_200, transaction);
            }

            return new BaseResponse<Transaction?>("Either bookingId or bookingServiceId must be provided.", StatusCodeEnum.BadRequest_400, null);
        }

        public async Task<BaseResponse<Booking>> RequestCancelBookingStatus(int bookingId)
        {
            var bookingExist = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (bookingExist == null)
            {
                return new BaseResponse<Booking>("Cannot find your Booking!",
                         StatusCodeEnum.NotFound_404, null);
            }
            else
            {
                if (bookingExist.Status == BookingStatus.RequestRefund)
                {
                    return new BaseResponse<Booking>("This booking is requesting a refund, cannot cancel immediately", StatusCodeEnum.BadRequest_400, null);
                }

                if (bookingExist.Status == BookingStatus.Confirmed)
                {
                    await _bookingRepository.ChangeBookingStatus(bookingExist.BookingID, BookingStatus.RequestCancel, bookingExist.paymentStatus);

                    var transaction = await _transactionRepository.ChangeTransactionStatusForBooking(bookingId, StatusOfTransaction.RequestCancel);

                    if (transaction is null)
                    {
                        return new BaseResponse<Booking>("Failed to update transaction status.", StatusCodeEnum.InternalServerError_500, null);
                    }

                    if (bookingExist.BookingServices?.Any() == true)
                    {
                        foreach (var serviceBooking in bookingExist.BookingServices)
                        {
                            var bookingService = await _bookingServiceRepository.GetBookingServiceByIdAsync(serviceBooking.BookingServicesID);

                            if (bookingService == null)
                            {
                                continue;
                            }

                            if (bookingService.Status == BookingServicesStatus.Completed || bookingService.Status == BookingServicesStatus.Cancelled || bookingService.Status == BookingServicesStatus.Pending)
                            {
                                continue;
                            }

                            var transactionService = await _transactionRepository.ChangeTransactionStatusForBookingService(serviceBooking.BookingServicesID, StatusOfTransaction.RequestCancel);

                            if (transactionService is null)
                            {
                                return new BaseResponse<Booking>("Failed to update transaction status.", StatusCodeEnum.InternalServerError_500, null);
                            }

                            await _bookingServiceRepository.ChangeBookingServicesStatus(serviceBooking.BookingServicesID, BookingServicesStatus.RequestCancel, serviceBooking.PaymentServiceStatus);
                        }
                    }

                    return new BaseResponse<Booking>("Booking and Transaction status updated to RequestCancel successfully.", StatusCodeEnum.OK_200, bookingExist);
                }
                else
                {
                    return new BaseResponse<Booking>("Cannot send request cancel!", StatusCodeEnum.OK_200, null);
                }
                 
            }
        }
    }
}

