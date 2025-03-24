using BusinessObject.Model;
using DataAccessObject;
using Microsoft.EntityFrameworkCore;
using Repository.BaseRepository;
using Repository.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class RoomRepository : BaseRepository<Room>, IRoomRepository
    {
        private readonly RoomDAO _roomDao;

        public RoomRepository(RoomDAO roomDao) : base(roomDao)
        {
            _roomDao = roomDao;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _roomDao.GetAllRoomsAsync();
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomFilter(DateTime checkInDate, DateTime checkOutDate)
        {
            return await _roomDao.GetAvailableRoomFilter(checkInDate, checkOutDate);
        }

        public async Task<Room> GetRoomByIdAsync(int? id)
        {
            return await _roomDao.GetRoomByIdAsync(id);
        }

        public async Task<IEnumerable<Room>> GetAllRoomsByRoomTypeIdAsync(int roomTypeId)
        {
            return await _roomDao.GetRoomsByRoomTypeIdAsync(roomTypeId);
        }

        public async Task<Room> AddAsync(Room entity)
        {
            return await _roomDao.AddAsync(entity);
        }

        public async Task<Room> UpdateAsync(Room entity)
        {
            return await _roomDao.UpdateAsync(entity);
        }

        public async Task<Room> DeleteAsync(Room entity)
        {
            return await _roomDao.DeleteAsync(entity);
        }

        public async Task<Room?> ChangeRoomStatusAsync(int roomId, bool? isUsed, bool? isActive)
        {
            return await _roomDao.ChangeRoomStatusAsync(roomId, isUsed, isActive);
        }
        public async Task SaveChangesAsync()
        {
            await _roomDao.SaveChangesAsync();
        }
    }
}
