using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IImageRoomTypesRepository : IBaseRepository<ImageRoomTypes>
    {
        Task AddImageAsync(ImageRoomTypes image);
        Task SaveChangesAsync();
    }
}
