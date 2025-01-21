using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IBookingServiceRepository
    {
        Task<IEnumerable<BookingServices>> GetAllBookingServicesAsync(string? search, DateTime? date = null, BookingServicesStatus? status = null);
        Task<IEnumerable<BookingServices>> GetBookingServicesByAccountId(string accountId);
        Task<BookingServices?> ChangeBookingServicesStatus(int bookingId, BookingServicesStatus status);
        Task AddBookingServicesAsync(BookingServices booking);
        Task UpdateBookingServicesAsync(BookingServices booking);
        Task<IEnumerable<BookingServices>> GetBookingServicesByDateAsync(DateTime date);
        Task<IEnumerable<BookingServices>> GetBookingServicesByStatusAsync(BookingServicesStatus status);
        Task<BookingServices?> GetBookingServicesByIdAsync(int bookingId);
        Task<BookingServices?> GetUnpaidServicesByAccountId(string accountId);
    }
}
