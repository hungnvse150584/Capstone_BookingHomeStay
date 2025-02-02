using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class BookingDAO : BaseDAO<Booking>
    {
        private readonly GreenRoamContext _context;
        public BookingDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllBookingAsync(string? search, DateTime? date = null, BookingStatus? status = null)
        {
            IQueryable<Booking> bookings = _context.Bookings
                .Include(b => b.Account)
                .Include(b => b.BookingDetails)
                .Include(b => b.BookingServices)
                .ThenInclude(bs => bs.BookingServicesDetails);
            
            if (date.HasValue)
            {
                bookings = bookings.Where(o => o.BookingDate.Date == date.Value.Date);
            }

            // Apply status filter if provided
            if (status.HasValue)
            {
                bookings = bookings.Where(o => o.Status == status.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                bookings = bookings.Where(o => o.Account.Phone.ToString().Contains(search.ToLower()) || o.Account.Email.ToString().Contains(search.ToLower()));

            }

            // Execute the query and return the results as a list
            return await bookings.ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByAccountId(string accountId)
        {
            return await _context.Bookings
                .Include(b => b.BookingDetails)
                .Where(b => b.AccountID == accountId)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingStatusByAccountId(string accountId)
        {
            return await _context.Bookings
                .Include(b => b.BookingDetails)
                .FirstOrDefaultAsync(b => b.AccountID == accountId && b.Status == BookingStatus.ToPay);
        }



        public async Task<Booking?> ChangeBookingStatus(int bookingId, BookingStatus status)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking != null)
            {
                booking.Status = status;
                await _context.SaveChangesAsync();
            }

            return await _context.Bookings.FindAsync(bookingId);
        }

        public async Task AddBookingAsync(Booking order)
        {
            await _context.Bookings.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date)
        {
            return await _context.Bookings
                .Include(o => o.Account)
                .Include(o => o.BookingDetails)
                .Where(o => o.BookingDate.Date == date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByStatusAsync(BookingStatus status)
        {
            return await _context.Bookings
                .Include(o => o.Account)
                .Include(o => o.BookingDetails)
                .Where(o => o.Status == status)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(o => o.Account)
                .Include(o => o.BookingDetails)
                .FirstOrDefaultAsync(o => o.BookingID == bookingId);
        }

        public async Task<Booking?> UpdateBookingWithReportAsync(int bookingId, Booking booking)
        {

            var existBooking = await _context.Bookings.FindAsync(bookingId);
            if (existBooking != null)
            {
                existBooking.ReportID = booking.ReportID;
            }
            await _context.SaveChangesAsync();
            return existBooking;
        }


        //For Admin and Owner DashBoard
        //Admin DashBoard
        public async Task<(int bookingsReturnOrCancell, int bookings, int bookingsComplete, int bookingsCancell, int bookingsReturnRefund, int bookingsReport)> GetStaticBookings()
        {
            // Lấy ngày hiện tại
            DateTime today = DateTime.Today;

            // Xác định ngày đầu tuần (ngày thứ Hai là ngày đầu tuần)
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

            // Xác định ngày cuối tuần (ngày Chủ nhật là ngày cuối tuần)
            DateTime endOfWeek = startOfWeek.AddDays(6);

            int bookingsReturnOrCancell = await _context.Bookings
                 .Where(o => o.BookingDetails
                     .Any(d => (d.CheckInDate >= startOfWeek && d.CheckInDate <= endOfWeek)
                            || (d.CheckOutDate >= startOfWeek && d.CheckOutDate <= endOfWeek)))
                 .Where(o => o.Status == BookingStatus.Cancelled
                          || o.Status == BookingStatus.RequestReturn
                          || o.Status == BookingStatus.ReturnRefund)
                 .CountAsync();

            int bookings = await _context.Bookings
                                .Where(o => o.BookingDetails
                                .Any( d => (d.CheckInDate >= startOfWeek && d.CheckInDate <= endOfWeek)
                                        || (d.CheckOutDate >= startOfWeek && d.CheckOutDate <= endOfWeek)))
                                .CountAsync();

            int bookingsComplete = await _context.Bookings
                                .Where(o => o.BookingDetails
                                .Any(d => (d.CheckInDate >= startOfWeek && d.CheckInDate <= endOfWeek)
                                        || (d.CheckOutDate >= startOfWeek && d.CheckOutDate <= endOfWeek)))
                                .Where(o => o.Status == BookingStatus.Completed)
                                .CountAsync();

            int bookingsCancell = await _context.Bookings
                                .Where(o => o.BookingDetails
                                .Any(d => (d.CheckInDate >= startOfWeek && d.CheckInDate <= endOfWeek)
                                        || (d.CheckOutDate >= startOfWeek && d.CheckOutDate <= endOfWeek)))
                                .Where(o => o.Status == BookingStatus.Cancelled)
                                .CountAsync();

            int bookingsReturnRefund = await _context.Bookings
                                .Where(o => o.BookingDetails
                                .Any(d => (d.CheckInDate >= startOfWeek && d.CheckInDate <= endOfWeek)
                                        || (d.CheckOutDate >= startOfWeek && d.CheckOutDate <= endOfWeek)))
                                .Where(o => o.Status == BookingStatus.ReturnRefund)
                                .CountAsync();

            int bookingsReport = await _context.Bookings
                               .Where(o => o.BookingDetails
                                .Any(d => (d.CheckInDate >= startOfWeek && d.CheckInDate <= endOfWeek)
                                        || (d.CheckOutDate >= startOfWeek && d.CheckOutDate <= endOfWeek)))
                                .Where(o => o.ReportID != null)
                                .CountAsync();
            return (bookingsReturnOrCancell, bookings, bookingsComplete, bookingsCancell, bookingsReturnRefund, bookingsReport);
        }

        public async Task<List<(string homeStayName, int QuantityOfBooking)>> GetTopHomeStayBookingInMonthAsync()
        {
            // Lấy ngày hiện tại
            DateTime today = DateTime.Today;

            // Xác định ngày đầu tháng và ngày cuối tháng
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // Lấy danh sách các Booking trong khoảng thời gian từ startOfMonth đến endOfMonth
            IEnumerable<Booking> bookingsInMonth = await _context.Bookings
                .Where(o => o.BookingDetails
                .Any(d => (d.CheckInDate >= startOfMonth && d.CheckInDate <= endOfMonth)
                       || (d.CheckOutDate >= startOfMonth && d.CheckOutDate <= endOfMonth)))
                .Include(o => o.BookingDetails) // Include BookingRoomDetails
                .ThenInclude(d => d.HomeStayTypes) // Include HomeStayPlaces from BookingRoomDetails
                .ToListAsync();

            // Tính tổng số lượng bookings đã đặt của từng homestay
            var homeStayQuantities = bookingsInMonth
                .SelectMany(b => b.BookingDetails)
                .GroupBy(bd => bd.HomeStayTypes.HomeStayID)
                .Select(g => new
                {
                    HomeStayID = g.Key,
                    QuantityOfBooking = g.Sum(bd => bd.Quantity)
                })
                .ToList();

            //Lấy tên HomeStay từ homeStayQuantities
            var homeStayNames = await _context.HomeStays
                .Where(hs => homeStayQuantities.Select(h => h.HomeStayID).Contains(hs.HomeStayID))
                .ToListAsync();

            //Lấy top theo homeStay
            var result = homeStayQuantities
                .Select(h =>
                {
                    var homeStayName = homeStayNames
                        .FirstOrDefault(hs => hs.HomeStayID == h.HomeStayID)?.Name; // Tìm tên HomeStay
                    return (homeStayName, h.QuantityOfBooking);
                })
                .OrderByDescending(h => h.QuantityOfBooking)
                .Take(4)
                .ToList();

            // Chuyển đổi kết quả sang List<(string homeStayName, int QuantityOfBooking)>
            List<(string homeStayName, int QuantityOfBooking)> topHomeStay = result
                .Select(p => (p.homeStayName, p.QuantityOfBooking))
                .ToList();

            return topHomeStay;
        }
    }
}
