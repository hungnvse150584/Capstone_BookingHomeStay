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
    public class StreetRepository : BaseRepository<Street>, IStreetRepository
    {
        private readonly StreetDAO _streetDao;

        public StreetRepository(StreetDAO streetDao) : base(streetDao)
        {
            _streetDao = streetDao;
        }

        public Task<Street> AddAsync(Street entity)
        {
            return _streetDao.AddAsync(entity);
        }

        public Task<Street> UpdateAsync(Street entity)
        {
            return _streetDao.UpdateAsync(entity);
        }

        public Task<Street> DeleteAsync(Street entity)
        {
            return _streetDao.DeleteAsync(entity);
        }

        public Task<IEnumerable<Street>> GetAllAsync()
        {
            return _streetDao.GetAllStreetAsync();
        }

        public Task<Street> GetByIdAsync(int id)
        {
            return _streetDao.GetStreetByIdAsync(id);
        }

        public Task<string> GetStreetNameById(int? streetId)
        {
            return _streetDao.GetStreetNameById(streetId);
        }
    }
}
