using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IImageRoomRepository : IBaseRepository<ImageRoom>
    {
        Task AddImageAsync(ImageRoom image);
        Task SaveChangesAsync();
    }
}
