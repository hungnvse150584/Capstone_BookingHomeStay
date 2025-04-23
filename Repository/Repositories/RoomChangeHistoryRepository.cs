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
    public class RoomChangeHistoryRepository : BaseRepository<RoomChangeHistory>, IRoomChangeHistoryRepository
    {
        private readonly RoomChangeHistoryDAO _roomDao;

        public RoomChangeHistoryRepository(RoomChangeHistoryDAO roomDao) : base(roomDao)
        {
            _roomDao = roomDao;
        }

        public async Task AddRoomHistory(RoomChangeHistory room)
        {
             await _roomDao.AddRoomHistoryAsync(room);
        }

        public async Task<IEnumerable<RoomChangeHistory>> GetRoomsByBookingDetailId(int bookingDetailId)
        {
            return await _roomDao.GetRoomsByBookingDetailIdAsync(bookingDetailId);
        }
    }
}
