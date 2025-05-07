using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Booking;
using Service.RequestAndResponse.Request.VnPayModel;
using Service.Service;
using System.Globalization;

namespace GreenRoam.Controllers
{
    [Route("api/booking-checkout")]
    [ApiController]
    public class CheckOutController : ControllerBase
    {
        private readonly ICheckOutService _checkoutService;
        private readonly IBookingService _bookingService;
        private readonly IBookingForService _bookingForService;
        private readonly IVnPayService _vpnPayService;
        private readonly IConfiguration _configuration;
        private readonly ICancellationPolicyService _cancellationService;
        private readonly IAccountService _accountService;
        public CheckOutController(ICheckOutService checkoutService,
            IBookingService bookingService,
            IBookingForService bookingForService,
            IVnPayService vpnPayService,
            IConfiguration configuration,
            ICancellationPolicyService cancellationService,
            IAccountService accountService)
        {
            _checkoutService = checkoutService;
            _bookingService = bookingService;
            _bookingForService = bookingForService;
            _vpnPayService = vpnPayService;
            _configuration = configuration;
            _cancellationService = cancellationService;
            _accountService = accountService;
        }
        [HttpPost]
        [Route("CreateBooking")]
        public async Task<ActionResult<BaseResponse<int>>> CreateBooking([FromBody] CreateBookingRequest bookingRequest, PaymentMethod paymentMethod)
        {
            if (bookingRequest == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var booking = await _checkoutService.CreateBooking(bookingRequest, paymentMethod);
            return booking;
        }

        [HttpPost]
        [Route("BookingPayment")]
        public async Task<ActionResult<string>> CheckOutBooking(int bookingID, bool isFullPayment)
        {
            var booking = await _bookingService.GetBookingsById(bookingID);
            if (booking.Data.Status == BookingStatus.Completed)
            {
                return BadRequest("This booking already completed, cannot have a payment");
            }

            if (booking.Data.Status == BookingStatus.Cancelled)
            {
                return BadRequest("This booking already cancelled, cannot have a payment");
            }

            if (booking.Data.Status == BookingStatus.InProgress)
            {
                return BadRequest("This booking already paid, cannot have a payment");
            }

            if (booking.Data.Status == BookingStatus.RequestRefund)
            {
                return BadRequest("This booking already paid, cannot have a payment");
            }
            double amount = isFullPayment ? booking.Data.Total : booking.Data.bookingDeposit;
            var bookingServiceID = booking.Data.BookingServices.SingleOrDefault()?.BookingServicesID;

            var vnPayModel = new VnPayRequestModel
            {
                BookingID = booking.Data.BookingID,
                BookingServiceID = bookingServiceID.HasValue ? bookingServiceID : null,
                HomeStayID = booking.Data.HomeStayID,
                AccountID = booking.Data.AccountID,
                Amount = amount,
                FullName = booking.Data.Account.Name,
                Description = $"{booking.Data.Account.Name} {booking.Data.Account.Phone}",
                CreatedDate = DateTime.UtcNow,
            };
            return _vpnPayService.CreatePaymentUrl(HttpContext, vnPayModel);
        }

        [HttpPost]
        [Route("BookingPayment-Refund")]
        public async Task<ActionResult<string>> CheckOutRefundBooking(int bookingID, string accountId)
        {
            var booking = await _bookingService.GetBookingsById(bookingID);

            if (booking.Data.Status == BookingStatus.RequestRefund)
            {
                var cancellation = await _cancellationService.GetCancellationPolicyByHomeStay(booking.Data.HomeStayID);

                if (cancellation == null)
                {
                    return BadRequest("Cannot Find the Cancellation Policy Of the HomeStay");
                }

                var checkInDate = booking.Data.BookingDetails.FirstOrDefault()?.CheckInDate;

                double amount = 0;

                if (booking.Data.paymentStatus == PaymentStatus.Deposited)
                {
                    amount = booking.Data.bookingDeposit;
                }
                if (booking.Data.paymentStatus == PaymentStatus.FullyPaid)
                {
                    amount = booking.Data.Total * cancellation.Data.RefundPercentage;
                }
                foreach (var bookingService in booking.Data.BookingServices)
                {
                    // Bỏ qua các service đã được thanh toán cùng booking
                    if (bookingService.isPaidWithBooking)
                        continue;

                    double serviceRefundAmount = 0;

                    if (bookingService.PaymentServiceStatus == PaymentServicesStatus.Deposited &&
                        booking.Data.paymentStatus == PaymentStatus.Deposited)
                    {
                        serviceRefundAmount = 0;
                    }
                    else if (bookingService.PaymentServiceStatus == PaymentServicesStatus.Deposited &&
                        booking.Data.paymentStatus == PaymentStatus.FullyPaid)
                    {
                        serviceRefundAmount = bookingService.bookingServiceDeposit;
                    }
                    else if (bookingService.PaymentServiceStatus == PaymentServicesStatus.FullyPaid
                        && booking.Data.paymentStatus == PaymentStatus.Deposited)
                    {
                        serviceRefundAmount = bookingService.Total * cancellation.Data.RefundPercentage;
                    }
                    else if (bookingService.PaymentServiceStatus == PaymentServicesStatus.FullyPaid
                        && booking.Data.paymentStatus == PaymentStatus.FullyPaid)
                    {
                        serviceRefundAmount = 0;
                    }

                    amount += serviceRefundAmount;
                }

                var bookingServiceID = booking.Data.BookingServices.FirstOrDefault()?.BookingServicesID;

                var vnPayModel = new VnPayRequestModel
                {
                    BookingID = booking.Data.BookingID,
                    BookingServiceID = bookingServiceID.HasValue ? bookingServiceID : null,
                    HomeStayID = booking.Data.HomeStayID,
                    AccountID = accountId,
                    Amount = amount,
                    FullName = booking.Data.Account.Name,
                    Description = $"{booking.Data.Account.Name} {booking.Data.Account.Phone}",
                    CreatedDate = DateTime.UtcNow,
                };
                return _vpnPayService.CreatePaymentUrlWeb(HttpContext, vnPayModel);
            }
            else
            {
                return BadRequest("Cannot Refunded");
            }
        }

        [HttpPost]
        [Route("BookingServicePayment")]
        public async Task<ActionResult<string>> CheckOutBookingService(int bookingServiceId, bool isFullPayment)
        {
            var bookingService = await _bookingForService.GetBookingServiceById(bookingServiceId);

            if (bookingService.Data.Status == BookingServicesStatus.Completed)
            {
                return BadRequest("This booking already completed, cannot have a payment");
            }

            if (bookingService.Data.Status == BookingServicesStatus.Cancelled)
            {
                return BadRequest("This booking already cancelled, cannot have a payment");
            }

            double amount = isFullPayment ? bookingService.Data.Total : bookingService.Data.bookingServiceDeposit;

            var vnPayModel = new VnPayRequestModel
            {
                BookingServiceID = bookingService.Data.BookingServicesID,
                HomeStayID = bookingService.Data.HomeStayID,
                AccountID = bookingService.Data.AccountID,
                Amount = amount,
                FullName = bookingService.Data.Account.Name,
                Description = $"{bookingService.Data.Account.Name} {bookingService.Data.Account.Phone}",
                CreatedDate = DateTime.UtcNow,
            };
            return _vpnPayService.CreatePaymentUrl(HttpContext, vnPayModel);
        }

        [HttpPost]
        [Route("BookingPaymentService-Refund")]
        public async Task<ActionResult<string>> CheckOutRefundBookingService(int bookingServiceID, string accountId)
        {
            var bookingService = await _bookingForService.GetBookingServiceById(bookingServiceID);

            if (bookingService == null || bookingService.Data == null)
            {
                return NotFound("Booking service not found.");
            }
            if (bookingService.Data.Status == BookingServicesStatus.RequestRefund)
            {
                var cancellation = await _cancellationService.GetCancellationPolicyByHomeStay(bookingService.Data.HomeStayID);

                if (cancellation == null)
                {
                    return BadRequest("Cannot Find the Cancellation Policy Of the HomeStay");
                }

                if (bookingService.Data.Booking?.BookingDetails == null || !bookingService.Data.Booking.BookingDetails.Any())
                {
                    return BadRequest("No booking details found.");
                }

                var checkInDate = bookingService.Data.Booking.BookingDetails.FirstOrDefault()?.CheckInDate;

                double amount = 0;

                if (bookingService.Data.PaymentServiceStatus == PaymentServicesStatus.Deposited)
                {
                    amount = bookingService.Data.bookingServiceDeposit;
                }
                if (bookingService.Data.PaymentServiceStatus == PaymentServicesStatus.FullyPaid)
                {
                    amount = bookingService.Data.Total * cancellation.Data.RefundPercentage;
                }

                var vnPayModel = new VnPayRequestModel
                {
                    BookingServiceID = bookingService.Data.BookingServicesID,
                    HomeStayID = bookingService.Data.HomeStayID,
                    AccountID = accountId,
                    Amount = amount,
                    FullName = bookingService.Data.Account.Name,
                    Description = $"{bookingService.Data.Account.Name} {bookingService.Data.Account.Phone}",
                    CreatedDate = DateTime.UtcNow,
                };

                return _vpnPayService.CreatePaymentUrlWeb(HttpContext, vnPayModel);
            }
            else
            {
                return BadRequest("Cannot Refunded");
            }
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> HandleVnPayReturn([FromQuery] VnPayReturnModel model)
        {
            if (model.Vnp_TransactionStatus != "00") return BadRequest();

            var (bookingId, bookingserviceId, accountId) = _bookingService.ParseOrderInfo(model.Vnp_OrderInfo);

            var transaction = new Transaction
            {
                Amount = model.Vnp_Amount/100,
                BankCode = model.Vnp_BankCode,
                BankTranNo = model.Vnp_BankTranNo,
                TransactionType = model.Vnp_CardType,
                OrderInfo = model.Vnp_OrderInfo,
                PayDate = DateTime.ParseExact((string)model.Vnp_PayDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                ResponseCode = model.Vnp_ResponseCode,
                TmnCode = model.Vnp_TmnCode,
                TransactionNo = model.Vnp_TransactionNo,
                TransactionStatus = model.Vnp_TransactionStatus,
                TxnRef = model.Vnp_TxnRef,
                SecureHash = model.Vnp_SecureHash,
                ResponseId = model.Vnp_TransactionNo,
                Message = model.Vnp_ResponseCode
            };

            if (bookingId.HasValue)
            {
                var booking = await _bookingService.GetBookingsById(bookingId.Value);
                if (booking == null)
                    return BadRequest($"Booking with ID {bookingId} not found.");

                await _checkoutService.CreateBookingPayment(bookingId, bookingserviceId, transaction);
                return Redirect($"{_configuration["VnPay:UrlReturnPayment"]}/{bookingId}");
            }

            if (bookingserviceId.HasValue)
            {
                var bookingService = await _bookingForService.GetBookingServicesById(bookingserviceId.Value);
                if (bookingService == null)
                    return BadRequest($"Booking with ID {bookingserviceId} not found.");
                await _checkoutService.CreateBookingServicePayment(bookingserviceId, transaction);
                return Redirect($"{_configuration["VnPay:UrlReturnPayment"]}/{bookingserviceId}");
            }
            return BadRequest("Cannot find Booking or Booking Service");
        }

        [HttpGet("vnpay-return-refunded")]
        public async Task<IActionResult> HandleVnPayReturnRefund([FromQuery] VnPayReturnModel model)
        {
            if (model.Vnp_TransactionStatus != "00") return BadRequest();

            var (bookingId, bookingserviceId, accountId) = _bookingService.ParseOrderInfo(model.Vnp_OrderInfo);

            var account = await _accountService.GetByStringId(accountId);

            var transaction = new Transaction
            {
                Account = account,
                Amount = model.Vnp_Amount/100,
                BankCode = model.Vnp_BankCode,
                BankTranNo = model.Vnp_BankTranNo,
                TransactionType = model.Vnp_CardType,
                OrderInfo = model.Vnp_OrderInfo,
                PayDate = DateTime.ParseExact((string)model.Vnp_PayDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                ResponseCode = model.Vnp_ResponseCode,
                TmnCode = model.Vnp_TmnCode,
                TransactionNo = model.Vnp_TransactionNo,
                TransactionStatus = model.Vnp_TransactionStatus,
                TxnRef = model.Vnp_TxnRef,
                SecureHash = model.Vnp_SecureHash,
                ResponseId = model.Vnp_TransactionNo,
                Message = model.Vnp_ResponseCode
            };

            if (bookingId.HasValue)
            {
                var booking = await _bookingService.GetBookingsById(bookingId.Value);
                if (booking == null)
                    return BadRequest($"Booking with ID {bookingId} not found.");

                await _checkoutService.CreateBookingRefundPayment(bookingId, bookingserviceId, transaction);
                return Redirect($"{_configuration["VnPay:PaymentBackReturnUrl2"]}/{bookingId}");
            }

            if (bookingserviceId.HasValue)
            {
                var bookingService = await _bookingForService.GetBookingServicesById(bookingserviceId.Value);
                if (bookingService == null)
                    return BadRequest($"Booking with ID {bookingserviceId} not found.");

                await _checkoutService.CreateBookingServiceRefundPayment(bookingserviceId, transaction);
                return Redirect($"{_configuration["VnPay:PaymentBackReturnUrl2"]}/{bookingserviceId}");
            }
            return BadRequest("Cannot find Booking or Booking Service");
        }

        [HttpPut]
        [Route("UpdateBooking")]
        public async Task<ActionResult<BaseResponse<UpdateBookingRequest>>> UpdateBooking(int bookingID, UpdateBookingRequest request)
        {
            if (request == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var booking = await _checkoutService.UpdateBooking(bookingID, request);
            return booking;
        }

        [HttpPut]
        [Route("ChangingRoom")]
        public async Task<ActionResult<BaseResponse<UpdateBookingForRoomRequest>>> ChangeRoomForBooking(int bookingID, UpdateBookingForRoomRequest request)
        {
            if (request == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var booking = await _checkoutService.ChangeRoomForBooking(bookingID, request);
            return booking;
        }

        [HttpPut]
        [Route("ChangeBookingStatus")]
        public async Task<ActionResult<BaseResponse<Booking>>> ChangeTheBookingStatus(int bookingId, int? bookingServiceID, BookingStatus status, PaymentStatus paymentStatus, BookingServicesStatus servicesStatus, PaymentServicesStatus statusPayment)
        {
            var booking = await _checkoutService.ChangeBookingStatus(bookingId, bookingServiceID, status, paymentStatus, servicesStatus, statusPayment);
            return Ok(booking);
        }
    }
}
