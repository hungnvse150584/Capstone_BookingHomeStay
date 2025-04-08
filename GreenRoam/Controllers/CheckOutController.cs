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
        public CheckOutController(ICheckOutService checkoutService,
            IBookingService bookingService,
            IBookingForService bookingForService,
            IVnPayService vpnPayService,
            IConfiguration configuration,
            ICancellationPolicyService cancellationService)
        {
            _checkoutService = checkoutService;
            _bookingService = bookingService;
            _bookingForService = bookingForService;
            _vpnPayService = vpnPayService;
            _configuration = configuration;
            _cancellationService = cancellationService;
        }
        [HttpPost]
        [Route("CreateBooking")]
        public async Task<ActionResult<BaseResponse<string>>> CreateBooking([FromBody] CreateBookingRequest bookingRequest, PaymentMethod paymentMethod)
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
        public async Task<string> CheckOutBooking(int bookingID, bool isFullPayment)
        {
            var booking = await _bookingService.GetBookingsById(bookingID);
            var bookingId = booking.Data.BookingID;
            double amount = isFullPayment ? booking.Data.Total : booking.Data.bookingDeposit;
            var bookingServiceID = booking.Data.BookingServices.SingleOrDefault()?.BookingServicesID;

            var vnPayModel = new VnPayRequestModel
            {
                BookingID = booking.Data.BookingID,
                BookingServiceID = bookingServiceID.HasValue ? bookingServiceID : null,
                Amount = amount,
                FullName = booking.Data.Account.Name,
                Description = $"{booking.Data.Account.Name} {booking.Data.Account.Phone}",
                CreatedDate = DateTime.UtcNow,
            };
            return _vpnPayService.CreatePaymentUrl(HttpContext, vnPayModel);
        }

        [HttpPost]
        [Route("BookingPayment-Refund")]
        public async Task<ActionResult<string>> CheckOutRefundBooking(int bookingID)
        {
            var booking = await _bookingService.GetBookingsById(bookingID);

            var bookingId = booking.Data.BookingID;

            var cancellation = await _cancellationService.GetCancellationPolicyByHomeStay(booking.Data.HomeStayID);

            var checkInDate = booking.Data.BookingDetails.FirstOrDefault()?.CheckInDate;

            double amount = 0;

            DateTime today = DateTime.UtcNow.Date;

            int daysUntilCheckIn = (checkInDate.Value.Date - today).Days;

            if (daysUntilCheckIn >= cancellation.Data.DayBeforeCancel)
            {
                if(booking.Data.paymentStatus == PaymentStatus.Deposited)
                {
                    amount = booking.Data.bookingDeposit;
                }
                if (booking.Data.paymentStatus == PaymentStatus.FullyPaid)
                {
                    amount = booking.Data.Total * cancellation.Data.RefundPercentage;
                }
            }
            else
            {
                return BadRequest("Booking cannot be refunded because it does not meet the homestay's cancellation policy.");
            }

            var bookingServiceID = booking.Data.BookingServices.SingleOrDefault()?.BookingServicesID;

            var vnPayModel = new VnPayRequestModel
            {
                BookingID = booking.Data.BookingID,
                BookingServiceID = bookingServiceID.HasValue ? bookingServiceID : null,
                Amount = amount,
                FullName = booking.Data.Account.Name,
                Description = $"{booking.Data.Account.Name} {booking.Data.Account.Phone}",
                CreatedDate = DateTime.UtcNow,
            };
            return _vpnPayService.CreatePaymentUrl(HttpContext, vnPayModel);
        }

        [HttpGet("vnpay-return")]
        public async Task<IActionResult> HandleVnPayReturn([FromQuery] VnPayReturnModel model)
        {
            if (model.Vnp_TransactionStatus != "00") return BadRequest();

            var (bookingId, bookingserviceId) = _bookingService.ParseOrderInfo(model.Vnp_OrderInfo);

            if (bookingId.HasValue)
            {
                var booking = await _bookingService.GetBookingsById(bookingId.Value);
                if (booking == null)
                    return BadRequest($"Booking with ID {bookingId} not found.");
            }

            if (bookingserviceId.HasValue)
            {
                var bookingService = await _bookingForService.GetBookingServicesById(bookingserviceId.Value);
                if (bookingService == null)
                    return BadRequest($"Booking with ID {bookingserviceId} not found.");
            }

            var transaction = new Transaction
            {
                Amount = model.Vnp_Amount,
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

            await _checkoutService.CreateBookingPayment(bookingId, bookingserviceId, transaction);
            return Redirect($"{_configuration["VnPay:UrlReturnPayment"]}/{bookingId}");
        }

        [HttpGet("vnpay-return-refunded")]
        public async Task<IActionResult> HandleVnPayReturnRefund([FromQuery] VnPayReturnModel model)
        {
            if (model.Vnp_TransactionStatus != "00") return BadRequest();

            var (bookingId, bookingserviceId) = _bookingService.ParseOrderInfo(model.Vnp_OrderInfo);

            if (bookingId.HasValue)
            {
                var booking = await _bookingService.GetBookingsById(bookingId.Value);
                if (booking == null)
                    return BadRequest($"Booking with ID {bookingId} not found.");
            }

            if (bookingserviceId.HasValue)
            {
                var bookingService = await _bookingForService.GetBookingServicesById(bookingserviceId.Value);
                if (bookingService == null)
                    return BadRequest($"Booking with ID {bookingserviceId} not found.");
            }

            var transaction = new Transaction
            {
                Amount = model.Vnp_Amount,
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

            await _checkoutService.CreateBookingRefundPayment(bookingId, bookingserviceId, transaction);
            return Redirect($"{_configuration["VnPay:UrlReturnPayment"]}/{bookingId}");
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
        [Route("ChangeBookingStatus")]
        public async Task<ActionResult<BaseResponse<Booking>>> ChangeTheBookingStatus(int bookingId, int? bookingServiceID, BookingStatus status, PaymentStatus paymentStatus, BookingServicesStatus servicesStatus, PaymentServicesStatus statusPayment)
        {
            var booking = await _checkoutService.ChangeBookingStatus(bookingId, bookingServiceID, status, paymentStatus, servicesStatus, statusPayment);
            return Ok(booking);
        }
    }
}
