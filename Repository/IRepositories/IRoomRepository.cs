using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IRoomRepository : IBaseRepository<Room>
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<IEnumerable<Room>> GetAllRoomsByRoomTypeIdAsync(int roomTypeId);
        Task<IEnumerable<Room>> GetAvailableRoomFilter(DateTime checkInDate, DateTime checkOutDate);
        Task<Room> GetRoomByIdAsync(int? id);
        Task<Room?> ChangeRoomStatusAsync(int roomId, bool? isUsed, bool? isActive);
        Task SaveChangesAsync();
        Task<IEnumerable<Room>> FilterRoomsByRoomTypeAndDates(int roomTypeId, DateTime checkInDate, DateTime checkOutDate);
    }
}
