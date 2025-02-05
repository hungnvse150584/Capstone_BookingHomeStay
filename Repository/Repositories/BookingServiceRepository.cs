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
    public class BookingServiceRepository : BaseRepository<BookingServices>, IBookingServiceRepository
    {
        private readonly BookingServicesDAO _bookingservicesDao;
        public BookingServiceRepository(BookingServicesDAO bookingservicesDao) : base(bookingservicesDao)
        {
            _bookingservicesDao = bookingservicesDao;
        }

        public async Task AddBookingServicesAsync(BookingServices booking)
        {
             await _bookingservicesDao.AddBookingServicesAsync(booking);
        }

        public async Task<BookingServices?> ChangeBookingServicesStatus(int bookingId, BookingServicesStatus status)
        {
           return await _bookingservicesDao.ChangeBookingServicesStatus(bookingId, status);
        }

        public async Task<BookingServices?> FindBookingServicesByIdAsync(int? bookingId)
        {
            return await _bookingservicesDao.FindBookingServicesByIdAsync(bookingId);
        }

        public async Task<IEnumerable<BookingServices>> GetAllBookingServicesAsync(string? search, DateTime? date = null, BookingServicesStatus? status = null)
        {
           return await _bookingservicesDao.GetAllBookingServicesAsync(search, date, status);
        }

        public async Task<IEnumerable<BookingServices>> GetBookingServicesByAccountId(string accountId)
        {
            return await _bookingservicesDao.GetBookingServicesByAccountId(accountId);
        }

        public async Task<IEnumerable<BookingServices>> GetBookingServicesByDateAsync(DateTime date)
        {
            return await _bookingservicesDao.GetBookingServicesByDateAsync(date);
        }

        public async Task<BookingServices?> GetBookingServicesByIdAsync(int bookingId)
        {
            return await _bookingservicesDao.GetBookingServicesByIdAsync(bookingId);
        }

        public async Task<IEnumerable<BookingServices>> GetBookingServicesByStatusAsync(BookingServicesStatus status)
        {
            return await _bookingservicesDao.GetBookingServicesByStatusAsync(status);
        }

        public async Task<BookingServices?> GetUnpaidServicesByAccountId(string accountId)
        {
            return await _bookingservicesDao.GetUnpaidServicesByAccountId(accountId);
        }

        public async Task UpdateBookingServicesAsync(BookingServices booking)
        {
           await _bookingservicesDao.UpdateAsync(booking);
        }
    }
}
