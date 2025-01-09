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
    public class WardDAO : BaseDAO<Ward>
    {
        private readonly GreenRoamContext _context;
        public WardDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ward>> GetAllWardAsync()
        {
            return await _context.Wards
                        .Include(c => c.Streets)
                        .Include(c => c.District)
                        .Include(C => C.Locations)
                        .ToListAsync();
        }

        public async Task<Ward> GetWardByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<Ward>()
                        .Include(c => c.Streets)
                        .Include(c => c.District)
                        .Include(C => C.Locations)
               .SingleOrDefaultAsync(c => c.WardID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

        public async Task<string> GetWardNameById(int? wardId)
        {
            // Assuming `Street` is an entity in your database context
            var ward = await _context.Wards
                                          .Where(s => s.WardID == wardId)
                                          .Select(s => s.wardName)
                                          .FirstOrDefaultAsync();

            if (ward == null)
                throw new Exception($"Ward with ID {wardId} not found.");

            return ward;
        }
    }
}
