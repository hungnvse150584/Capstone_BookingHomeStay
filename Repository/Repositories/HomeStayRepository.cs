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
    public class HomeStayRepository : BaseRepository<HomeStay>, IHomeStayRepository
    {
        private readonly HomeStayDAO _homestayDao;

        public HomeStayRepository(HomeStayDAO homestayDao) : base(homestayDao)
        {
            _homestayDao = homestayDao;
        }

        public async Task<HomeStay?> ChangeHomeStayStatus(int homestayId, HomeStayStatus status)
        {
            return await _homestayDao.ChangeHomeStayStatus(homestayId, status);
        }

        public async Task<List<HomeStay>> AddListAsync(List<HomeStay> entity)
        {
            return await _homestayDao.AddRange(entity);
        }

        public async Task<HomeStay> UpdateAsync(HomeStay entity)
        {
            return await _homestayDao.UpdateAsync(entity);
        }

        public async Task<HomeStay> DeleteAsync(HomeStay entity)
        {
            return await _homestayDao.DeleteAsync(entity);
        }

        public async Task<IEnumerable<HomeStay>> GetAllAsync()
        {
            return await _homestayDao.GetAllHomeStayAsync();
        }

        public async Task<HomeStay> GetByIdAsync(int id)
        {
            return await _homestayDao.GetHomeStayByIdAsync(id);
        }

        //Admin + Owner
        public async Task<IEnumerable<HomeStay>> GetAllRegisterHomeStayAsync()
        {
            return await _homestayDao.GetAllRegisterHomeStayAsync();
        }

        public async Task<HomeStay> GetHomeStayDetailByIdAsync(int id)
        {
            return await _homestayDao.GetHomeStayDetailByIdAsync(id);
        }

        public  async Task<HomeStay> GetOwnerHomeStayByIdAsync(string accountId)
        {
           return await _homestayDao.GetOwnerHomeStayByIdAsync(accountId);
        }
    }
}
