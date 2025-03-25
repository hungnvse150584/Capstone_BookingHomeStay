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
    }
}
