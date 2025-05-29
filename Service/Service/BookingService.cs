using AutoMapper;
using Azure;
using BusinessObject.Model;
using CloudinaryDotNet;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;
using Repository.IRepositories;
using Repository.Repositories;
using Service.Hubs;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Booking;
using Service.RequestAndResponse.Request.BookingDetail;
using Service.RequestAndResponse.Request.BookingServices;
using Service.RequestAndResponse.Response.Accounts;
using Service.RequestAndResponse.Response.BookingOfServices;
using Service.RequestAndResponse.Response.Bookings;
using Service.RequestAndResponse.Response.HomeStays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Cloud.Dialogflow.V2.Intent.Types.Message.Types.CarouselSelect.Types;
using static Google.Rpc.Context.AttributeContext.Types;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Service.Service
{
    public class BookingService : IBookingService
    {
        private readonly IMapper _mapper;
        private readonly IBookingRepository _bookingRepository;
        private readonly ICancellationPolicyRepository _cancelltaionRepository;
        private readonly IHomeStayRepository _homeStayRepository;
        private readonly INotificationService _notificationService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICommissionRateRepository _commissionRateRepository;
        private readonly IHubContext<NotificationHubs> _notificationHub;
        private readonly IServiceRepository _serviceRepository;
        private readonly IBookingServiceRepository _bookingServiceRepository;

        public BookingService(IMapper mapper, IBookingRepository bookingRepository,
                              ICancellationPolicyRepository cancelltaionRepository,
                              IHubContext<NotificationHubs> notificationHub,
                              ITransactionRepository transactionRepository, 
                              ICommissionRateRepository commissionRateRepository,
                              IServiceRepository serviceRepository,
                              IBookingServiceRepository bookingServiceRepository)
        {
            _mapper = mapper;
            _bookingRepository = bookingRepository;
            _cancelltaionRepository = cancelltaionRepository;
            _notificationHub = notificationHub;
            _transactionRepository = transactionRepository;
            _commissionRateRepository = commissionRateRepository;
            _serviceRepository = serviceRepository;
            _bookingServiceRepository = bookingServiceRepository;
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

        public async Task<BaseResponse<GetCancellationBooking>> GetCancellationBooking(int bookingID)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingID);
            if (booking == null)
            {
                return new BaseResponse<GetCancellationBooking>("Get Booking Fail", StatusCodeEnum.BadGateway_502, null);
            }

            if (booking.Status == BookingStatus.Cancelled && booking.paymentStatus == PaymentStatus.Refunded)
            {
                var cancellationRate = await _cancelltaionRepository.GetCancellationPolicyByHomeStayAsync(booking.HomeStayID);
                if (cancellationRate == null)
                {
                    return new BaseResponse<GetCancellationBooking>("Get CancellationPolicy Fail", StatusCodeEnum.BadGateway_502, null);
                }

                var bookingResponse = _mapper.Map<GetCancellationBooking>(booking);

                var checkInDate = booking.BookingDetails.First().CheckInDate;

                var daysBeforeCheckIn = (checkInDate - DateTime.Now).TotalDays;

                bookingResponse.CancelFee = daysBeforeCheckIn >= cancellationRate.DayBeforeCancel
                ? booking.Total * (1 - cancellationRate.RefundPercentage)
                : booking.Total; // Không hoàn tiền nếu hủy sát ngày

                return new BaseResponse<GetCancellationBooking>("Get Success Cancellation Booking", StatusCodeEnum.OK_200, bookingResponse);
            }
            else
            {
                return new BaseResponse<GetCancellationBooking>("Booking is not cancelled or refunded", StatusCodeEnum.BadRequest_400, null);
            }
        }

        public async Task<BaseResponse<Booking>> GetBookingsById(int bookingID)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingID);
            if (booking == null)
            {
                return new BaseResponse<Booking>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }

            return new BaseResponse<Booking>("Get all bookings as base success",
                StatusCodeEnum.OK_200, booking);
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
                bookingsReturnRefund = booking.bookingsReturnRefund,
                bookingConfirmed = booking.bookingConfirmed
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

        public async Task<BaseResponse<IEnumerable<GetBookingByHomeStay>>> GetBookingsByHomeStayId(int homeStayID)
        {
            IEnumerable<Booking> booking = await _bookingRepository.GetBookingsByHomeStayId(homeStayID);
            if (booking == null || !booking.Any())
            {
                return new BaseResponse<IEnumerable<GetBookingByHomeStay>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var bookings = _mapper.Map<IEnumerable<GetBookingByHomeStay>>(booking);
            if (bookings == null || !bookings.Any())
            {
                return new BaseResponse<IEnumerable<GetBookingByHomeStay>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetBookingByHomeStay>>("Get all bookings as base success",
                StatusCodeEnum.OK_200, bookings);
        }

        public async Task<BaseResponse<IEnumerable<GetBookingByAccount>>> GetBookingsByAccountId(string accountId)
        {
            IEnumerable<Booking> booking = await _bookingRepository.GetBookingsByAccountId(accountId);
            if (booking == null || !booking.Any())
            {
                return new BaseResponse<IEnumerable<GetBookingByAccount>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var bookings = _mapper.Map<IEnumerable<GetBookingByAccount>>(booking);
            if (bookings == null || !bookings.Any())
            {
                return new BaseResponse<IEnumerable<GetBookingByAccount>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetBookingByAccount>>("Get all bookings as base success",
                StatusCodeEnum.OK_200, bookings);
        }

        public async Task<BaseResponse<IEnumerable<GetBookingByRoom>>> GetBookingsByRoom(int roomId, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (roomId <= 0)
            {
                return new BaseResponse<IEnumerable<GetBookingByRoom>>("Invalid Room ID", StatusCodeEnum.BadRequest_400, null);
            }

            IEnumerable<Booking> bookings = await _bookingRepository.GetBookingsByRoom(roomId, startDate, endDate);

            // Ánh xạ bookings sang GetBookingByRoom, kể cả khi bookings rỗng
            var bookingResponses = _mapper.Map<IEnumerable<GetBookingByRoom>>(bookings ?? new List<Booking>(), opt =>
            {
                opt.Items["roomId"] = roomId;
                opt.Items["startDate"] = startDate;
                opt.Items["endDate"] = endDate;
            });

            // Luôn trả về 200 OK, với data là mảng rỗng nếu không có booking
            return new BaseResponse<IEnumerable<GetBookingByRoom>>(
                bookings != null && bookings.Any() ? "Get bookings by room success" : "No bookings found for this room in the specified date range",
                StatusCodeEnum.OK_200,
                bookingResponses ?? new List<GetBookingByRoom>()
            );
        }
        public async Task<BaseResponse<GetBookingResponse>> GetBookingById(int? bookingID)
        {
            Booking booking = await _bookingRepository.GetBookingsByIdAsync(bookingID);
            var result = _mapper.Map<GetBookingResponse>(booking);
            return new BaseResponse<GetBookingResponse>("Get HomeStay as base success", StatusCodeEnum.OK_200, result);
        }

        public (int? bookingId, int? serviceId, string? accountId) ParseOrderInfo(string orderInfo)
        {
            int? bookingId = null;
            int? serviceId = null;
            string? accountId = null;

            if (string.IsNullOrWhiteSpace(orderInfo))
                return (null, null, null);

            var parts = orderInfo.Split(',', StringSplitOptions.TrimEntries);

            foreach (var part in parts)
            {
                var keyValue = part.Split(':', StringSplitOptions.TrimEntries);
                if (keyValue.Length == 2)
                {
                    switch (keyValue[0])
                    {
                        case "BookingID":
                            if (int.TryParse(keyValue[1], out int bId))
                                bookingId = bId;
                            break;

                        case "ServiceID":
                            if (int.TryParse(keyValue[1], out int sId))
                                serviceId = sId;
                            break;

                        case "AccountID":
                            accountId = keyValue[1];
                            break;
                    }
                }
            }
            return (bookingId, serviceId, accountId);
        }

        public async Task<BaseResponse<List<GetTotalBookingsTotalBookingsAmountForHomeStay>>> GetTotalBookingsTotalBookingsAmountForHomeStay(int homeStayID, DateTime startDate, DateTime endDate, string? timeSpanType)
        {
            if(homeStayID <= 0)
            {
                return new BaseResponse<List<GetTotalBookingsTotalBookingsAmountForHomeStay>>("Please input correct ID", StatusCodeEnum.NotImplemented_501, null);
            }

            if (startDate == default(DateTime).Date || endDate == default(DateTime).Date)
            {
                return new BaseResponse<List<GetTotalBookingsTotalBookingsAmountForHomeStay>>("Please input time", StatusCodeEnum.NotImplemented_501, null);
            }

            if (startDate >= endDate)
            {
                return new BaseResponse<List<GetTotalBookingsTotalBookingsAmountForHomeStay>>("Please input endDate > startDate", StatusCodeEnum.NotAcceptable_406, null);
            }

            var total = await _bookingRepository.GetTotalBookingsTotalBookingsAmountForHomeStay(homeStayID ,startDate, endDate, timeSpanType);
            var response = total.Select(p => new GetTotalBookingsTotalBookingsAmountForHomeStay
            {
                span = p.span,
                totalBookings = p.totalBookings,
                totalBookingsAmount = p.totalBookingsAmount
            }).ToList();
            if (response == null || !response.Any())
            {
                return new BaseResponse<List<GetTotalBookingsTotalBookingsAmountForHomeStay>>("Get Total Fail", StatusCodeEnum.BadRequest_400, null);
            }
            return new BaseResponse<List<GetTotalBookingsTotalBookingsAmountForHomeStay>>("Get All Success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<List<GetTopLoyalCustomers>>> GetTopLoyalCustomers(int homeStayId, int top = 5)
        {
            if (homeStayId <= 0)
            {
                return new BaseResponse<List<GetTopLoyalCustomers>>("Please input correct ID", StatusCodeEnum.NotImplemented_501, null);
            }
            var total = await _bookingRepository.GetTopLoyalCustomersAsync(homeStayId, top);
            var response = total.Select(p => new GetTopLoyalCustomers
            {
                accountID = p.accountID,
                CustomerName = p.CustomerName,
                totalBookings = p.BookingCount
            }).ToList();
            if (response == null || !response.Any())
            {
                return new BaseResponse<List<GetTopLoyalCustomers>>("Get Total Fail", StatusCodeEnum.BadRequest_400, null);
            }
            return new BaseResponse<List<GetTopLoyalCustomers>>("Get All Success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<List<GetCustomerUser>>> GetCustomersByHomeStay(int homeStayId)
        {
            var accounts = await _bookingRepository.GetCustomersByHomeStay(homeStayId);

            var result = accounts.Select(a => new GetCustomerUser
            {
                AccountID = a.Account.Id,
                Email = a.Account.Email,
                Name = a.Account.Name,
                Phone = a.Account.Phone,
                Address = a.Account.Address,
                TotalBooking = a.TotalBooking
            }).ToList();

            return new BaseResponse<List<GetCustomerUser>>("Get All Success", StatusCodeEnum.OK_200, result);
        }

        public async Task<BaseResponse<List<GetCurrentWeekRevenueForHomeStay>>> GetCurrentWeekRevenueForHomeStay(int homestayId)
        {
            var revenueData = await _bookingRepository.GetCurrentWeekRevenueForHomeStay(homestayId);

            // Chuyển đổi dữ liệu từ tuple (string, double) thành đối tượng GetCurrentWeekRevenueForHomeStay
            var result = revenueData.Select(d => new GetCurrentWeekRevenueForHomeStay
            {
                Date = d.date,
                totalBookingsAmount = d.totalBookingsAmount
            }).ToList();

            return new BaseResponse<List<GetCurrentWeekRevenueForHomeStay>>("Get All Success", StatusCodeEnum.OK_200, result);
        }

        public async Task<BaseResponse<int>> GetBookingByAccountAndHomeStayAsync(string accountId, int homeStayId)
        {
            try
            {
                if (string.IsNullOrEmpty(accountId) || homeStayId <= 0)
                {
                    return new BaseResponse<int>(
                        "Please provide a valid Account ID and HomeStay ID.",
                        StatusCodeEnum.BadRequest_400,
                        0);
                }

                var booking = await _bookingRepository.GetBookingByAccountAndHomeStayAsync(accountId, homeStayId);
                if (booking == null)
                {
                    return new BaseResponse<int>(
                        "No valid booking found for this AccountID and HomeStayID!",
                        StatusCodeEnum.NotFound_404,
                        0);
                }

                return new BaseResponse<int>(
                    "Booking found and eligible for rating.",
                    StatusCodeEnum.OK_200,
                    booking.BookingID);
            }
            catch (Exception ex)
            {
                return new BaseResponse<int>(
                    $"Something went wrong! Error: {ex.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    0);
            }

        }
        public async Task<BaseResponse<int>> CancelExpiredBookings()
        {
            var expiredBookings = await _bookingRepository.GetExpiredBookings();
            if (expiredBookings == null || !expiredBookings.Any())
            {
                throw new InvalidOperationException("There are no Bookings for cancellation!");
            }

            int cancelledCount = 0;

            foreach (var booking in expiredBookings)
            {
                booking.Status = BookingStatus.Cancelled;

                if (booking.paymentStatus != PaymentStatus.FullyPaid && booking.paymentStatus != PaymentStatus.Deposited)
                {
                    booking.paymentStatus = PaymentStatus.Pending;
                }

                if (booking.BookingServices != null && booking.BookingServices.Any())
                {
                    foreach (var service in booking.BookingServices)
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
                                if (serviceDetail?.Services != null && booking.HomeStayID.HasValue)
                                {
                                    var cancellationPolicy = await _cancelltaionRepository.GetCancellationPolicyByHomeStayAsync(booking.HomeStayID.Value);
                                    var bookingDetail = booking.BookingDetails.FirstOrDefault();

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
                                                serviceTransaction.FinishDate = DateTime.Now;
                                                await _transactionRepository.UpdateAsync(serviceTransaction);
                                            }
                                        }
                                    }
                                }
                            }
                            service.Status = BookingServicesStatus.Cancelled;
                            await _bookingServiceRepository.UpdateBookingServicesAsync(service);
                        }
                    }
                }

                if (booking.paymentStatus == PaymentStatus.FullyPaid || booking.paymentStatus == PaymentStatus.Deposited)
                {
                    var transaction = await _transactionRepository.GetTransactionByBookingId(booking.BookingID);
                    if (transaction != null && transaction.StatusTransaction == StatusOfTransaction.Pending)
                    {
                        /*var commissionRate = await _commissionRateRepository.GetCommissionByHomeStayAsync(booking.HomeStayID.Value);
                        if (commissionRate != null)
                        {

                            // Chia tiền tùy theo loại giao dịch
                            if (transaction.TransactionKind == TransactionKind.FullPayment || transaction.TransactionKind == TransactionKind.Deposited)
                            {
                                transaction.OwnerAmount = transaction.Amount * commissionRate.HostShare;
                                transaction.AdminAmount = transaction.Amount * commissionRate.PlatformShare;
                            }

                            await _transactionRepository.UpdateAsync(transaction);
                        }
                        else
                        {
                            throw new InvalidOperationException("There are no CommissionRate for Bookings!");
                        }*/

                        transaction.StatusTransaction = StatusOfTransaction.Completed;
                        transaction.FinishDate = DateTime.Now;
                        await _transactionRepository.UpdateAsync(transaction);
                    }
                }

                await _bookingRepository.UpdateBookingAsync(booking);
                cancelledCount++;
            }
            return new BaseResponse<int>("The booking has been automatically cancelled successfully!", StatusCodeEnum.Created_201, cancelledCount);
        }

        public async Task<BaseResponse<string>> AutoCheckOutBookings()
        {
            var checkoutBookings= await _bookingRepository.GetCheckOutBookings();
            if (checkoutBookings == null || !checkoutBookings.Any())
            {
                throw new InvalidOperationException("There are no Bookings for checking-out!");
            }

            foreach(var booking in checkoutBookings)
            {
                booking.Status = BookingStatus.Completed;

                var guestNotification = new Notification
                {
                    AccountID = booking.AccountID, // Account của khách
                    Title = "Check-out Successful",
                    Message = $"Your booking (ID: {booking.BookingID}) has been successfully checked out.",
                    BookingID = booking.BookingID,
                    TypeNotify = NotificationType.Reminder
                };

                // Thêm thông báo cho khách vào danh sách Notifications của Booking
                booking.Notifications.Add(guestNotification);

                Notification hostNotification = null;
                if (booking.HomeStayID.HasValue)
                {
                    hostNotification = new Notification
                    {
                        AccountID = booking.HomeStay?.AccountID, // Account của chủ homestay
                        Title = "Booking Check-out Notification",
                        Message = $"A guest has checked out from your homestay. Booking ID: {booking.BookingID}.",
                        BookingID = booking.BookingID,
                        TypeNotify = NotificationType.Reminder
                    };

                    // Thêm thông báo cho chủ homestay vào danh sách Notifications của Booking
                    booking.Notifications.Add(hostNotification);
                }

                var transaction = await _transactionRepository.GetTransactionByBookingId(booking.BookingID);
                if (transaction != null && transaction.StatusTransaction == StatusOfTransaction.Pending)
                {
                    /*var commissionRate = await _commissionRateRepository.GetCommissionByHomeStayAsync(booking.HomeStayID.Value);
                    if (commissionRate != null)
                    {
                        var hostRate = commissionRate.HostShare;
                        var adminRate = commissionRate.PlatformShare;

                        // Áp dụng theo loại giao dịch
                        if (transaction.TransactionKind == TransactionKind.Deposited || transaction.TransactionKind == TransactionKind.FullPayment)
                        {
                            transaction.OwnerAmount = transaction.Amount * hostRate;
                            transaction.AdminAmount = transaction.Amount * adminRate;
                        }
                        else if (transaction.TransactionKind == TransactionKind.Refund)
                        {
                            transaction.OwnerAmount = transaction.Amount;
                            transaction.AdminAmount = 0;
                        }

                        await _transactionRepository.UpdateAsync(transaction);
                    }*/
                    transaction.StatusTransaction = StatusOfTransaction.Completed;
                    transaction.FinishDate = DateTime.Now;
                    await _transactionRepository.UpdateAsync(transaction);
                }
                else
                {
                    throw new InvalidOperationException("The booking does not have a transaction!");
                }

                if(booking.BookingServices != null && booking.BookingServices.Any())
                {
                    foreach (var service in booking.BookingServices)
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
                                if (serviceDetail?.Services != null && booking.HomeStayID.HasValue)
                                {
                                    serviceDetail.Services.Quantity += serviceDetail.Quantity;
                                    await _serviceRepository.UpdateAsync(serviceDetail.Services);
                                }
                            }
                            service.Status = BookingServicesStatus.Completed;
                            await _bookingServiceRepository.UpdateBookingServicesAsync(service);
                        }
                        var serviceTransaction = await _transactionRepository.GetTransactionByBookingServiceId(service.BookingServicesID);

                        if (serviceTransaction != null && serviceTransaction.StatusTransaction == StatusOfTransaction.Pending)
                        {
                            serviceTransaction.StatusTransaction = StatusOfTransaction.Completed;
                            serviceTransaction.FinishDate = DateTime.Now;
                            await _transactionRepository.UpdateAsync(serviceTransaction);
                        }
                    }
                }
                    await _bookingRepository.UpdateBookingAsync(booking);

                await _notificationHub.Clients.User(booking.AccountID).SendAsync(
                    "ReceiveNotification",
                    guestNotification.AccountID,
                    guestNotification.Title,
                    guestNotification.Message,
                    guestNotification.NotificationID,
                    guestNotification.CreateAt,
                    guestNotification.IsRead,
                    guestNotification.TypeNotify
                );

                if (hostNotification != null)
                {
                    await _notificationHub.Clients.User(booking.HomeStay.AccountID).SendAsync(
                        "ReceiveNotification",
                        hostNotification.AccountID,
                        hostNotification.Title,
                        hostNotification.Message,
                        hostNotification.NotificationID,
                        hostNotification.CreateAt,
                        hostNotification.IsRead,
                        hostNotification.TypeNotify
                    );
                }
            }
            return new BaseResponse<string>("The booking has been automatically Checkout successfully!", StatusCodeEnum.Created_201, "Success");
        }

        public async Task<BaseResponse<GetTotalBookingsAndAmount>> GetTotalBookingsAndAmount()
        {
            var total = await _bookingRepository.GetTotalBookingsAndAmount();
            var response = new GetTotalBookingsAndAmount
            {

                totalBookings = total.totalBookings,
                totalBookingsAmount = total.totalBookingsAmount
            };
            if (response == null)
            {
                return new BaseResponse<GetTotalBookingsAndAmount>("Get Total Fail", StatusCodeEnum.BadRequest_400, null);
            }
            return new BaseResponse<GetTotalBookingsAndAmount>("Get All Success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<GetTotalBookingsAndAmountForHomeStay>> GetTotalBookingsAndAmountForHomeStay(int homeStayID)
        {
            if (homeStayID <= 0)
            {
                return new BaseResponse<GetTotalBookingsAndAmountForHomeStay>("Please input correct ID", StatusCodeEnum.NotImplemented_501, null);
            }

            var total = await _bookingRepository.GetTotalBookingsAndAmountForHomeStay(homeStayID);
            var response =  new GetTotalBookingsAndAmountForHomeStay
            {
                
                totalBookings = total.totalBookings,
                totalBookingsAmount =total.totalBookingsAmount
            };
            if (response == null)
            {
                return new BaseResponse<GetTotalBookingsAndAmountForHomeStay>("Get Total Fail", StatusCodeEnum.BadRequest_400, null);
            }
            return new BaseResponse<GetTotalBookingsAndAmountForHomeStay>("Get All Success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<GetStaticBookingsForHomeStay>> GetStaticBookingsForHomeStay(int homestayId)
        {
            var booking = await _bookingRepository.GetStaticBookingsForHomeStay(homestayId);
            var response = new GetStaticBookingsForHomeStay
            {
                bookings = booking.bookings,
                bookingsReturnOrCancell = booking.bookingsReturnOrCancell,
                bookingsCancell = booking.bookingsCancell,
                bookingsComplete = booking.bookingsComplete,
                bookingsReport = booking.bookingsReport,
                bookingsReturnRefund = booking.bookingsReturnRefund,
                bookingConfirmed = booking.bookingConfirmed
            };
            if (response == null)
            {
                return new BaseResponse<GetStaticBookingsForHomeStay>("Get All Fail", StatusCodeEnum.BadGateway_502, response);
            }
            return new BaseResponse<GetStaticBookingsForHomeStay>("Get All Success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<List<GetStaticBookingsForAllHomestays>>> GetStaticBookingsForAllHomestays()
        {
            try
            {
                var staticBookings = await _bookingRepository.GetStaticBookingsForAllHomestays();

                var result = staticBookings
                    .Select(item => new GetStaticBookingsForAllHomestays
                    {
                        HomeStays = _mapper.Map<SingleHomeStayResponse>(item.homeStay),
                        bookings = item.bookings,
                        bookingsReturnOrCancell = item.bookingsReturnOrCancell,
                        bookingsCancell = item.bookingsCancell,
                        bookingsComplete = item.bookingsComplete,
                        bookingsReport = item.bookingsReport,
                        bookingsReturnRefund = item.bookingsReturnRefund,
                        bookingConfirmed = item.bookingConfirmed
                    })
                    .ToList();

                return new BaseResponse<List<GetStaticBookingsForAllHomestays>>("Get All Success", StatusCodeEnum.OK_200, result);
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<GetStaticBookingsForAllHomestays>>($"Đã xảy ra lỗi: {ex.Message}", StatusCodeEnum.BadGateway_502, null);
            }
        }
    }
}
