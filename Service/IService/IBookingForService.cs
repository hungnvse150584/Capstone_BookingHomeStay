﻿using BusinessObject.Model;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.BookingServices;
using Service.RequestAndResponse.Response.BookingOfServices;
using Service.RequestAndResponse.Response.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IBookingForService
    {
        Task<BaseResponse<BookingServices>> CreateServiceBooking(CreateBookingServices bookingServiceRequest, PaymentServicesMethod paymentServicesMethod);
        Task<BaseResponse<UpdateBookingService>> UpdateBookingServices(int bookingServiceID, UpdateBookingService request);
        Task<BaseResponse<IEnumerable<GetAllBookingServices>>> GetAllBookingService(string? search, DateTime? date = null, BookingServicesStatus? status = null, PaymentServicesStatus? paymentStatus = null);
        Task<BaseResponse<BookingServices>> GetBookingServicesById(int bookingID);
        Task<BaseResponse<BookingServices>> GetBookingServiceById(int? bookingID);
        Task<BaseResponse<IEnumerable<GetBookingServiceByAccount>>> GetBookingServiceByAccountId(string accountId);
        Task<BaseResponse<IEnumerable<GetBookingServiceByHomeStay>>> GetBookingServicesByHomeStayId(int homeStayID);
        Task<BaseResponse<List<GetServiceStats>>> GetServiceUsageStatsAsync(int homestayId);
    }
}
