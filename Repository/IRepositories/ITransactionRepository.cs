﻿using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface ITransactionRepository : IBaseRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetAllTransactions();
        Task<IEnumerable<Transaction>> GetTransactionsByAccountId(string accountId);
        Task<IEnumerable<Transaction>> GetTransactionsByHomeStayId(int homeStayID);
        Task<Transaction?> GetTransactionById(string transactionID);
        Task<Transaction?> GetTransactionByBookingId(int bookingID);
        Task<Transaction?> GetTransactionByBookingServiceId(int bookingID);
        Task<Transaction?> ChangeTransactionStatusForBooking(int? bookingId, StatusOfTransaction newStatus);
        Task<Transaction?> ChangeTransactionStatusForBookingService(int? bookingId, StatusOfTransaction newStatus);
    }
}
