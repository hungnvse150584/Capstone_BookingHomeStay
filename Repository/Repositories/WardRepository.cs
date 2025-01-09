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
    public class WardRepository : BaseRepository<Ward>, IWardRepository
    {
        private readonly WardDAO _wardDao;

        public WardRepository(WardDAO wardDao) : base(wardDao)
        {
            _wardDao = wardDao;
        }

        public Task<Ward> AddAsync(Ward entity)
        {
            return _wardDao.AddAsync(entity);
        }

        public Task<Ward> UpdateAsync(Ward entity)
        {
            return _wardDao.UpdateAsync(entity);
        }

        public Task<Ward> DeleteAsync(Ward entity)
        {
            return _wardDao.DeleteAsync(entity);
        }

        public Task<IEnumerable<Ward>> GetAllAsync()
        {
            return _wardDao.GetAllWardAsync();
        }

        public Task<Ward> GetByIdAsync(int id)
        {
            return _wardDao.GetWardByIdAsync(id);
        }

        public Task<string> GetWardNameById(int? wardId)
        {
            return _wardDao.GetWardNameById(wardId);
        }
    }
}
