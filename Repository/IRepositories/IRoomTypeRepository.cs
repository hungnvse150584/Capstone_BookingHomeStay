using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IRoomTypeRepository : IBaseRepository<RoomTypes>
    {
<<<<<<< HEAD:Repository/IRepositories/IHomeStayTypeRepository.cs
        Task<HomeStayTypes> GetHomeStayTypesByIdAsync(int? id);
        
        Task<IEnumerable<HomeStayTypes>> GetAllHomeStayTypesAsync(int typeId);
       
=======
        Task<RoomTypes> GetRoomTypesByIdAsync(int? id);
>>>>>>> main:Repository/IRepositories/IRoomTypeRepository.cs
    }
}
