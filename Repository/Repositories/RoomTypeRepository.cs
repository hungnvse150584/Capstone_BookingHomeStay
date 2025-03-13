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

        public Task<RoomTypes> AddAsync(RoomTypes entity)
        {
            return _roomTypeDao.AddAsync(entity);
        }

        public Task<RoomTypes> UpdateAsync(RoomTypes entity)
        {
            return _roomTypeDao.UpdateAsync(entity);
        }

        public Task<RoomTypes> DeleteAsync(RoomTypes entity)
        {
            return _roomTypeDao.DeleteAsync(entity);
        }

        public Task<IEnumerable<RoomTypes>> GetAllAsync(int roomTypeId)
        {
            return _roomTypeDao.GetAllRoomTypesAsync(roomTypeId);
        }

        public Task<RoomTypes> GetRoomTypesByIdAsync(int? id)
        {
            return _roomTypeDao.GetRoomTypeByIdAsync(id);
        }
    }
}
