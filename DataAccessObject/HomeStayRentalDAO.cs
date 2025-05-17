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

        public async Task<IEnumerable<HomeStayRentals>> GetHomeStayTypesByIdsAsync(List<int?> homeStayTypeIds)
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
            Console.WriteLine($"DAO: Parameters received - HomeStayID: {homeStayId}, RentWhole: {rentWhole?.ToString() ?? "null"}, " +
                              $"CheckInDate: {checkInDate}, CheckOutDate: {checkOutDate}, NumberOfAdults: {numberOfAdults}, " +
                              $"NumberOfChildren: {numberOfChildren}");

            var query = _context.HomeStayRentals
                .AsNoTracking()
                .Where(h => h.HomeStayID == homeStayId &&
                            h.Status == true &&
                            h.MaxAdults >= numberOfAdults &&
                            h.MaxChildren >= numberOfChildren &&
                            h.MaxPeople >= (numberOfAdults + numberOfChildren));

            // Log giá trị rentWhole
            Console.WriteLine($"DAO: RentWhole value received: {rentWhole?.ToString() ?? "null"}");

            // Áp dụng lọc RentWhole nếu có giá trị
            if (rentWhole.HasValue)
            {
                Console.WriteLine($"DAO: Applying RentWhole filter: {rentWhole.Value}");
                query = query.Where(h => h.RentWhole == rentWhole.Value);
            }
            else
            {
                Console.WriteLine("DAO: No RentWhole filter applied.");
            }

            // Include các quan hệ cần thiết
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

            // Thực thi truy vấn
            var result = await query.ToListAsync();

            // Log số lượng HomeStayRentals trả về trước khi lọc
            Console.WriteLine($"DAO: Number of HomeStayRentals retrieved: {result.Count}");

            // Tính toán và lọc các HomeStayRentals dựa trên availability
            var filteredResult = result.Select(rental =>
            {
                int availableRooms = 0;

                if (rental.RentWhole)
                {
                    // Với RentWhole=true, kiểm tra có booking nào trong khoảng thời gian không
                    var hasBooking = rental.BookingDetails
                        .Any(bd => bd.Booking != null &&
                                  (bd.Booking.Status == BookingStatus.Pending ||
                                   bd.Booking.Status == BookingStatus.Confirmed ||
                                   bd.Booking.Status == BookingStatus.InProgress) &&
                                  bd.CheckInDate.Date <= checkOutDate &&
                                  bd.CheckOutDate.Date >= checkInDate);

                    availableRooms = hasBooking ? 0 : 1;
                }
                else
                {
                    // Với RentWhole=false, tính số phòng khả dụng dựa trên từng phòng
                    var allRoomIds = rental.RoomTypes
                        .SelectMany(rt => rt.Rooms)
                        .Select(r => r.RoomID)
                        .ToList();

                    if (allRoomIds.Any())
                    {
                        var bookedRoomIds = rental.BookingDetails
                            .Where(bd => bd.Booking != null &&
                                        (bd.Booking.Status == BookingStatus.Pending ||
                                         bd.Booking.Status == BookingStatus.Confirmed ||
                                         bd.Booking.Status == BookingStatus.InProgress) &&
                                        bd.CheckInDate.Date <= checkOutDate &&
                                        bd.CheckOutDate.Date >= checkInDate &&
                                        bd.RoomID.HasValue)
                            .Select(bd => bd.RoomID.Value)
                            .Distinct()
                            .ToList();

                        availableRooms = allRoomIds.Count - bookedRoomIds.Count;

                        // Tính AvailableRoomsCount cho từng RoomType (dùng tạm thời trong logic)
                        foreach (var roomType in rental.RoomTypes)
                        {
                            var roomTypeRoomIds = roomType.Rooms
                                .Select(r => r.RoomID)
                                .ToList();

                            var bookedRoomTypeRoomIds = bookedRoomIds
                                .Where(roomId => roomTypeRoomIds.Contains(roomId))
                                .ToList();

                            // Gán tạm giá trị AvailableRoomsCount (vì không sửa model, chỉ dùng trong logic)
                            int availableRoomsForType = roomTypeRoomIds.Count - bookedRoomTypeRoomIds.Count;
                            Console.WriteLine($"DAO: RoomTypeID: {roomType.RoomTypesID}, AvailableRoomsCount: {availableRoomsForType}");
                        }
                    }
                    else
                    {
                        availableRooms = 0;
                    }
                }

                // Log thông tin cho từng rental
                Console.WriteLine($"DAO: HomeStayRentalID: {rental.HomeStayRentalID}, RentWhole: {rental.RentWhole}, " +
                                  $"AvailableRooms: {availableRooms}");

                // Chỉ giữ lại các rental có availableRooms > 0
                return new { Rental = rental, AvailableRooms = availableRooms };
            })
            .Where(x => x.AvailableRooms > 0)
            .Select(x => x.Rental)
            .ToList();

            // Log số lượng HomeStayRentals sau khi lọc
            Console.WriteLine($"DAO: Number of HomeStayRentals after filtering: {filteredResult.Count}");

            return filteredResult;
        }
    }
}