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
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        private readonly TransactionDAO _transactionDao;

        public TransactionRepository(TransactionDAO transactionDao) : base(transactionDao)
        {
            _transactionDao = transactionDao;
        }

        public async Task<Transaction?> GetTransactionByBookingId(int bookingID)
        {
            return await _transactionDao.GetTransactionByBookingId(bookingID);
        }

        public async Task<Transaction?> GetTransactionById(string transactionID)
        {
            return await _transactionDao.GetTransactionById(transactionID);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByAccountId(string accountId)
        {
            return await _transactionDao.GetTransactionsByAccountId(accountId);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByHomeStayId(int homeStayID)
        {
            return await _transactionDao.GetTransactionsByHomeStayId(homeStayID);
        }
    }
}
