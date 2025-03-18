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
    public class PricingDAO : BaseDAO<Pricing>
    {
        private readonly GreenRoamContext _context;
        public PricingDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pricing>> GetAllPricingByHomeStayAsync(int homestayID)
        {
            if (homestayID <= 0)
            {
                throw new ArgumentNullException($"id {homestayID} not found");
            }
            return await _context.Prices
               .Where(p => p.HomeStayRentals.HomeStayID == homestayID)
               .Include(p => p.HomeStayRentals)
               .Include(p => p.RoomTypes)
               .ToListAsync();
        }

        public async Task<IEnumerable<Pricing>> GetPricingByHomeStayRentalAsync(int rentalID)
        {
            if (rentalID <= 0)
            {
                throw new ArgumentNullException($"id {rentalID} not found");
            }
            return await _context.Prices
               .Where(p => p.HomeStayRentalID == rentalID)
               .Include(p => p.HomeStayRentals)
               .ToListAsync();
        }

        public async Task<IEnumerable<Pricing>> GetPricingByRoomTypeAsync(int roomTypeID)
        {
            if (roomTypeID <= 0)
            {
                throw new ArgumentNullException($"id {roomTypeID} not found");
            }
            return await _context.Prices
               .Where(p => p.RoomTypesID == roomTypeID)
               .Include(p => p.RoomTypes)
               .ToListAsync();
        }


        public async Task<Pricing> GetPricingByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<Pricing>()
               .Include(p => p.HomeStayRentals)
               .Include(p => p.RoomTypes)
               .SingleOrDefaultAsync(c => c.PricingID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }
    }
}
