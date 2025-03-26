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

        public async Task<HomeStay> ChangeHomeStayStatus(int homestayId, HomeStayStatus status, int? commissionRateId = null)
        {
            var homestay = await _homestayDao.GetByIdAsync(homestayId);
            if (homestay == null)
            {
                return null;
            }

            homestay.Status = status;
            if (commissionRateId.HasValue)
            {
                homestay.CommissionRateID = commissionRateId.Value;
            }
            homestay.UpdateAt = DateTime.UtcNow;

            await _homestayDao.UpdateAsync(homestay);
            await _homestayDao.SaveChangesAsync();

            return homestay;
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

        public async Task SaveChangesAsync()
        {
            await _homestayDao.SaveChangesAsync();
        }

        /*public async Task<IEnumerable<HomeStay>> GetNearestHomeStaysAsync(double userLat, double userLon, int topN = 5)
        {
            return await _homestayDao.GetNearestHomeStaysAsync(userLat, userLon, topN);
        }*/

        public async Task<IEnumerable<HomeStay>> GetNearestHomeStaysAsync(double userLat, double userLon, int pageIndex = 1, int pageSize = 5)
        {
            return await _homestayDao.GetNearestHomeStaysAsync(userLat, userLon, pageIndex, pageSize);
        }
    }
}
