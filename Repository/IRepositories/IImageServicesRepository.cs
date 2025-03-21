using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IImageServicesRepository : IBaseRepository<ImageServices>
    {
        Task<IEnumerable<ImageServices>> GetAllByServiceIdAsync(int serviceId);
        Task<ImageServices> GetImageServiceByIdAsync(int id);
        Task<ImageServices> AddImageAsync(ImageServices image);
    }
}
