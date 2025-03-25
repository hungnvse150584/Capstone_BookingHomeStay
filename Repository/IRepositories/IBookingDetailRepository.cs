using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IBookingDetailRepository
    {
        Task<List<BookingDetail>> DeleteBookingDetailAsync(List<BookingDetail> bookingDetails);
        Task<List<BookingDetail>> GetBookingDetailsToRemoveAsync(int bookingId, List<int> updatedDetailIds);
    }
}
