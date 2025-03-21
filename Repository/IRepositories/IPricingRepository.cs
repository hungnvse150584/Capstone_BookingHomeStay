using BusinessObject.Model;
using Repository.IBaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IPricingRepository : IBaseRepository<Pricing>
    {
        Task<IEnumerable<Pricing>> GetAllPricingByHomeStayAsync(int homestayID);
        Task<IEnumerable<Pricing>> GetPricingByHomeStayRentalAsync(int rentalID);
        Task<IEnumerable<Pricing>> GetPricingByRoomTypeAsync(int roomTypeID);
        Task<Pricing> GetPricingByIdAsync(int id);
        Task<DayType> GetDayType(DateTime date);
        Task<(double totalRentPrice, double totalUnitPrice)> GetTotalPrice(
        DateTime checkInDate, DateTime checkOutDate, int homeStayRentalId, int? roomTypeId = null);
    }
}
