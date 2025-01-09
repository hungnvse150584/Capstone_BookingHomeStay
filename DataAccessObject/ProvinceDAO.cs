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
    public class ProvinceDAO : BaseDAO<Province>
    {
        private readonly GreenRoamContext _context;
        public ProvinceDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Province>> GetAllProvinceAsync()
        {
            return await _context.Provinces
                        .Include(c => c.Districts)
                        .Include(C => C.Locations)
                        .ToListAsync();
        }

        public async Task<Province> GetProvinceByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<Province>()
                        .Include(c => c.Districts)
                        .Include(C => C.Locations)
               .SingleOrDefaultAsync(c => c.ProvinceID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

        public async Task<string> GetProvinceNameById(int? provinceId)
        {
            // Assuming `Street` is an entity in your database context
            var province = await _context.Provinces
                                          .Where(s => s.ProvinceID == provinceId)
                                          .Select(s => s.provinceName)
                                          .FirstOrDefaultAsync();

            if (province == null)
                throw new Exception($" Province with ID {provinceId} not found.");

            return province;
        }
    }
}
