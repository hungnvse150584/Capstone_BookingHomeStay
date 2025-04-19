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
    public class BookingServicesDAO : BaseDAO<BookingServices>
    {
        private readonly GreenRoamContext _context;
        public BookingServicesDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookingServices>> GetAllBookingServicesAsync(string? search, DateTime? date = null, BookingServicesStatus? status = null, PaymentServicesStatus? paymentStatus = null)
        {
            IQueryable<BookingServices> bookings = _context.BookingServices
                .Include(b => b.Account)
                .Include(b => b.BookingServicesDetails)
                .ThenInclude(bd => bd.Services)
                .ThenInclude(s => s.ImageServices);
            //.Include(o => o.Transaction)
            // Apply date filter if provided
            if (date.HasValue)
            {
                bookings = bookings.Where(o => o.BookingServicesDate.Date == date.Value.Date);
            }

            // Apply status filter if provided
            if (status.HasValue)
            {
                bookings = bookings.Where(o => o.Status == status.Value);
            }

            if (paymentStatus.HasValue)
            {
                bookings = bookings.Where(o => o.PaymentServiceStatus == paymentStatus.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                bookings = bookings.Where(o => o.Account.Phone.ToString().Contains(search.ToLower()) || o.Account.Email.ToString().Contains(search.ToLower()));

            }

            // Execute the query and return the results as a list
            return await bookings.ToListAsync();
        }

        public async Task<IEnumerable<BookingServices>> GetBookingServicesByAccountId(string accountId)
        {
            return await _context.BookingServices
                .Include(b => b.Account)
                .Include(o => o.Booking)
                .ThenInclude(o => o.BookingDetails)
                .Include(b => b.HomeStay)
                .Include(b => b.BookingServicesDetails)
                .ThenInclude(bd => bd.Services)
                .ThenInclude(s => s.ImageServices)
                .Where(b => b.AccountID == accountId)
                .ToListAsync();
        }

        public async Task<BookingServices?> ChangeBookingServicesStatus(int bookingId, BookingServicesStatus status, PaymentServicesStatus statusPayment)
        {
            var booking = await _context.BookingServices.FindAsync(bookingId);
            if (booking != null)
            {
                booking.Status = status;
                booking.PaymentServiceStatus = statusPayment;
                await _context.SaveChangesAsync();
            }

            return await _context.BookingServices.FindAsync(bookingId);
        }

        public async Task<IEnumerable<BookingServices>> GetBookingServiceByAccountId(string accountId)
        {
            return await _context.BookingServices
                .Include(b => b.BookingServicesDetails)
                .ThenInclude(bd => bd.Services)
                .Include(b => b.Booking)
                .ThenInclude(o => o.BookingDetails)
                .Include(b => b.Account)
                .Include(b => b.HomeStay)
                .Where(b => b.AccountID == accountId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingServices>> GetConfirmedBookingServiceByBookingId(int? bookingID)
        {
            return await _context.BookingServices
                .Include(b => b.BookingServicesDetails)
                .ThenInclude(bd => bd.Services)
                .Include(b => b.Booking)
                .ThenInclude(o => o.BookingDetails)
                .Include(b => b.Transactions)
                .Where(b => b.BookingID == bookingID && b.Status == BookingServicesStatus.Confirmed)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingServices>> GetBookingServicesByHomeStayId(int homeStayID)
        {
            return await _context.BookingServices
                .Include(b => b.BookingServicesDetails)
                .ThenInclude(bd => bd.Services)
                .Include(b => b.Booking)
                .ThenInclude(o => o.BookingDetails)
                .Include(b => b.Account)
                .Include(b => b.HomeStay)
                .Where(b => b.Booking != null && b.Booking.HomeStayID == homeStayID)
                .ToListAsync();
        }

        public async Task AddBookingServicesAsync(BookingServices order)
        {
            await _context.BookingServices.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BookingServices>> GetBookingServicesByDateAsync(DateTime date)
        {
            return await _context.BookingServices
                .Include(o => o.Account)
                .Include(o => o.Booking)
                .ThenInclude(o => o.BookingDetails)
                .Include(o => o.BookingServicesDetails)
                .ThenInclude(bd => bd.Services)
                .ThenInclude(s => s.ImageServices)
                .Where(o => o.BookingServicesDate.Date == date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingServices>> GetBookingServicesByStatusAsync(BookingServicesStatus status)
        {
            return await _context.BookingServices
                .Include(o => o.Account)
                .Include(o => o.Booking)
                .ThenInclude(o => o.BookingDetails)
                .Include(o => o.BookingServicesDetails)
                .ThenInclude(bd => bd.Services)
                .ThenInclude(s => s.ImageServices)
                .Where(o => o.Status == status)
                .ToListAsync();
        }

        public async Task<BookingServices?> GetBookingServicesByIdAsync(int bookingId)
        {
            return await _context.BookingServices
                .Include(o => o.Account)
                .Include(o => o.Booking)
                .ThenInclude(o => o.BookingDetails)
                .Include(b => b.HomeStay)
                .Include(o => o.BookingServicesDetails)
                .ThenInclude(bd => bd.Services)
                .ThenInclude(s => s.ImageServices)
                .FirstOrDefaultAsync(o => o.BookingServicesID == bookingId);
        }

        public async Task<BookingServices?> GetBookingServiceByIdAsync(int? bookingId)
        {
            return await _context.BookingServices
                .Include(o => o.Account)
                .Include(o => o.Booking)
                .ThenInclude(o => o.BookingDetails)
                .Include(b => b.HomeStay)
                .Include(o => o.BookingServicesDetails)
                .ThenInclude(bd => bd.Services)
                .ThenInclude(s => s.ImageServices)
                .FirstOrDefaultAsync(o => o.BookingServicesID == bookingId);
        }

        public async Task<BookingServices?> GetBookingServicesByBookingIdAsync(int bookingId)
        {
            return await _context.BookingServices
                .Include(o => o.Account)
                .Include(o => o.Booking)
                .ThenInclude(o => o.BookingDetails)
                .Include(b => b.HomeStay)
                .Include(o => o.BookingServicesDetails)
                .ThenInclude(bd => bd.Services)
                .ThenInclude(s => s.ImageServices)
                .FirstOrDefaultAsync(o => o.BookingID == bookingId && o.Status == BookingServicesStatus.Pending);
        }

        public async Task<BookingServices?> FindBookingServicesByIdAsync(int? bookingId)
        {
            return await _context.BookingServices
                .Include(o => o.Account)
                .Include(o => o.Booking)
                .ThenInclude(o => o.BookingDetails)
                .Include(b => b.HomeStay)
                .Include(o => o.BookingServicesDetails)
                .ThenInclude(bd => bd.Services)
                .ThenInclude(s => s.ImageServices)
                .FirstOrDefaultAsync(o => o.BookingServicesID == bookingId);
        }

        public async Task<BookingServices?> GetUnpaidServicesByAccountId(string accountId)
        {
            return await _context.BookingServices
                .Include(b => b.BookingServicesDetails)
                .ThenInclude(bd => bd.Services)
                .FirstOrDefaultAsync(b => b.AccountID == accountId && b.Status == BookingServicesStatus.Pending);
        }

        /*public async Task<Booking?> UpdateBookingWithReportAsync(int bookingId, Booking booking)
        {

            var existBooking = await _context.Bookings.FindAsync(bookingId);
            if (existBooking != null)
            {
                existBooking.ReportID = booking.ReportID;
            }
            await _context.SaveChangesAsync();
            return existBooking;
        }*/
    }
}
