using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
     public interface ICancellationPolicyRepository : IBaseRepository<CancellationPolicy>
    {
        Task<IEnumerable<CancellationPolicy>> GetAllAsync();
        Task<CancellationPolicy> GetByIdAsync(int id);
        
    }
}
