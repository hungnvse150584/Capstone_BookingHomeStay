﻿using BusinessObject.Model;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Booking;
using Service.RequestAndResponse.Request.BookingServices;
using Service.RequestAndResponse.Response.Accounts;
using Service.RequestAndResponse.Response.BookingOfServices;
using Service.RequestAndResponse.Response.Bookings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IBookingService
    {
        /*Task<BaseResponse<Booking>> CreateBooking(CreateBookingRequest createBookingRequest, PaymentMethod paymentMethod);
        Task<BaseResponse<Booking>> ChangeBookingStatus(int bookingId, int? bookingServiceID, BookingStatus status, PaymentStatus paymentStatus, BookingServicesStatus servicesStatus, PaymentServicesStatus statusPayment);*/
        
        Task<BaseResponse<IEnumerable<GetAllBookings>>> GetAllBooking(string? search, DateTime? date = null, BookingStatus? status = null, PaymentStatus? paymentStatus = null);

        /*Task<BaseResponse<UpdateBookingRequest>> UpdateBooking(int bookingID, UpdateBookingRequest request);*/

        Task<BaseResponse<int>> CancelExpiredBookings();
        Task<BaseResponse<string>> AutoCheckOutBookings();
        public (int? bookingId, int? serviceId, string? accountId) ParseOrderInfo(string orderInfo);
        Task<BaseResponse<IEnumerable<GetBookingByHomeStay>>> GetBookingsByHomeStayId(int homeStayID);
        Task<BaseResponse<IEnumerable<GetBookingByRoom>>> GetBookingsByRoom(int roomId, DateTime? startDate = null, DateTime? endDate = null);
        Task<BaseResponse<Booking>> GetBookingsById(int bookingID);
        Task<BaseResponse<GetBookingResponse>> GetBookingById(int? bookingID);
        Task<BaseResponse<IEnumerable<GetBookingByAccount>>> GetBookingsByAccountId(string accountId);
        Task<BaseResponse<GetCancellationBooking>> GetCancellationBooking(int bookingID);
        //For Admin DashBoard
        Task<BaseResponse<GetStaticBookings>> GetStaticBookings();
        Task<BaseResponse<GetTopHomeStayBookingInMonth>> GetTopHomeStayBookingInMonth();
        Task<BaseResponse<List<GetTotalBookingsTotalBookingsAmount>>> GetTotalBookingsTotalBookingsAmount
        (DateTime startDate, DateTime endDate, string? timeSpanType);
        Task<BaseResponse<List<GetTotalBookingsTotalBookingsAmountForHomeStay>>> GetTotalBookingsTotalBookingsAmountForHomeStay
        (int homeStayID, DateTime startDate, DateTime endDate, string? timeSpanType);
        Task<BaseResponse<List<GetTopLoyalCustomers>>> GetTopLoyalCustomers(int homeStayId, int top = 5);
        Task<BaseResponse<List<GetCustomerUser>>> GetCustomersByHomeStay(int homeStayId);
        Task<BaseResponse<List<GetCurrentWeekRevenueForHomeStay>>> GetCurrentWeekRevenueForHomeStay(int homestayId);
        Task<BaseResponse<int>> GetBookingByAccountAndHomeStayAsync(string accountId, int homeStayId);
        Task<BaseResponse<GetTotalBookingsAndAmount>> GetTotalBookingsAndAmount();
        Task<BaseResponse<GetTotalBookingsAndAmountForHomeStay>> GetTotalBookingsAndAmountForHomeStay(int homeStayID);
        Task<BaseResponse<GetStaticBookingsForHomeStay>> GetStaticBookingsForHomeStay(int homestayId);
        Task<BaseResponse<List<GetStaticBookingsForAllHomestays>>> GetStaticBookingsForAllHomestays();
    }
}
