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
    public class DistrictDAO : BaseDAO<District>
    {
        private readonly GreenRoamContext _context;
        public DistrictDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<District>> GetAllDistrictAsync()
        {
            return await _context.Districts
                        .Include(c => c.Province)
                        .Include(c => c.Wards)
                        .Include(C => C.Locations)
                        .ToListAsync();
        }

        public async Task<District> GetDistrictByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<District>()
                        .Include(c => c.Province)
                        .Include(c => c.Wards)
                        .Include(C => C.Locations)
               .SingleOrDefaultAsync(c => c.DistrictID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

        public async Task<string> GetDistrictNameById(int? districtId)
        {
            // Assuming `Street` is an entity in your database context
            var district = await _context.Districts
                                          .Where(s => s.DistrictID == districtId)
                                          .Select(s => s.districtName)
                                          .FirstOrDefaultAsync();

            if (district == null)
                throw new Exception($" District with ID {districtId} not found.");

            return district;
        }
    }
}
