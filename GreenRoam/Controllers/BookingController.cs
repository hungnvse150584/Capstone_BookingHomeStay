﻿using BusinessObject.Model;
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
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllBookings>>>> GetAllBookings(string? search, DateTime? date = null, BookingStatus? status = null, PaymentStatus? paymentStatus = null)
        {
            var bookings = await _bookingService.GetAllBooking(search, date, status, paymentStatus);
            return Ok(bookings);
        }


        [HttpGet]
        [Route("GetBookingByHomeStay/{homeStayID}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetBookingByHomeStay>>>> GetBookingsByHomeStayId(int homeStayID)
        {
            var bookings = await _bookingService.GetBookingsByHomeStayId(homeStayID);
            return Ok(bookings);
        }

        [HttpGet]
        [Route("GetBookingByAccountID/{accountId}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetBookingByAccount>>>> GetBookingsByAccountId(string accountId)
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
    }
}
