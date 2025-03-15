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
    public class RoomTypeRepository : BaseRepository<RoomTypes>, IRoomTypeRepository
    {
        private readonly RoomTypesDAO _roomTypeDao;

        public RoomTypeRepository(RoomTypesDAO roomTypeDao) : base(roomTypeDao)
        {
            _roomTypeDao = roomTypeDao;
        }

        public async Task<RoomTypes> AddAsync(RoomTypes entity)
        {
            return await _roomTypeDao.AddAsync(entity);
        }

        public async Task<RoomTypes> UpdateAsync(RoomTypes entity)
        {
            return await _roomTypeDao.UpdateAsync(entity);
        }

        public async Task<RoomTypes> DeleteAsync(RoomTypes entity)
        {
            return await _roomTypeDao.DeleteAsync(entity);
        }

        public async Task<IEnumerable<RoomTypes>> GetAllAsync()
        {
            return await _roomTypeDao.GetAllRoomTypesAsync();
        }

        public async Task<RoomTypes> GetRoomTypesByIdAsync(int? id)
        {
            return await _roomTypeDao.GetRoomTypeByIdAsync(id);
        }
    }
}
