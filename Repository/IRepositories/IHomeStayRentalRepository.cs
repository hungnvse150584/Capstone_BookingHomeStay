using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IHomeStayRentalRepository : IBaseRepository<HomeStayRentals>
    {
        Task<HomeStayRentals> GetHomeStayTypesByIdAsync(int? id);
        Task<IEnumerable<HomeStayRentals>> GetAllHomeStayTypesAsync(int homestayId);
        Task SaveChangesAsync();
        Task AddRoomTypeAsync(RoomTypes roomType);
        Task<IEnumerable<HomeStayRentals>> GetAllAsyncFilter(bool? rentWhole = null);
        Task<IEnumerable<HomeStayRentals>> GetAllHomeStayTypesAsyncFilter(int? homestayId, bool? rentWhole = null);
        Task<IEnumerable<HomeStayRentals>> FilterHomeStayRentalsAsync(
            int homeStayId,
            bool rentWhole,
            DateTime checkInDate,
            DateTime checkOutDate,
            int numberOfAdults,
            int numberOfChildren);
    }
}
