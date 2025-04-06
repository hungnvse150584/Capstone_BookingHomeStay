using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IServiceRepository : IBaseRepository<Services>
    {
        Task<IEnumerable<Services>> GetAllServiceAsync(int homestayId = 0);
        Task<IEnumerable<Services>> GetServicesByIdsAsync(List<int> servicesIds);
        //Task<Services> GetServiceByIdAsync(int id);
        Task SaveChangesAsync();
    }
}
