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
    public class LocationRepository : BaseRepository<Location>, ILocationRepository
    {
        private readonly LocationDAO _locationDao;

        public LocationRepository(LocationDAO locationDao) : base(locationDao)
        {
            _locationDao = locationDao;
        }

        public Task<Location> AddAsync(Location entity)
        {
            return _locationDao.AddAsync(entity);
        }

        public Task<Location> UpdateAsync(Location entity)
        {
            return _locationDao.UpdateAsync(entity);
        }

        public Task<Location> DeleteAsync(Location entity)
        {
            return _locationDao.DeleteAsync(entity);
        }

        public Task<IEnumerable<Location>> GetAllAsync()
        {
            return _locationDao.GetAllAsync();
        }

        public Task<Location> GetByIdAsync(int id)
        {
            return _locationDao.GetByIdAsync(id);
        }
    }
}
