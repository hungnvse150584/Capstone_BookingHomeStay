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
    public class ProvinceRepository : BaseRepository<Province>, IProvinceRepository
    {
        private readonly ProvinceDAO _provinceDao;

        public ProvinceRepository(ProvinceDAO provinceDao) : base(provinceDao)
        {
            _provinceDao = provinceDao;
        }

        public Task<Province> AddAsync(Province entity)
        {
            return _provinceDao.AddAsync(entity);
        }

        public Task<Province> UpdateAsync(Province entity)
        {
            return _provinceDao.UpdateAsync(entity);
        }

        public Task<Province> DeleteAsync(Province entity)
        {
            return _provinceDao.DeleteAsync(entity);
        }

        public Task<IEnumerable<Province>> GetAllAsync()
        {
            return _provinceDao.GetAllProvinceAsync();
        }

        public Task<Province> GetByIdAsync(int id)
        {
            return _provinceDao.GetProvinceByIdAsync(id);
        }

        public Task<string> GetProvinceNameById(int? provinceId)
        {
            return _provinceDao.GetProvinceNameById(provinceId);
        }
    }
}
