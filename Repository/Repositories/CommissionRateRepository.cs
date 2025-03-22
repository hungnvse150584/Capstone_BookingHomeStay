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
    public class CommissionRateRepository : BaseRepository<CommissionRate>, ICommissionRateRepository
    {
        private readonly CommissionRateDAO _commissionRateDao;

        public CommissionRateRepository(CommissionRateDAO commissionRateDao) : base(commissionRateDao)
        {
            _commissionRateDao = commissionRateDao;
        }

        public async Task<IEnumerable<CommissionRate>> GetAllCommissionRate()
        {
            return await _commissionRateDao.GetAllCommissionRateAsync();
        }

        public async Task<CommissionRate?> GetCommissionRateByHomeStay(int homeStayID)
        {
            return await _commissionRateDao.GetCommissionRateByHomeStayAsync(homeStayID);
        }

        public async Task<CommissionRate> AddAsync(CommissionRate entity)
        {
            return await _commissionRateDao.AddAsync(entity);
        }

        public async Task<CommissionRate> UpdateAsync(CommissionRate entity)
        {
            return await _commissionRateDao.UpdateAsync(entity);
        }

        public async Task<CommissionRate> DeleteAsync(CommissionRate entity)
        {
            return await _commissionRateDao.DeleteAsync(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _commissionRateDao.SaveChangesAsync();
        }

        public async Task<CommissionRate?> GetCommissionByHomeStayAsync(int? homeStayID)
        {
            return await _commissionRateDao.GetCommissionByHomeStayAsync(homeStayID);
        }
    }
}
