using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IRoomChangeHistoryRepository : IBaseRepository<RoomChangeHistory>
    {
        Task AddRoomHistory(RoomChangeHistory room);
        Task<IEnumerable<RoomChangeHistory>> GetRoomsByBookingDetailId(int bookingDetailId);
    }
}
