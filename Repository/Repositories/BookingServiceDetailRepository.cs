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
    public class BookingServiceDetailRepository : BaseRepository<BookingServicesDetail>, IBookingServiceDetailRepository
    {
        private readonly BookingServiceDetailDAO _bookingServiceDetailDao;
        public BookingServiceDetailRepository(BookingServiceDetailDAO bookingServiceDetailDao) : base(bookingServiceDetailDao)
        {
            _bookingServiceDetailDao = bookingServiceDetailDao;
        }

        public async Task<List<BookingServicesDetail>> DeleteBookingServiceDetailAsync(List<BookingServicesDetail> serviceDetails)
        {
            return await _bookingServiceDetailDao.DeleteRange(serviceDetails);
        }

        public async Task<List<BookingServicesDetail>> GetBookingServiceDetailsToRemoveAsync(int bookingId, List<int> updatedDetailIds)
        {
            return await _bookingServiceDetailDao.GetServiceDetailsToRemoveAsync(bookingId, updatedDetailIds);
        }

        public async Task<List<(string serviceName, int count)>> GetServiceUsageStatsAsync(int homestayId)
        {
            return await _bookingServiceDetailDao.GetServiceUsageStatsAsync(homestayId);
        }
    }
}
