using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class RoomDAO : BaseDAO<Room>
    {
        private readonly GreenRoamContext _context;
        public RoomDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms
                        .Include(r => r.RoomTypes)
                        .Include(r => r.ImageRooms)
                        .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetRoomsByRoomTypeIdAsync(int roomTypeId)
        {
            return await _context.Rooms
                .Include(r => r.ImageRooms)
                .Where(r => r.RoomTypesID == roomTypeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomFilter(DateTime checkInDate, DateTime checkOutDate)
        {
            if (checkInDate >= checkOutDate)
            {
                throw new ArgumentException("Check-out date must be after check-in date.");
            }

            var availableRooms = await _context.Rooms
                .Include(r => r.ImageRooms)
                .Where(r => r.isActive == true) // Phòng chưa bị chủ khóa và chưa có khách
                .Where(r => !_context.BookingDetails
                    .Where(bd => bd.RoomID != null) // Đảm bảo RoomID không null
                    .Any(bd => bd.RoomID == r.RoomID &&
                        // Kiểm tra các trạng thái booking hợp lệ
                        (
                            // Booking đã xác nhận và đã đặt cọc hoặc thanh toán toàn bộ
                            (bd.Booking.Status == BookingStatus.Confirmed &&
                             (bd.Booking.paymentStatus == PaymentStatus.Deposited ||
                              bd.Booking.paymentStatus == PaymentStatus.FullyPaid)) ||
                            // Booking đang diễn ra và đã thanh toán toàn bộ
                            (bd.Booking.Status == BookingStatus.InProgress &&
                             bd.Booking.paymentStatus == PaymentStatus.FullyPaid) ||
                            // Booking đang chờ xác nhận và chưa hết thời gian ExpiredTime
                            (bd.Booking.Status == BookingStatus.Pending &&
                             bd.Booking.ExpiredTime > DateTime.UtcNow) ||
                            // Booking đang yêu cầu hoàn tiền và chưa được hoàn tiền
                            (bd.Booking.Status == BookingStatus.RequestRefund &&
                             bd.Booking.paymentStatus != PaymentStatus.Refunded)
                        ) &&
                        // Kiểm tra trùng ngày
                        (checkInDate < bd.CheckOutDate && checkOutDate > bd.CheckInDate)))
                .ToListAsync();

            return availableRooms;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<Room> GetRoomByIdAsync(int? id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<Room>()
               .Include(r => r.ImageRooms)
               .SingleOrDefaultAsync(c => c.RoomID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

        public async Task<Room?> ChangeRoomStatusAsync(int roomId, bool? isActive)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room != null)
            {
                
                if (isActive.HasValue) room.isActive = isActive.Value;
                await _context.SaveChangesAsync();
            }

            return await _context.Rooms.FindAsync(roomId);
        }
        public async Task<IEnumerable<Room>> FilterRoomsByRoomTypeAndDates(int roomTypeId, DateTime checkInDate, DateTime checkOutDate)
        {
            // Log thời gian hiện tại
            Console.WriteLine($"Current DateTime.UtcNow: {DateTime.UtcNow}");

            if (checkInDate >= checkOutDate)
            {
                throw new ArgumentException("Check-out date must be after check-in date.");
            }

            // Log danh sách phòng trước khi lọc
            var roomsBeforeFilter = await _context.Rooms
                .Where(r => r.RoomTypesID == roomTypeId)
                .Where(r => r.isActive == true)
                .ToListAsync();
            Console.WriteLine($"Rooms before filter: {string.Join(", ", roomsBeforeFilter.Select(r => r.RoomID))}");

            // Thực hiện lọc phòng
            var availableRooms = await _context.Rooms
                .Where(r => r.RoomTypesID == roomTypeId)
                .Where(r => r.isActive == true)
                .Where(r => !_context.BookingDetails
                    .Where(bd => bd.RoomID != null)
                    .Any(bd => bd.RoomID == r.RoomID &&
                        (
                            (bd.Booking.Status == BookingStatus.Confirmed &&
                             (bd.Booking.paymentStatus == PaymentStatus.Deposited ||
                              bd.Booking.paymentStatus == PaymentStatus.FullyPaid)) ||
                            (bd.Booking.Status == BookingStatus.InProgress &&
                             bd.Booking.paymentStatus == PaymentStatus.FullyPaid) ||
                            (bd.Booking.Status == BookingStatus.Pending &&
                             bd.Booking.ExpiredTime > DateTime.UtcNow) ||
                            (bd.Booking.Status == BookingStatus.RequestRefund &&
                             bd.Booking.paymentStatus != PaymentStatus.Refunded)
                        ) &&
                        (checkInDate < bd.CheckOutDate && checkOutDate > bd.CheckInDate)))
                .Include(r => r.RoomTypes)
                .Include(r => r.ImageRooms)
                .ToListAsync();

            // Log danh sách phòng sau khi lọc
            Console.WriteLine($"Rooms after filter: {string.Join(", ", availableRooms.Select(r => r.RoomID))}");

            return availableRooms;
        }
        public async Task<IEnumerable<Room>> FilterAllRoomsByHomeStayIDAsync(int homeStayID, DateTime? startDate, DateTime? endDate)
        {
            Console.WriteLine($"Current DateTime.UtcNow: {DateTime.UtcNow}");

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate >= endDate)
                {
                    throw new ArgumentException("Ngày kết thúc phải sau ngày bắt đầu.");
                }
            }

            IQueryable<Room> query = _context.Rooms
                .Where(r => r.RoomTypes != null)
                .Where(r => r.RoomTypes.HomeStayRentals != null)
                .Where(r => r.RoomTypes.HomeStayRentals.HomeStayID == homeStayID)
                .Where(r => r.isActive == true);

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(r => !_context.BookingDetails
                    .Where(bd => bd.RoomID != null)
                    .Any(bd => bd.RoomID == r.RoomID &&
                        (
                            (bd.Booking.Status == BookingStatus.Confirmed &&
                             (bd.Booking.paymentStatus == PaymentStatus.Deposited ||
                              bd.Booking.paymentStatus == PaymentStatus.FullyPaid)) ||
                            (bd.Booking.Status == BookingStatus.InProgress) ||
                            (bd.Booking.Status == BookingStatus.Pending &&
                             bd.Booking.ExpiredTime > DateTime.UtcNow) ||
                            (bd.Booking.Status == BookingStatus.RequestRefund &&
                             bd.Booking.paymentStatus != PaymentStatus.Refunded)
                        ) &&
                        (startDate < bd.CheckOutDate && endDate > bd.CheckInDate)));
            }

            query = query
          
                .Include(r => r.RoomTypes)
                .ThenInclude(rt => rt.HomeStayRentals)
                .Include(r => r.RoomTypes)
                .ThenInclude(rt => rt.Prices)
                .Include(r => r.ImageRooms);

            var rooms = await query.AsNoTracking().ToListAsync();

            Console.WriteLine($"Phòng sau khi lọc cho HomeStayID {homeStayID}: {string.Join(", ", rooms.Select(r => r.RoomID))}");
            foreach (var room in rooms)
            {
                Console.WriteLine($"RoomID: {room.RoomID}, RoomTypesID: {room.RoomTypesID}, RoomTypeName: {room.RoomTypes?.Name}");
                if (room.RoomTypes?.Prices != null)
                {
                    Console.WriteLine($"Giá cho RoomTypesID {room.RoomTypesID}: {string.Join(", ", room.RoomTypes.Prices.Select(p => $"RentPrice: {p.RentPrice}, DayType: {p.DayType}, IsActive: {p.IsActive}"))}");
                }
                else
                {
                    Console.WriteLine($"Không tìm thấy giá cho RoomTypesID {room.RoomTypesID}");
                }
            }

            return rooms;
        }
        public async Task<IEnumerable<Room>> FilterAllRoomsByHomeStayRentalIDAsync(int homeStayRentalID, DateTime? startDate, DateTime? endDate)
        {
            Console.WriteLine($"Current DateTime.UtcNow: {DateTime.UtcNow}");

            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate >= endDate)
                {
                    throw new ArgumentException("Ngày kết thúc phải sau ngày bắt đầu.");
                }
            }

            IQueryable<Room> query = _context.Rooms
                .Where(r => r.RoomTypes != null)
                .Where(r => r.RoomTypes.HomeStayRentals != null)
                .Where(r => r.RoomTypes.HomeStayRentals.HomeStayRentalID == homeStayRentalID)
                .Where(r => r.isActive == true);

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(r => !_context.BookingDetails
                    .Where(bd => bd.RoomID != null)
                    .Any(bd => bd.RoomID == r.RoomID &&
                        (
                            (bd.Booking.Status == BookingStatus.Confirmed &&
                             (bd.Booking.paymentStatus == PaymentStatus.Deposited ||
                              bd.Booking.paymentStatus == PaymentStatus.FullyPaid)) ||
                            (bd.Booking.Status == BookingStatus.InProgress) ||
                            (bd.Booking.Status == BookingStatus.Pending &&
                             bd.Booking.ExpiredTime > DateTime.UtcNow) ||
                            (bd.Booking.Status == BookingStatus.RequestRefund &&
                             bd.Booking.paymentStatus != PaymentStatus.Refunded)
                        ) &&
                        (startDate < bd.CheckOutDate && endDate > bd.CheckInDate)));
            }

            query = query
                .Include(r => r.RoomTypes)
                .ThenInclude(rt => rt.HomeStayRentals)
                .Include(r => r.RoomTypes)
                .ThenInclude(rt => rt.Prices)
                .Include(r => r.ImageRooms);

            var rooms = await query.AsNoTracking().ToListAsync();

            Console.WriteLine($"Phòng sau khi lọc cho HomeStayRentalID {homeStayRentalID}: {string.Join(", ", rooms.Select(r => r.RoomID))}");
            foreach (var room in rooms)
            {
                Console.WriteLine($"RoomID: {room.RoomID}, RoomTypesID: {room.RoomTypesID}, RoomTypeName: {room.RoomTypes?.Name}");
                if (room.RoomTypes?.Prices != null)
                {
                    Console.WriteLine($"Giá cho RoomTypesID {room.RoomTypesID}: {string.Join(", ", room.RoomTypes.Prices.Select(p => $"RentPrice: {p.RentPrice}, DayType: {p.DayType}, IsActive: {p.IsActive}"))}");
                }
                else
                {
                    Console.WriteLine($"Không tìm thấy giá cho RoomTypesID {room.RoomTypesID}");
                }
            }

            return rooms;
        }
    }
}
