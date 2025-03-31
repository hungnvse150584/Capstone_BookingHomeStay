using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Booking;

namespace GreenRoam.Controllers
{
    [Route("api/booking-checkout")]
    [ApiController]
    public class CheckOutController : ControllerBase
    {
        private readonly ICheckOutService _bookingService;
        public CheckOutController(ICheckOutService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        [Route("CreateBooking")]
        public async Task<ActionResult<BaseResponse<Booking>>> CreateBooking([FromBody] CreateBookingRequest bookingRequest, PaymentMethod paymentMethod)
        {
            if (bookingRequest == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var booking = await _bookingService.CreateBooking(bookingRequest, paymentMethod);
            return booking;
        }

        [HttpPut]
        [Route("UpdateBooking")]
        public async Task<ActionResult<BaseResponse<UpdateBookingRequest>>> UpdateBooking(int bookingID, UpdateBookingRequest request)
        {
            if (request == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var booking = await _bookingService.UpdateBooking(bookingID, request);
            return booking;
        }

        [HttpPut]
        [Route("ChangeBookingStatus")]
        public async Task<ActionResult<BaseResponse<Booking>>> ChangeTheBookingStatus(int bookingId, int? bookingServiceID, BookingStatus status, PaymentStatus paymentStatus, BookingServicesStatus servicesStatus, PaymentServicesStatus statusPayment)
        {
            var booking = await _bookingService.ChangeBookingStatus(bookingId, bookingServiceID, status, paymentStatus, servicesStatus, statusPayment);
            return Ok(booking);
        }
    }
}
