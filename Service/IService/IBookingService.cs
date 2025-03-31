using BusinessObject.Model;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Booking;
using Service.RequestAndResponse.Request.BookingServices;
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
        Task<BaseResponse<Booking>> CreateBooking(CreateBookingRequest createBookingRequest, PaymentMethod paymentMethod);
        Task<BaseResponse<Booking>> ChangeBookingStatus(int bookingId, int? bookingServiceID, BookingStatus status, PaymentStatus paymentStatus, BookingServicesStatus servicesStatus, PaymentServicesStatus statusPayment);
        Task<BaseResponse<BookingServices>> CreateServiceBooking(CreateBookingServices bookingServiceRequest, PaymentServicesMethod paymentServicesMethod);
        Task<BaseResponse<IEnumerable<GetAllBookings>>> GetAllBooking(string? search, DateTime? date = null, BookingStatus? status = null, PaymentStatus? paymentStatus = null);
        Task<BaseResponse<IEnumerable<GetAllBookingServices>>> GetAllBookingService(string? search, DateTime? date = null, BookingServicesStatus? status = null, PaymentServicesStatus? paymentStatus = null);
        Task<BaseResponse<UpdateBookingRequest>> UpdateBooking(int bookingID, UpdateBookingRequest request);
        Task<BaseResponse<UpdateBookingService>> UpdateBookingServices(int bookingServiceID, UpdateBookingService request);
        Task<BaseResponse<IEnumerable<GetAllBookings>>> GetBookingsByHomeStayId(int homeStayID);
        Task<BaseResponse<IEnumerable<GetAllBookings>>> GetBookingsByAccountId(string accountId);
        Task<BaseResponse<GetCancellationBooking>> GetCancellationBooking(int bookingID);
        //For Admin DashBoard
        Task<BaseResponse<GetStaticBookings>> GetStaticBookings();
        Task<BaseResponse<GetTopHomeStayBookingInMonth>> GetTopHomeStayBookingInMonth();
        Task<BaseResponse<List<GetTotalBookingsTotalBookingsAmount>>> GetTotalBookingsTotalBookingsAmount
        (DateTime startDate, DateTime endDate, string? timeSpanType);
    }
}
