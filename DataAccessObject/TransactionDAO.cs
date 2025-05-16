using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class TransactionDAO : BaseDAO<Transaction>
    {
        private readonly GreenRoamContext _context;
        public TransactionDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByAccountId(string accountId)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Booking)
                .Include(t => t.BookingService)
                .Include(t => t.HomeStay)
                .Where(t => t.Account != null && t.Account.Id == accountId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactions()
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Booking)
                .Include(t => t.BookingService)
                .Include(t => t.HomeStay)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByHomeStayId(int homeStayID)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Booking)
                .Include(t => t.BookingService)
                .Include(t => t.HomeStay)
                .Where(t => t.HomeStay != null && t.HomeStay.HomeStayID == homeStayID)
                .ToListAsync();
        }

        public async Task<Transaction?> GetTransactionById(string transactionID)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Booking)
                .Include(t => t.BookingService)
                .Include(t => t.HomeStay)
                .FirstOrDefaultAsync(t => t.ResponseId == transactionID);
        }

        public async Task<Transaction?> GetTransactionByBookingId(int bookingID)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Booking)
                .Include(t => t.BookingService)
                .Include(t => t.HomeStay)
                .FirstOrDefaultAsync(t => t.Booking.BookingID == bookingID);
        }

        public async Task<Transaction?> GetTransactionByBookingServiceId(int bookingID)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Booking)
                .Include(t => t.BookingService)
                .Include(t => t.HomeStay)
                .FirstOrDefaultAsync(t => t.BookingService.BookingServicesID == bookingID);
        }

        public async Task<Transaction?> ChangeTransactionStatusForBooking(int? bookingId, StatusOfTransaction newStatus)
        {
            var transaction = await _context.Transactions
                .Where(t => t.Booking != null && t.Booking.BookingID == bookingId)
                .OrderByDescending(t => t.PayDate) // chọn transaction mới nhất
                .FirstOrDefaultAsync();

            if (transaction is null || transaction.StatusTransaction == newStatus)
                return transaction;

            transaction.StatusTransaction = newStatus;
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction?> ChangeTransactionStatusForBookingService(int? bookingId, StatusOfTransaction newStatus)
        {
            var transaction = await _context.Transactions
                .Where(t => t.BookingService != null && t.BookingService.BookingServicesID == bookingId)
                .OrderByDescending(t => t.PayDate) // chọn transaction mới nhất
                .FirstOrDefaultAsync();

            if (transaction is null || transaction.StatusTransaction == newStatus)
                return transaction;

            transaction.StatusTransaction = newStatus;
            await _context.SaveChangesAsync();

            return transaction;
        }
    }
}
