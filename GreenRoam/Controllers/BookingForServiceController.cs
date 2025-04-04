﻿using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.BookingServices;
using Service.RequestAndResponse.Response.Bookings;

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

        [HttpGet]
        [Route("GetAllBookingServices")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllBookings>>>> GetAllBookingServices(string? search, DateTime? date = null, BookingServicesStatus? status = null, PaymentServicesStatus? paymentStatus = null)
        {
            var bookings = await _bookingService.GetAllBookingService(search, date, status, paymentStatus);
            return Ok(bookings);
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
