using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface ICommissionRateRepository
    {
        Task<IEnumerable<CommissionRate>> GetAllCommissionRate();
        Task<CommissionRate?> GetCommissionRateByHomeStay(int homeStayID);
        Task<CommissionRate> AddAsync(CommissionRate entity);
    }
}
