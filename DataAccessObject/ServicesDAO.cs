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
    public class ServicesDAO : BaseDAO<Services>
    {
        private readonly GreenRoamContext _context;
        public ServicesDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Services>> GetAllServicesAsync(int homestayId)
        {
            return await _context.Services
                        .Where(s => s.HomeStayID == homestayId)
                        .Include(s => s.ImageServices)
                        .Include(s => s.BookingServicesDetails)
                        .ToListAsync();
        }

        public async Task<Services> GetServiceByIdAsync(int? id)
        {
            if (id == null || id <= 0)
            {
                throw new ArgumentNullException($"Invalid id: {id}");
            }

            var entity = await _context.Services
                        .Include(s => s.ImageServices)
                        .Include(s => s.BookingServicesDetails)
                        .SingleOrDefaultAsync(s => s.ServicesID == id);

            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }
    }
}
