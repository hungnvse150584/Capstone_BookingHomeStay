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
        Task<IEnumerable<BookingServices>> GetAllBookingServicesAsync(string? search, DateTime? date = null, BookingServicesStatus? status = null, PaymentServicesStatus? paymentStatus = null);
        Task<IEnumerable<BookingServices>> GetBookingServicesByAccountId(string accountId);
        Task<BookingServices?> ChangeBookingServicesStatus(int bookingId, BookingServicesStatus status, PaymentServicesStatus statusPayment);
        Task AddBookingServicesAsync(BookingServices booking);
        Task UpdateBookingServicesAsync(BookingServices booking);
        Task<IEnumerable<BookingServices>> GetBookingServicesByDateAsync(DateTime date);
        Task<IEnumerable<BookingServices>> GetBookingServicesByStatusAsync(BookingServicesStatus status);
        Task<BookingServices?> GetBookingServicesByIdAsync(int bookingId);
        Task<BookingServices?> GetBookingServiceByIdAsync(int? bookingId);
        Task<BookingServices?> GetBookingServicesByBookingIdAsync(int bookingId);
        Task<BookingServices?> FindBookingServicesByIdAsync(int? bookingId);
        Task<BookingServices?> GetUnpaidServicesByAccountId(string accountId);
        Task<IEnumerable<BookingServices>> GetBookingServiceByAccountId(string accountId);
        Task<IEnumerable<BookingServices>> GetBookingServicesByHomeStayId(int homeStayID);
        Task<IEnumerable<BookingServices>> GetConfirmedBookingServiceByBookingId(int? bookingID);
        Task<bool> ExistsBookingServiceCodeAsync(string code);

    }
}
