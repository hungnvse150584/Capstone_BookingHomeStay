using BusinessObject.Model;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Booking;
using Service.RequestAndResponse.Request.BookingServices;
using Service.RequestAndResponse.Response.Bookings;
using Service.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [HttpGet]
        [Route("GetBookingByHomeStay/{homeStayID}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllBookings>>>> GetBookingsByHomeStayId(int homeStayID)
        {
            var bookings = await _bookingService.GetBookingsByHomeStayId(homeStayID);
            return Ok(bookings);
        }

        [HttpGet]
        [Route("GetBookingByAccountID/{accountId}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllBookings>>>> GetBookingsByAccountId(string accountId)
        {
            var bookings = await _bookingService.GetBookingsByAccountId(accountId);
            return Ok(bookings);
        }

        [HttpGet]
        [Route("GetCancellationBooking/{bookingID}")]
        public async Task<ActionResult<BaseResponse<GetCancellationBooking>>> GetCancellationBooking(int bookingID)
        {
            var bookings = await _bookingService.GetCancellationBooking(bookingID);
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

        [HttpGet("adminDashBoard/GetTotalBookingsTotalBookingsAmount")]
        public async Task<BaseResponse<List<GetTotalBookingsTotalBookingsAmount>>> GetTotalBookingsTotalBookingsAmount
           (DateTime startDate, DateTime endDate, string? timeSpanType)
        {
            return await _bookingService.GetTotalBookingsTotalBookingsAmount(startDate, endDate, timeSpanType);
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

        [HttpPut]
        [Route("UpdateBookingServices")]
        public async Task<ActionResult<BaseResponse<UpdateBookingService>>> UpdateBookingServices(int bookingServiceID, UpdateBookingService request)
        {
            if (request == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var bookingServices = await _bookingService.UpdateBookingServices(bookingServiceID, request);
            return bookingServices;
        }


    }
}
