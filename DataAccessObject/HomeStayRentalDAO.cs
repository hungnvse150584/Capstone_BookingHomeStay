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

    }
}
