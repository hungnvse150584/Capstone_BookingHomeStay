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
    public class PropertyRepository : BaseRepository<Property>, IPropertyRepository
    {
        private readonly PropertyDAO _propertyDao;

        public PropertyRepository(PropertyDAO propertyDao) : base(propertyDao)
        {
            _propertyDao = propertyDao;
        }

        public Task<Property> AddAsync(Property entity)
        {
            return _propertyDao.AddAsync(entity);
        }

        public Task<Property> UpdateAsync(Property entity)
        {
            return _propertyDao.UpdateAsync(entity);
        }

        public Task<Property> DeleteAsync(Property entity)
        {
            return _propertyDao.DeleteAsync(entity);
        }

        public Task<IEnumerable<Property>> GetAllAsync()
        {
            return _propertyDao.GetAllAsync();
        }

        public Task<Property> GetByIdAsync(int id)
        {
            return _propertyDao.GetByIdAsync(id);
        }
    }
}
