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
    public class BookingDetailDAO : BaseDAO<BookingDetail>
    {
        private readonly GreenRoamContext _context;
        public BookingDetailDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<BookingDetail>> GetBookingDetailsToRemoveAsync(int bookingId, List<int> updatedDetailIds)
        {
            return await _context.BookingDetails
                                 .Where(d => d.BookingID == bookingId && !updatedDetailIds.Contains(d.BookingDetailID))
                                 .ToListAsync();
        }

        public async Task<List<(string roomTypeName, int count)>> GetRoomTypeUsageStatsAsync(int homestayId)
        {
            var stats = await _context.BookingDetails
                .Where(b => (b.Booking.Status == BookingStatus.Confirmed ||
                       b.Booking.Status == BookingStatus.Completed ||
                       b.Booking.Status == BookingStatus.InProgress) &&
                       b.Booking.HomeStayID == homestayId &&
                       b.RoomID != null)
                .Include(b => b.Rooms)
                    .ThenInclude(r => r.RoomTypes)
                .GroupBy(b => b.Rooms.RoomTypes.Name)
                .Select(g => new
                {
                    roomTypeName = g.Key,
                    count = g.Count()
                }).ToListAsync();

            return stats.Select(s => (s.roomTypeName, s.count)).ToList();
        }
    }
}
