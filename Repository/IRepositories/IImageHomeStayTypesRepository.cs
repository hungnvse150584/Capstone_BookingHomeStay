using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IImageHomeStayTypesRepository : IBaseRepository<ImageHomeStayTypes>
    {
    
        Task<IEnumerable<ImageHomeStayTypes>> GetAllByImageIdAsync(int imageId);
        Task<ImageHomeStayTypes> GetImageHomeStayTypesByIdAsync(int id);
    }
}
