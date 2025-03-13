using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IImageHomeStayTypesRepository : IBaseRepository<ImageHomeStayRentals>
    {
    
        Task<IEnumerable<ImageHomeStayRentals>> GetAllByImageIdAsync(int imageId);
        Task<ImageHomeStayRentals> GetImageHomeStayTypesByIdAsync(int id);
    }
}
