using BusinessObject.Model;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Booking;
using Service.RequestAndResponse.Response.RoomType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface ICheckOutService
    {
        Task<BaseResponse<int>> CreateBooking(CreateBookingRequest createBookingRequest, PaymentMethod paymentMethod);
        Task<BaseResponse<Booking>> ChangeBookingStatus(int bookingId, int? bookingServiceID, BookingStatus status, PaymentStatus paymentStatus, BookingServicesStatus servicesStatus, PaymentServicesStatus statusPayment);
        Task<BaseResponse<UpdateBookingRequest>> UpdateBooking(int bookingID, UpdateBookingRequest request);
        Task<Booking> CreateBookingPayment(int? bookingID, int? bookingServiceID, Transaction transaction);
        Task<Booking> CreateBookingRefundPayment(int? bookingID, int? bookingServiceID, Transaction transaction);
        Task<BookingServices> CreateBookingServicePayment(int? bookingServiceID, Transaction transaction);
        Task<BookingServices> CreateBookingServiceRefundPayment(int? bookingServiceID, Transaction transaction);
        Task<BaseResponse<UpdateBookingForRoomRequest>> ChangeRoomForBooking(int bookingID, UpdateBookingForRoomRequest request);
        Task<BaseResponse<List<GetRoomTypeStats>>> GetRoomTypeUsageStatsAsync(int homestayId);
    }
}
