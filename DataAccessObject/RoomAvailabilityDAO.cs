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
    public class RoomAvailabilityDAO : BaseDAO<RoomAvailability>
    {
        private readonly GreenRoamContext _context;
        public RoomAvailabilityDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }


        public async Task<IEnumerable<RoomAvailability>> GetAvailableRoomsAsync()
        {
            return await _context.RoomAvailabilities
           .Include(ra => ra.RoomTypes)
           .ToListAsync();
        }
    }
}
