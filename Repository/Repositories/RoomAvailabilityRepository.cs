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
    public class RoomAvailabilityRepository : BaseRepository<RoomAvailability>, IRoomAvailabilityRepository
    {
        private readonly RoomAvailabilityDAO _roomAvaibilityDao;

        public RoomAvailabilityRepository(RoomAvailabilityDAO roomAvaibilityDao) : base(roomAvaibilityDao)
        {
            _roomAvaibilityDao = roomAvaibilityDao;
        }

        public async Task<IEnumerable<RoomAvailability>> GetAvailableRoomsAsync()
        {
            return await _roomAvaibilityDao.GetAvailableRoomsAsync();
        }

        public Task<List<RoomAvailability>> AddRangeAsync(List<RoomAvailability> entity)
        {
            return _roomAvaibilityDao.AddRange(entity);
        }

        public Task<List<RoomAvailability>> UpdateRangeAsync(List<RoomAvailability> entity)
        {
            return _roomAvaibilityDao.UpdateRange(entity);
        }

        public Task<RoomAvailability> DeleteAsync(RoomAvailability entity)
        {
            return _roomAvaibilityDao.DeleteAsync(entity);
        }
    }
}
