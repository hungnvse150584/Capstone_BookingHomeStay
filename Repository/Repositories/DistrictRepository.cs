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
    public class DistrictRepository : BaseRepository<District>, IDistrictRepository
    {
        private readonly DistrictDAO _districtDao;

        public DistrictRepository(DistrictDAO districtDao) : base(districtDao)
        {
            _districtDao = districtDao;
        }

        public Task<District> AddAsync(District entity)
        {
            return _districtDao.AddAsync(entity);
        }

        public Task<District> DeleteAsync(District entity)
        {
            return _districtDao.DeleteAsync(entity);
        }

        public Task<IEnumerable<District>> GetAllAsync()
        {
            return _districtDao.GetAllDistrictAsync();
        }

        public Task<District> GetByIdAsync(int id)
        {
            return _districtDao.GetDistrictByIdAsync(id);
        }

        public Task<string> GetDistrictNameById(int? districtId)
        {
            return _districtDao.GetDistrictNameById(districtId);
        }
    }
}
