using BusinessObject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.BookingServices;
using Service.RequestAndResponse.Response.BookingOfServices;
using Service.RequestAndResponse.Response.Bookings;
using Service.RequestAndResponse.Response.Services;

namespace GreenRoam.Controllers
{
    [Route("api/bookingservices")]
    [ApiController]
    public class BookingForServiceController : ControllerBase
    {
        private readonly IBookingForService _bookingService;
        public BookingForServiceController(IBookingForService bookingService)
        {
            _bookingService = bookingService;
        }

        [Authorize(Roles = "Admin, Owner, Staff")]
        [HttpGet]
        [Route("GetAllBookingServices")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllBookings>>>> GetAllBookingServices(string? search, DateTime? date = null, BookingServicesStatus? status = null, PaymentServicesStatus? paymentStatus = null)
        {
            var bookings = await _bookingService.GetAllBookingService(search, date, status, paymentStatus);
            return Ok(bookings);
        }

        [Authorize(Roles = "Customer, Owner, Staff")]
        [HttpGet]
        [Route("GetBookingServicesByAccountID/{accountId}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetBookingServiceByAccount>>>> GetBookingServiceByAccountId(string accountId)
        {
            var bookings = await _bookingService.GetBookingServiceByAccountId(accountId);
            return Ok(bookings);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpGet]
        [Route("GetBookingServicesByHomeStayID/{homeStayID}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetBookingServiceByHomeStay>>>> GetBookingServicesByHomeStayId(int homeStayID)
        {
            var bookings = await _bookingService.GetBookingServicesByHomeStayId(homeStayID);
            return Ok(bookings);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpGet]
        [Route("GetServiceStats")]
        public async Task<ActionResult<BaseResponse<List<GetServiceStats>>>> GetServiceUsageStats(int homestayId)
        {
            var services = await _bookingService.GetServiceUsageStatsAsync(homestayId);
            return Ok(services);
        }

        [Authorize(Roles = "Customer")]
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

        [Authorize(Roles = "Customer")]
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
