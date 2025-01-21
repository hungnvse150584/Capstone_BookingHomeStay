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
    public class ImageServicesDAO : BaseDAO<ImageServices>
    {
        private readonly GreenRoamContext _context;

        public ImageServicesDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ImageServices>> GetAllByServiceIdAsync(int serviceId)
        {
            return await _context.ImageServices
                        .Where(i => i.ServicesID == serviceId)
                        .ToListAsync();
        }

        public async Task<ImageServices> GetImageServiceByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.ImageServices
                                       .SingleOrDefaultAsync(i => i.ImageServicesID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

    }
}
