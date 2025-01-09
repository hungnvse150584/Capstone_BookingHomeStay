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
    public class LocationDAO : BaseDAO<Location>
    {
        private readonly GreenRoamContext _context;
        public LocationDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Location>> GetAllDistrictAsync()
        {
            return await _context.Locations
                        .Include(x => x.Street)
                        .Include(c => c.Ward)
                        .Include(C => C.District)
                        .Include(c => c.Province)
                        .ToListAsync();
        }

        public async Task<Location> GetDistrictByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<Location>()
                        .Include(x => x.Street)
                        .Include(c => c.Ward)
                        .Include(C => C.District)
                        .Include(c => c.Province)
               .SingleOrDefaultAsync(c => c.LocationID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }
    }
}
