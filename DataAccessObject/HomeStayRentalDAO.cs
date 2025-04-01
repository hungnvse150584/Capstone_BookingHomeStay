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
        public async Task<IEnumerable<HomeStayRentals>> FilterHomeStayRentalsAsync(
            int homeStayId,
            bool rentWhole,
            DateTime checkInDate,
            DateTime checkOutDate,
            int numberOfAdults,
            int numberOfChildren)
        {
            if (homeStayId <= 0)
            {
                throw new ArgumentException("HomeStayID must be greater than 0.");
            }

            // Bước 1: Lấy tất cả HomeStayRentals (đã lọc Status, RentWhole và HomeStayID tại database)
            var query = _context.HomeStayRentals
                .AsNoTracking()
                .Where(h => h.HomeStayID == homeStayId &&
                            h.RentWhole == rentWhole &&
                            h.Status == true &&
                            h.MaxAdults >= numberOfAdults &&
                            h.MaxChildren >= numberOfChildren &&
                            h.MaxPeople >= (numberOfAdults + numberOfChildren))
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

            if (!homeStayRentals.Any())
            {
                return new List<HomeStayRentals>();
            }

            // Bước 2: Lọc các HomeStayRental có ít nhất 1 phòng trống
            var finalFilteredRentals = new List<HomeStayRentals>();
            foreach (var rental in homeStayRentals)
            {
                var roomIds = rental.RoomTypes?
                    .SelectMany(rt => rt.Rooms)
                    .Where(r => r.isActive)
                    .Select(r => r.RoomID)
                    .ToList() ?? new List<int>();

                if (!roomIds.Any())
                {
                    Console.WriteLine($"HomeStayRentalID: {rental.HomeStayRentalID} has no active rooms available.");
                    continue;
                }

                var bookedRoomIds = rental.BookingDetails
                    .Where(bd => bd.RoomID.HasValue &&
                                 roomIds.Contains(bd.RoomID.Value) &&
                                 bd.Booking != null &&
                                 (bd.Booking.Status == BookingStatus.Pending ||
                                  bd.Booking.Status == BookingStatus.Confirmed ||
                                  bd.Booking.Status == BookingStatus.InProgress) &&
                                 bd.CheckInDate.Date <= checkOutDate &&
                                 bd.CheckOutDate.Date >= checkInDate)
                    .Select(bd => bd.RoomID.Value)
                    .Distinct()
                    .ToList();

                bool hasAvailableRoom = roomIds.Any(roomId => !bookedRoomIds.Contains(roomId));
                if (hasAvailableRoom)
                {
                    Console.WriteLine($"HomeStayRentalID: {rental.HomeStayRentalID} has at least one available room.");
                    finalFilteredRentals.Add(rental);
                }
                else
                {
                    Console.WriteLine($"HomeStayRentalID: {rental.HomeStayRentalID} has no available rooms.");
                }
            }

            return finalFilteredRentals;
        }
    }
}
