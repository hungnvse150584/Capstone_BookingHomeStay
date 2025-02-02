using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Booking;
using Service.RequestAndResponse.Request.BookingServices;
using Service.RequestAndResponse.Request.District;
using Service.RequestAndResponse.Response.Bookings;
using Service.Service;

namespace GreenRoam.Controllers
{
    [Route("api/booking-bookingservices")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        [Route("GetAllBooking")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllBookings>>>> GetAllBookings(string? search, DateTime? date = null, BookingStatus? status = null)
        {
            var bookings = await _bookingService.GetAllBooking(search, date, status);
            return Ok(bookings);
        }


        [HttpGet("adminDashBoard/GetStaticBookings")]
        public async Task<BaseResponse<GetStaticBookings>> GetStaticBookings()
        {
            return await _bookingService.GetStaticBookings();
        }

        [HttpGet("adminDashBoard/GetTopHomeStayBookingInMonth")]
        public async Task<BaseResponse<GetTopHomeStayBookingInMonth>> GetTopHomeStayBookingInMonth()
        {
            return await _bookingService.GetTopHomeStayBookingInMonth();
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
        [Route("ChangeBookingStatus")]
        public async Task<ActionResult<BaseResponse<Booking>>> ChangeTheBookingStatus(int bookingId, int? bookingServiceID, BookingStatus status, BookingServicesStatus servicesStatus)
        {
            var booking = await _bookingService.ChangeBookingStatus(bookingId, bookingServiceID, status, servicesStatus);
            return Ok(booking);
        }

        [HttpPost]
        [Route("CreateBookingServices")]
        public async Task<ActionResult<BaseResponse<BookingServices>>> CreateBookingServices([FromBody] CreateBookingServices bookingServiceRequest, PaymentServicesMethod paymentServicesMethod)
        {

            if (bookingServiceRequest == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var booking = await _bookingService.CreateServiceBooking(bookingServiceRequest, paymentServicesMethod);
            return booking;
        }


    }
}
