using BusinessObject.Model;
using DataAccessObject;
using Repository.BaseRepository;
using Repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        private readonly BookingDAO _bookingDao;
        public BookingRepository(BookingDAO bookingDao) : base(bookingDao)
        {
            _bookingDao = bookingDao;
        }

        public async Task AddBookingAsync(Booking booking)
        {
            await _bookingDao.AddBookingAsync(booking);
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            await _bookingDao.UpdateAsync(booking);
        }

        public async Task<Booking?> ChangeBookingStatus(int bookingId, BookingStatus status, PaymentStatus paymentStatus)
        {
            return await _bookingDao.ChangeBookingStatus(bookingId, status, paymentStatus);
        }

        public async Task<IEnumerable<Booking>> GetAllBookingAsync(string? search, DateTime? date = null, BookingStatus? status = null, PaymentStatus? paymentStatus = null)
        {
           return await _bookingDao.GetAllBookingAsync(search, date, status, paymentStatus);
        }

        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _bookingDao.GetBookingByIdAsync(bookingId);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByAccountId(string accountId)
        {
            return await _bookingDao.GetBookingsByAccountId(accountId);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date)
        {
            return await _bookingDao.GetBookingsByDateAsync(date);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByStatusAsync(BookingStatus status)
        {
            return await _bookingDao.GetBookingsByStatusAsync(status);
        }

        public async Task<Booking?> UpdateBookingWithReportAsync(int bookingId, Booking booking)
        {
           return await _bookingDao.UpdateBookingWithReportAsync(bookingId, booking);
        }

        public async Task<Booking?> GetBookingStatusByAccountId(string accountId)
        {
            return await _bookingDao.GetBookingStatusByAccountId(accountId);
        }

        public async Task<(int bookingsReturnOrCancell, int bookings, int bookingsComplete, int bookingsCancell, int bookingsReturnRefund, int bookingsReport)> GetStaticBookings()
        {
            return await _bookingDao.GetStaticBookings();
        }

        public async Task<List<(string homeStayName, int QuantityOfBooking)>> GetTopHomeStayBookingInMonthAsync()
        {
           return await _bookingDao.GetTopHomeStayBookingInMonthAsync();
        }

        public async Task<List<(object span, int totalBookings, double totalBookingsAmount)>> GetTotalBookingsTotalBookingsAmount(DateTime startDate, DateTime endDate, string? timeSpanType)
        {
            return await _bookingDao.GetTotalBookingsTotalBookingsAmountAsync(startDate, endDate, timeSpanType);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByHomeStayId(int homeStayID)
        {
            return await _bookingDao.GetBookingsByHomeStayId(homeStayID);
        }

        public async Task<Booking?> GetBookingsByIdAsync(int? bookingId)
        {
            return await _bookingDao.GetBookingsByIdAsync(bookingId);
        }
        public async Task<IEnumerable<Booking>> GetBookingsForCheckInReminderAsync()
        {
            return await _bookingDao.GetBookingsForCheckInReminderAsync();
        }

        public async Task<List<(object span, int totalBookings, double totalBookingsAmount)>> GetTotalBookingsTotalBookingsAmountForHomeStay(int homeStayID, DateTime startDate, DateTime endDate, string? timeSpanType)
        {
            return await _bookingDao.GetTotalBookingsTotalBookingsAmountForHomeStay(homeStayID, startDate, endDate, timeSpanType);
        }

        public async Task<List<(string accountID, string CustomerName, int BookingCount)>> GetTopLoyalCustomersAsync(int homeStayId, int top = 5)
        {
            return await _bookingDao.GetTopLoyalCustomersAsync(homeStayId, top);
        }

        public async Task<List<(Account Account, int TotalBooking)>> GetCustomersByHomeStay(int homeStayId)
        {
           return await _bookingDao.GetCustomersByHomeStayAsync(homeStayId);
        }

        public async Task<List<(string date, double totalBookingsAmount)>> GetCurrentWeekRevenueForHomeStay(int homestayId)
        {
            return await _bookingDao.GetCurrentWeekRevenueForHomeStayAsync(homestayId);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByRoom(int roomId)
        {
            return await _bookingDao.GetBookingsByRoomIdAsync(roomId);
        }

        public async Task<IEnumerable<Booking>> GetExpiredBookings()
        {
            return await _bookingDao.GetExpiredBookingsAsync();
        }
    }
}
