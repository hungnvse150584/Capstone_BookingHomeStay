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
    public class ImageHomeStayRentalsDAO : BaseDAO<ImageHomeStayRentals>
    {
        private readonly GreenRoamContext _context;

        public ImageHomeStayRentalsDAO(GreenRoamContext context) : base(context) 
        {
            _context = context;
        }
        public async Task<IEnumerable<ImageHomeStayRentals>> GetAllByImageIdAsync(int imageId)
        {
            return await _context.ImageHomeStayRentals
                        .Where(i => i.HomeStayRentalID == imageId)
                        .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<ImageHomeStayRentals> GetImageHomeStayTypesByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.ImageHomeStayRentals
                                       .SingleOrDefaultAsync(i => i.ImageHomeStayRentalsID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

        
    }
}
