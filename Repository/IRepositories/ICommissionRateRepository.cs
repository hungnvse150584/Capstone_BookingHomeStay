using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface ICommissionRateRepository : IBaseRepository<CommissionRate>
    {
        Task<IEnumerable<CommissionRate>> GetAllCommissionRate();
        Task<CommissionRate?> GetCommissionRateByHomeStay(int homeStayID);
        Task<CommissionRate?> GetCommissionByHomeStayAsync(int? homeStayID);
        Task<CommissionRate?> GetCommissionRateByIDAsync(int rateID);
        Task<CommissionRate> AddAsync(CommissionRate entity);
        Task<CommissionRate> UpdateAsync(CommissionRate entity);
        Task SaveChangesAsync();
    }
}
