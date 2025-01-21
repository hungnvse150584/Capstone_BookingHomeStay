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
    public class RoomsDAO : BaseDAO<Room>
    {
        private readonly GreenRoamContext _context;
        public RoomsDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync()
        {
            return await _context.Rooms
           .Where(r => r.Status.Equals(true))
           .ToListAsync();
        }

        



        //OwnerDASHBOARD
        public async Task<IEnumerable<Room>> GetRoomsWithBookingsAsync()
        {
            return await _context.Rooms
           .ToListAsync();
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms
           .ToListAsync();
        }
    }
}
