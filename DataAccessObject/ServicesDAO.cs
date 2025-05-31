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
        public async Task<IEnumerable<Services>> GetAllServiceAsync()
        {
            //return await _context.HomeStays
            //            .Include(c => c.Account)

            //            .ToListAsync();
            return await _context.Services
               .Include(s => s.HomeStay)
               .Include(h => h.BookingServicesDetails)
               .Include(h => h.ImageServices)
               .ToListAsync();
        }
        public async Task<IEnumerable<Services>> GetAllServiceAsync(int homestayId = 0)
        {
            if (homestayId == 0)
            {
                return await _context.Services
                    .Include(s => s.HomeStay)
                       .Include(h => h.BookingServicesDetails)
                       .Include(h => h.ImageServices)
                       .ToListAsync();
            }
            else
            {
                return await _context.Services
                    .Where(s => s.HomeStayID == homestayId)
                    .Include(s => s.HomeStay)
                    .Include(s => s.ImageServices)
                    .Include(s => s.BookingServicesDetails)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<Services>> GetServicesByIdsAsync(List<int> servicesIds)
        {
            return await _context.Services.Where(h => servicesIds.Contains(h.ServicesID)).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<Services> GetServiceByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.Set<Services>()
                        .Include(c => c.BookingServicesDetails)
                        .Include(c => c.ImageServices)
               .SingleOrDefaultAsync(c => c.ServicesID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }
    }
}
