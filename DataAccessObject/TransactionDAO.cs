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
    }
}
