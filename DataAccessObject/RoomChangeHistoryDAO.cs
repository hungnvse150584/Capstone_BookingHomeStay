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
    public class RoomChangeHistoryDAO : BaseDAO<RoomChangeHistory>
    {
        private readonly GreenRoamContext _context;
        public RoomChangeHistoryDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomChangeHistory>> GetRoomsByBookingDetailIdAsync(int bookingDetailId)
        {
            return await _context.RoomChangeHistories
                .Where(r => r.BookingDetailID == bookingDetailId)
                .ToListAsync();
        }

        public async Task AddRoomHistoryAsync(RoomChangeHistory room)
        {
            await _context.RoomChangeHistories.AddAsync(room);
            await _context.SaveChangesAsync();
        }
    }
}
