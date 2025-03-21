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

        public async Task<DayType> GetDayType(DateTime date)
        {
            // Kiểm tra xem ngày có nằm trong một khoảng Holiday không
            bool isHoliday = await _context.Prices
            .AnyAsync(p => p.DayType == DayType.Holiday &&
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

        public async Task<(double totalRentPrice, double totalUnitPrice)> GetTotalPrice(
        DateTime checkInDate, DateTime checkOutDate, int homeStayRentalId, int? roomTypeId = null)
        {
            double totalRentPrice = 0;
            double totalUnitPrice = 0;

            // Kiểm tra xem HomeStayRental có RentWhole = true không
            bool isRentWhole = await _context.HomeStayRentals
                .Where(h => h.HomeStayRentalID == homeStayRentalId)
                .Select(h => h.RentWhole)
                .FirstOrDefaultAsync();

            if (roomTypeId.HasValue)
            {
                bool isValidRoomType = await _context.RoomTypes
                    .AnyAsync(r => r.RoomTypesID == roomTypeId.Value && r.HomeStayRentalID == homeStayRentalId);

                if (!isValidRoomType)
                {
                    throw new ArgumentException("RoomType không thuộc HomeStayRental!");
                }

                if (isRentWhole)
                {
                    throw new ArgumentException("HomeStayRental này cho thuê nguyên căn, không hỗ trợ RoomType!");
                }
            }

            // Lấy tất cả giá theo HomeStayRental hoặc RoomType
            var priceList = await _context.Prices
                .Where(p => p.HomeStayRentalID == homeStayRentalId &&
                            (isRentWhole ? p.RoomTypesID == null : p.RoomTypesID == roomTypeId.Value))
                .ToListAsync();

            for (DateTime date = checkInDate.Date; date <= checkOutDate.Date; date = date.AddDays(1))
            {
                // Xác định loại ngày (Holiday, Weekend, Weekday)
                DayType dayType = await GetDayType(date.Date);

                // Tìm giá theo loại ngày
                var pricing = priceList.FirstOrDefault(p => p.DayType == dayType);

                // Nếu không có giá cho ngày này, fallback về RegularDay (Weekday)
                if (pricing == null)
                {
                    pricing = priceList.FirstOrDefault(p => p.DayType == DayType.Weekday);
                }

                if (pricing != null)
                {
                    totalRentPrice += pricing.RentPrice;
                    totalUnitPrice += pricing.UnitPrice;
                }
            }

            return (totalRentPrice, totalUnitPrice);
        }
    }
}
