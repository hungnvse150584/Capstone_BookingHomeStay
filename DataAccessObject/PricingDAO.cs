using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class PricingDAO : BaseDAO<Pricing>
    {
        private readonly GreenRoamContext _context;
        public PricingDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pricing>> GetAllPricingByHomeStayAsync(int homestayID)
        {
            if (homestayID <= 0)
            {
                throw new ArgumentNullException($"id {homestayID} not found");
            }
            return await _context.Prices
               .Where(p => p.HomeStayRentals.HomeStayID == homestayID)
               .Include(p => p.HomeStayRentals)
               .Include(p => p.RoomTypes)
               .ToListAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Pricing>> GetPricingByHomeStayRentalAsync(int rentalID)
        {
            if (rentalID <= 0)
            {
                throw new ArgumentNullException($"id {rentalID} not found");
            }
            return await _context.Prices
               .Where(p => p.HomeStayRentalID == rentalID)
               .Include(p => p.HomeStayRentals)
               .ToListAsync();
        }

        public async Task<IEnumerable<Pricing>> GetPricingByRoomTypeAsync(int roomTypeID)
        {
            if (roomTypeID <= 0)
            {
                throw new ArgumentNullException($"id {roomTypeID} not found");
            }
            return await _context.Prices
               .Where(p => p.RoomTypesID == roomTypeID)
               .Include(p => p.RoomTypes)
               .ToListAsync();
        }


        public async Task<Pricing> GetPricingByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<Pricing>()
               .Include(p => p.HomeStayRentals)
               .Include(p => p.RoomTypes)
               .SingleOrDefaultAsync(c => c.PricingID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

        public async Task<List<Pricing>> GetPricingDetailsToRemoveAsync(int homeStayRentalID, List<int> updatedDetailIds)
        {
            return await _context.Prices
                                 .Where(d => d.HomeStayRentalID == homeStayRentalID && !updatedDetailIds.Contains(d.PricingID))
                                 .ToListAsync();
        }

        public async Task<DayType> GetDayType(DateTime date, int? homeStayRentalId, int? roomtypeId)
        {
            // Kiểm tra xem ngày có nằm trong một khoảng Holiday không
            bool isHoliday = await _context.Prices
            .AnyAsync(p => p.DayType == DayType.Holiday &&
                           (
                                (roomtypeId.HasValue && p.RoomTypesID == roomtypeId) ||
                                (homeStayRentalId.HasValue && p.RoomTypesID == null && p.HomeStayRentalID == homeStayRentalId)
                           ) &&
                           date.Date >= p.StartDate.Value.Date &&
                           date.Date <= p.EndDate.Value.Date);

            if (isHoliday)
            {
                return DayType.Holiday;
            }

            // Nếu là Thứ 7 hoặc Chủ nhật => Cuối tuần
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                return DayType.Weekend;
            }

            return DayType.Weekday;
        }

        public async Task<double> GetTotalPrice(
    DateTime checkInDate, DateTime checkOutDate, int? homeStayRentalId, int? roomTypeId = null)
        {
            double totalRentPrice = 0;
            

            // Nếu cả hai đều null thì không đủ dữ liệu
            if (!homeStayRentalId.HasValue && !roomTypeId.HasValue)
                throw new ArgumentException("Cần cung cấp HomeStayRentalID hoặc RoomTypeID!");

            bool isRentWhole = false;

            if (homeStayRentalId.HasValue)
            {
                isRentWhole = await _context.HomeStayRentals
                    .Where(h => h.HomeStayRentalID == homeStayRentalId.Value)
                    .Select(h => h.RentWhole)
                    .FirstOrDefaultAsync();
            }
            else if (roomTypeId.HasValue)
            {
                // Lấy HomeStayRentalID từ RoomType nếu chỉ có RoomTypeID
                homeStayRentalId = await _context.RoomTypes
                    .Where(r => r.RoomTypesID == roomTypeId.Value)
                    .Select(r => r.HomeStayRentalID)
                    .FirstOrDefaultAsync();

                if (homeStayRentalId == 0)
                    throw new ArgumentException("Không tìm thấy HomeStayRental từ RoomTypeID!");

                isRentWhole = await _context.HomeStayRentals
                    .Where(h => h.HomeStayRentalID == homeStayRentalId.Value)
                    .Select(h => h.RentWhole)
                    .FirstOrDefaultAsync();
            }

            // Nếu là thuê nguyên căn mà lại truyền RoomTypeID thì sai
            if (isRentWhole && roomTypeId.HasValue)
                throw new ArgumentException("HomeStayRental này cho thuê nguyên căn, không hỗ trợ RoomType!");

            // Nếu là thuê theo phòng mà thiếu RoomTypeID thì cũng sai
            if (!isRentWhole && !roomTypeId.HasValue)
                throw new ArgumentException("HomeStayRental không cho thuê nguyên căn, cần cung cấp RoomTypeID!");

            // Lấy danh sách giá phù hợp
            List<Pricing> priceList;

            if (isRentWhole)
            {
                priceList = await _context.Prices
                    .Where(p => p.HomeStayRentalID == homeStayRentalId && p.RoomTypesID == null)
                    .ToListAsync();
            }
            else
            {
                priceList = await _context.Prices
                    .Where(p => p.RoomTypesID == roomTypeId.Value)
                    .ToListAsync();
            }

            // Duyệt từng ngày để tính tổng giá
            for (DateTime date = checkInDate.Date; date < checkOutDate.Date; date = date.AddDays(1))
            {
                DayType dayType = await GetDayType(date, homeStayRentalId, roomTypeId);

                var pricing = priceList.FirstOrDefault(p => p.DayType == dayType)
                           ?? priceList.FirstOrDefault(p => p.DayType == DayType.Weekday); // fallback

                if (pricing != null)
                {
                    totalRentPrice += pricing.RentPrice;
                    
                }
            }

            return totalRentPrice;
        }
    }
}
