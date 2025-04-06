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
    public class HomeStayRentalDAO : BaseDAO<HomeStayRentals>
    {
        private readonly GreenRoamContext _context;
        public HomeStayRentalDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HomeStayRentals>> GetAllHomeStayTypesAsync(int homestayId)
        {
            return await _context.HomeStayRentals
           .Where(c => c.HomeStayID == homestayId)
           .Include(c => c.Prices)  
           .Include(c => c.HomeStay)
           .Include(c => c.ImageHomeStayRentals)
           .Include(c => c.RoomTypes)
           .Include(c => c.BookingDetails)
            .ThenInclude(bd => bd.Booking)
           .ToListAsync();

        }

        public async Task<IEnumerable<HomeStayRentals>> GetHomeStayTypesByIdsAsync(List<int> homeStayTypeIds)
        {
            return await _context.HomeStayRentals.Where(h => homeStayTypeIds.Contains(h.HomeStayRentalID)).ToListAsync();
        }

        public async Task<IEnumerable<HomeStayRentals>> GetAllHomeStayTypesAsyncFilter(int? homestayId, bool? rentWhole = null)
        {
            var query = _context.HomeStayRentals.AsQueryable();

            // Áp dụng điều kiện Where trước Include
            query = query.Where(r => r.Status == true);

            if (homestayId.HasValue)
            {
                query = query.Where(c => c.HomeStayID == homestayId);
            }

            if (rentWhole.HasValue)
            {
                query = query.Where(c => c.RentWhole == rentWhole.Value);
            }

            // Sau đó áp dụng Include và ThenInclude
            query = query
                .Include(c => c.Prices)
                .Include(c => c.HomeStay)
                .Include(c => c.ImageHomeStayRentals)
                .Include(c => c.RoomTypes)
                .Include(c => c.BookingDetails)
                    .ThenInclude(bd => bd.Booking);

            return await query.ToListAsync();
        }
        public async Task AddRoomTypeAsync(RoomTypes roomType)
        {
            await _context.RoomTypes.AddAsync(roomType);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<HomeStayRentals> GetHomeStayTypeByIdAsync(int? id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }

            var entity = await _context.Set<HomeStayRentals>()
                .Include(c => c.HomeStay) // Bao gồm thông tin HomeStay
                .Include(c => c.ImageHomeStayRentals) // Bao gồm hình ảnh
                .Include(c => c.Prices) // Bao gồm giá
                .Include(c => c.RoomTypes) // Bao gồm RoomTypes
                    .ThenInclude(rt => rt.ImageRoomTypes) // Bao gồm hình ảnh của RoomType
                .Include(c => c.RoomTypes)
                    .ThenInclude(rt => rt.Prices) // Bao gồm giá của RoomType
                .Include(c => c.RoomTypes)
                    .ThenInclude(rt => rt.Rooms) // Bao gồm phòng của RoomType
                .Include(c => c.BookingDetails) // Bao gồm BookingDetails
                    .ThenInclude(bd => bd.Booking) // Bao gồm thông tin Booking nếu cần
                .SingleOrDefaultAsync(c => c.HomeStayRentalID == id);

            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }

            return entity;
        }
        public async Task<IEnumerable<HomeStayRentals>> FilterHomeStayRentalsAsync(
     int homeStayId,
     bool? rentWhole,
     DateTime checkInDate,
     DateTime checkOutDate,
     int numberOfAdults,
     int numberOfChildren)
        {
            if (homeStayId <= 0)
            {
                throw new ArgumentException("HomeStayID must be greater than 0.");
            }

            var query = _context.HomeStayRentals
                .AsNoTracking()
                .Where(h => h.HomeStayID == homeStayId &&
                            h.Status == true &&
                            h.MaxAdults >= numberOfAdults &&
                            h.MaxChildren >= numberOfChildren &&
                            h.MaxPeople >= (numberOfAdults + numberOfChildren));

            // Chỉ thêm điều kiện RentWhole nếu rentWhole có giá trị
            if (rentWhole.HasValue)
            {
                query = query.Where(h => h.RentWhole == rentWhole.Value);
            }

            query = query
                .Include(h => h.RoomTypes)
                    .ThenInclude(rt => rt.Rooms)
                .Include(h => h.RoomTypes)
                    .ThenInclude(rt => rt.ImageRoomTypes)
                .Include(h => h.RoomTypes)
                    .ThenInclude(rt => rt.Prices)
                .Include(h => h.HomeStay)
                .Include(h => h.ImageHomeStayRentals)
                .Include(h => h.Prices)
                .Include(h => h.BookingDetails)
                    .ThenInclude(bd => bd.Booking);

            var homeStayRentals = await query.ToListAsync();

            foreach (var rental in homeStayRentals)
            {
                rental.BookingDetails = rental.BookingDetails
                    .Where(bd => bd.HomeStayRentalID == rental.HomeStayRentalID)
                    .ToList();
            }

            return homeStayRentals;
        }
    }
}
