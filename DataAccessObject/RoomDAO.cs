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
            if(checkInDate >= checkOutDate)
            {
                throw new ArgumentException($"{checkOutDate} must be after {checkInDate}.");
            }

            var availableRooms = await _context.Rooms
             .Where(r => r.isActive == true && r.isUsed == false) // Phòng chưa bị chủ khóa và chưa có khách
             .Where(r => !_context.BookingDetails
             .Any(bd => bd.RoomID == r.RoomID &&
                  bd.Booking.paymentStatus == PaymentStatus.Deposited &&
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
    }
}
