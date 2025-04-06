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
        Task<RoomTypes> GetRoomTypesByIdAsync(int? id);
        Task<IEnumerable<RoomTypes>> GetRoomTypesByIdsAsync(List<int> roomTypeIds);
        Task SaveChangesAsync();
        Task<IEnumerable<RoomTypes>> GetAllRoomTypeByHomeStayRentalID(int homeStayRentalId);
    }
}
