using AutoMapper;
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

                var bookingServices = new BookingServices
                {
                    BookingServicesDate = DateTime.Now,
                    Status = BookingServicesStatus.Pending,
                    PaymentServicesMethod = PaymentServicesMethod.VnPay,
                    PaymentServiceStatus = PaymentServicesStatus.Pending,
                    AccountID = createBookingRequest.AccountID,
                    BookingServicesDetails = createBookingRequest.BookingOfServices.BookingServicesDetails.Select(s => new BookingServicesDetail
                    {
                        Quantity = s.Quantity,
                        unitPrice = s.Quantity * services.FirstOrDefault(x => x.ServicesID == s.ServicesID).UnitPrice,
                        TotalAmount = s.Quantity * services.First(x => x.ServicesID == s.ServicesID).servicesPrice,
                        ServicesID = s.ServicesID
                    }).ToList()
                };

                bookingServices.Total = bookingServices.BookingServicesDetails.Sum(d => d.TotalAmount);
                booking.BookingServices = new List<BookingServices> { bookingServices };
                totalPriceServices = bookingServices.Total;
            }

            double totalPriceBooking = bookingDetails.Sum(d => d.TotalAmount);
            double totalAmount = totalPriceBooking + totalPriceServices;

            booking.TotalRentPrice = totalPriceBooking;
            booking.Total = totalAmount;
            booking.bookingDeposit = commissionRate.PlatformShare * totalAmount;
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
            var totalPriceAmount = totalPriceExistBooking + totalPriceExistService;
            var deposit = commissionrate.PlatformShare * totalPriceAmount;
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

            if (bookingServiceID.HasValue && bookingServiceID.Value > 0)
            {
                var bookingService = await _bookingServiceRepository.GetBookingServiceByIdAsync(bookingServiceID);
                if (bookingService == null)
                {
                    throw new Exception("BookingService not found");
                }

                bookingService.PaymentServiceStatus = PaymentServicesStatus.Refunded;
                bookingService.Status = BookingServicesStatus.Cancelled;
                bookingService.Transactions ??= new List<Transaction>();
                transaction.HomeStay = bookingService.HomeStay;
                transaction.Account = bookingService.HomeStay.Account;
                bookingService.Transactions.Add(transaction);
                await _bookingServiceRepository.UpdateBookingServicesAsync(bookingService);
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

            transaction.HomeStay = bookingService.HomeStay;
            transaction.Account = bookingService.Account;

            bookingService.Transactions ??= new List<Transaction>();

            bookingService.Transactions.Add(transaction);
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

            transaction.HomeStay = bookingService.HomeStay;

            transaction.Account = bookingService.HomeStay.Account;

            bookingService.Transactions.Add(transaction);

            await _bookingServiceRepository.UpdateBookingServicesAsync(bookingService);

            return bookingService;
        }
    }
}

