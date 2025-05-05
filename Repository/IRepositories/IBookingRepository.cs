using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllBookingAsync(string? search, DateTime? date = null, BookingStatus? status = null, PaymentStatus? paymentStatus = null);
        Task<IEnumerable<Booking>> GetBookingsByAccountId(string accountId);
        Task<IEnumerable<Booking>> GetExpiredBookings();
        Task<IEnumerable<Booking>> GetCheckOutBookings();
        Task<IEnumerable<Booking>> GetBookingsByHomeStayId(int homeStayID);
        Task<Booking?> GetBookingStatusByAccountId(string accountId);
        Task<Booking?> ChangeBookingStatus(int bookingId, BookingStatus status, PaymentStatus paymentStatus);
        Task AddBookingAsync(Booking booking);
        Task UpdateBookingAsync(Booking booking);
        Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date);
        Task<IEnumerable<Booking>> GetBookingsByStatusAsync(BookingStatus status);
        Task<Booking?> GetBookingByIdAsync(int bookingId);
        Task<Booking?> GetBookingsByIdAsync(int? bookingId);
        Task<Booking?> UpdateBookingWithReportAsync(int bookingId, Booking booking);
        Task<IEnumerable<Booking>> GetBookingsForCheckInReminderAsync();
        Task<List<(string date, double totalBookingsAmount)>> GetCurrentWeekRevenueForHomeStay(int homestayId);


        //For AdminDashBoard
        Task<(int bookingsReturnOrCancell, int bookings, int bookingsComplete, int bookingsCancell, int bookingsReturnRefund, int bookingsReport)> GetStaticBookings();
        Task<List<(string homeStayName, int QuantityOfBooking)>> GetTopHomeStayBookingInMonthAsync();
        Task<List<(object span, int totalBookings, double totalBookingsAmount)>> GetTotalBookingsTotalBookingsAmount
        (DateTime startDate, DateTime endDate, string? timeSpanType);
        Task<List<(object span, int totalBookings, double totalBookingsAmount)>> GetTotalBookingsTotalBookingsAmountForHomeStay
        (int homeStayID, DateTime startDate, DateTime endDate, string? timeSpanType);
        Task<List<(string accountID, string CustomerName, int BookingCount)>> GetTopLoyalCustomersAsync(int homeStayId, int top = 5);
        Task<List<(Account Account, int TotalBooking)>> GetCustomersByHomeStay(int homeStayId);
        Task<IEnumerable<Booking>> GetBookingsByRoom(int roomId);
        Task<Booking?> GetBookingByAccountAndHomeStayAsync(string accountId, int homeStayId);


    }
}
