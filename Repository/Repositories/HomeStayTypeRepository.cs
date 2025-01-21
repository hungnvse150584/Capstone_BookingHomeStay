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
    public class HomeStayTypeRepository : BaseRepository<HomeStayTypes>, IHomeStayTypeRepository
    {
        private readonly HomeStayTypeDAO _homestaytypeDao;

        public HomeStayTypeRepository(HomeStayTypeDAO homestaytypeDao) : base(homestaytypeDao)
        {
            _homestaytypeDao = homestaytypeDao;
        }

        public Task<HomeStayTypes> AddAsync(HomeStayTypes entity)
        {
            return _homestaytypeDao.AddAsync(entity);
        }

        public Task<HomeStayTypes> UpdateAsync(HomeStayTypes entity)
        {
            return _homestaytypeDao.UpdateAsync(entity);
        }

        public Task<HomeStayTypes> DeleteAsync(HomeStayTypes entity)
        {
            return _homestaytypeDao.DeleteAsync(entity);
        }

        public Task<IEnumerable<HomeStayTypes>> GetAllAsync(int homestayId)
        {
            return _homestaytypeDao.GetAllHomeStayTypesAsync(homestayId);
        }

        public Task<HomeStayTypes> GetHomeStayTypesByIdAsync(int? id)
        {
            return _homestaytypeDao.GetHomeStayTypeByIdAsync(id);
        }
    }
}
