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
    public class BookingServiceDetailDAO : BaseDAO<BookingServicesDetail>
    {
        private readonly GreenRoamContext _context;
        public BookingServiceDetailDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<BookingServicesDetail>> GetServiceDetailsToRemoveAsync(int bookingId, List<int> updatedDetailIds)
        {
            return await _context.BookingServicesDetails
                                 .Where(d => d.BookingServicesID == bookingId && !updatedDetailIds.Contains(d.BookingServicesDetailID))
                                 .ToListAsync();
        }
    }
}
