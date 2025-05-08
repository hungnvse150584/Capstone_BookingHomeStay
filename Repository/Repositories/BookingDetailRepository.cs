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
    public class BookingDetailRepository : BaseRepository<BookingDetail>, IBookingDetailRepository
    {
        private readonly BookingDetailDAO _bookingDetailDao;
        public BookingDetailRepository(BookingDetailDAO bookingDetailDao) : base(bookingDetailDao)
        {
            _bookingDetailDao = bookingDetailDao;
        }

        public async Task<List<BookingDetail>> DeleteBookingDetailAsync(List<BookingDetail> bookingDetails)
        {
            return await _bookingDetailDao.DeleteRange(bookingDetails);
        }

        public async Task<List<BookingDetail>> GetBookingDetailsToRemoveAsync(int bookingId, List<int> updatedDetailIds)
        {
            return await _bookingDetailDao.GetBookingDetailsToRemoveAsync(bookingId, updatedDetailIds);
        }

        public async Task<List<(string roomTypeName, int count)>> GetRoomTypeUsageStatsAsync(int homestayId)
        {
            return await _bookingDetailDao.GetRoomTypeUsageStatsAsync(homestayId);
        }
    }
}
