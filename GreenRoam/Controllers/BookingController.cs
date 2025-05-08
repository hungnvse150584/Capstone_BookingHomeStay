using BusinessObject.Model;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Booking;
using Service.RequestAndResponse.Request.BookingServices;
using Service.RequestAndResponse.Response.Accounts;
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

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetAllBooking")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllBookings>>>> GetAllBookings(string? search, DateTime? date = null, BookingStatus? status = null, PaymentStatus? paymentStatus = null)
        {
            var bookings = await _bookingService.GetAllBooking(search, date, status, paymentStatus);
            return Ok(bookings);
        }

        //[Authorize(Roles = "Admin, Owner, Staff")]
        [HttpGet]
        [Route("GetBookingByID/{bookingID}")]
        public async Task<ActionResult<BaseResponse<GetBookingResponse>>> GetBookingsById(int bookingID)
        {
            var booking = await _bookingService.GetBookingById(bookingID);
            return Ok(booking);
        }

        //[Authorize(Roles = "Admin, Owner, Staff")]
        [HttpGet]
        [Route("GetBookingByHomeStay/{homeStayID}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetBookingByHomeStay>>>> GetBookingsByHomeStayId(int homeStayID)
        {
            var bookings = await _bookingService.GetBookingsByHomeStayId(homeStayID);
            return Ok(bookings);
        }

        //[Authorize(Roles = "Admin, Owner, Staff, Customer")]
        [HttpGet]
        [Route("GetBookingByAccountID/{accountId}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetBookingByAccount>>>> GetBookingsByAccountId(string accountId)
        {
            var bookings = await _bookingService.GetBookingsByAccountId(accountId);
            return Ok(bookings);
        }

        //[Authorize(Roles = "Admin, Owner, Staff, Customer")]
        [HttpGet]
        [Route("GetBookingByRoomID/{roomId}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetBookingByRoom>>>> GetBookingsByRoom(int roomId)
        {
            var bookings = await _bookingService.GetBookingsByRoom(roomId);
            return Ok(bookings);
        }

        //[Authorize(Roles = "Owner, Staff")]
        [HttpGet]
        [Route("GetCancellationBooking/{bookingID}")]
        public async Task<ActionResult<BaseResponse<GetCancellationBooking>>> GetCancellationBooking(int bookingID)
        {
            var bookings = await _bookingService.GetCancellationBooking(bookingID);
            return Ok(bookings);
        }


        //[Authorize(Roles = "Admin")]
        [HttpGet("adminDashBoard/GetStaticBookings")]
        public async Task<BaseResponse<GetStaticBookings>> GetStaticBookings()
        {
            return await _bookingService.GetStaticBookings();
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("adminDashBoard/GetTopHomeStayBookingInMonth")]
        public async Task<BaseResponse<GetTopHomeStayBookingInMonth>> GetTopHomeStayBookingInMonth()
        {
            return await _bookingService.GetTopHomeStayBookingInMonth();
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("adminDashBoard/GetTotalBookingsTotalBookingsAmount")]
        public async Task<BaseResponse<List<GetTotalBookingsTotalBookingsAmount>>> GetTotalBookingsTotalBookingsAmount
           (DateTime startDate, DateTime endDate, string? timeSpanType)
        {
            return await _bookingService.GetTotalBookingsTotalBookingsAmount(startDate, endDate, timeSpanType);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("adminDashBoard/GetTotalBookingsTotalBookingsAmountForHomeStay")]
        public async Task<BaseResponse<List<GetTotalBookingsTotalBookingsAmountForHomeStay>>> GetTotalBookingsTotalBookingsAmountForHomeStay
            (int homeStayID, DateTime startDate, DateTime endDate, string? timeSpanType)
        {
            return await _bookingService.GetTotalBookingsTotalBookingsAmountForHomeStay(homeStayID, startDate, endDate, timeSpanType);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("adminDashBoard/GetTopLoyalCustomers")]
        public async Task<BaseResponse<List<GetTopLoyalCustomers>>> GetTopLoyalCustomers(int homeStayId, int top = 5)
        {
            return await _bookingService.GetTopLoyalCustomers(homeStayId, top);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("adminDashBoard/GetCustomersByHomeStay")]
        public async Task<BaseResponse<List<GetCustomerUser>>> GetCustomersByHomeStay(int homeStayId)
        {
            return await _bookingService.GetCustomersByHomeStay(homeStayId);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("adminDashBoard/GetCurrentWeekRevenueForHomeStay")]
        public async Task<BaseResponse<List<GetCurrentWeekRevenueForHomeStay>>> GetCurrentWeekRevenueForHomeStay(int homestayId)
        {
            return await _bookingService.GetCurrentWeekRevenueForHomeStay(homestayId);
        }

        //[Authorize(Roles = "Owner, Staff, Customer")]
        [HttpGet]
        [Route("CheckBookingForRating")]
        public async Task<ActionResult<BaseResponse<int>>> CheckBookingForRating(string accountId, int homeStayId)
        {
            var result = await _bookingService.GetBookingByAccountAndHomeStayAsync(accountId, homeStayId);
            return StatusCode((int)result.StatusCode, result);
        }

    }
}
