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
    public class StreetDAO : BaseDAO<Street>
    {
        private readonly GreenRoamContext _context;
        public StreetDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Street>> GetAllStreetAsync()
        {
            return await _context.Streets
                        .Include(c => c.Ward)
                        .Include(C => C.Locations)
                        .ToListAsync();
        }

        public async Task<Street> GetStreetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<Street>()
                        .Include(c => c.Ward)
                        .Include(C => C.Locations)
               .SingleOrDefaultAsync(c => c.StreetID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

        public async Task<string> GetStreetNameById(int? streetId)
        {
            // Assuming `Street` is an entity in your database context
            var street = await _context.Streets
                                          .Where(s => s.StreetID == streetId)
                                          .Select(s => s.streetName)
                                          .FirstOrDefaultAsync();

            if (street == null)
                throw new Exception($"Street with ID {streetId} not found.");

            return street;
        }
    }
}
