using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Booking;
using Service.RequestAndResponse.Request.VnPayModel;
using Service.Service;

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
        public CheckOutController(ICheckOutService checkoutService, IBookingService bookingService, IBookingForService bookingForService, IVnPayService vpnPayService)
        {
            _checkoutService = checkoutService;
            _bookingService = bookingService;
            _bookingForService = bookingForService;
            _vpnPayService = vpnPayService;
        }
        [HttpPost]
        [Route("CreateBooking")]
        public async Task<ActionResult<BaseResponse<Booking>>> CreateBooking([FromBody] CreateBookingRequest bookingRequest, PaymentMethod paymentMethod)
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
            double amount = isFullPayment ? booking.Data.Total : booking.Data.bookingDeposit;
            var bookingServiceID = booking.Data.BookingServices.FirstOrDefault()?.BookingServicesID;

            var vnPayModel = new VnPayRequestModel
            {
                BookingID = booking.Data.BookingID,
                BookingServiceID = bookingServiceID.HasValue ? bookingServiceID : null,
                Amount = amount,
                FullName = booking.Data.Account.Name,
                CreatedDate = DateTime.UtcNow,
            };
            return _vpnPayService.CreatePaymentUrl(HttpContext, vnPayModel);
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
