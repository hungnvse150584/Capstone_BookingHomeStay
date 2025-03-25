using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IBookingServiceDetailRepository
    {
        Task<List<BookingServicesDetail>> GetBookingServiceDetailsToRemoveAsync(int bookingId, List<int> updatedDetailIds);
        Task<List<BookingServicesDetail>> DeleteBookingServiceDetailAsync(List<BookingServicesDetail> serviceDetails);
    }
}
