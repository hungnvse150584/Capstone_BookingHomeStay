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
    public class HomeStayTypeDAO : BaseDAO<HomeStayTypes>
    {
        private readonly GreenRoamContext _context;
        public HomeStayTypeDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<HomeStayTypes>> GetAllHomeStayTypesAsync(int homestayId)
        {
            return await _context.HomeStayTypes
                        .Where(c => c.HomeStayID == homestayId)  
                        .Include(c => c.Rooms)                   
                        .Include(c => c.Property)                
                        .Include(c => c.ImageHomeStayTypes)      
                        .ToListAsync();                          
        }


        public async Task<HomeStayTypes> GetHomeStayTypeByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<HomeStayTypes>()
                        .Include(c => c.Rooms)
                        .Include(c => c.Property)
                        .Include(c => c.ImageHomeStayTypes)
               .SingleOrDefaultAsync(c => c.HomeStayTypesID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

    }
}
