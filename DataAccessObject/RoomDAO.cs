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
                        .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetRoomsByRoomTypeIdAsync(int roomTypeId)
        {
            return await _context.Rooms
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
                .Where(r => r.isActive == true && r.isUsed == false) // Phòng chưa bị chủ khóa và chưa có khách
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
               .SingleOrDefaultAsync(c => c.RoomID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

        public async Task<Room?> ChangeRoomStatusAsync(int roomId, bool? isUsed, bool? isActive)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room != null)
            {
                if (isUsed.HasValue) room.isUsed = isUsed.Value;
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
                .Where(r => r.isActive == true && r.isUsed == false)
                .ToListAsync();
            Console.WriteLine($"Rooms before filter: {string.Join(", ", roomsBeforeFilter.Select(r => r.RoomID))}");

            // Thực hiện lọc phòng
            var availableRooms = await _context.Rooms
                .Where(r => r.RoomTypesID == roomTypeId)
                .Where(r => r.isActive == true && r.isUsed == false)
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
                .ToListAsync();

            // Log danh sách phòng sau khi lọc
            Console.WriteLine($"Rooms after filter: {string.Join(", ", availableRooms.Select(r => r.RoomID))}");

            return availableRooms;
        }
    }
}
