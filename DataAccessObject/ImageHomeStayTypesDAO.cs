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
    public class ImageHomeStayTypesDAO : BaseDAO<ImageHomeStayTypes>
    {
        private readonly GreenRoamContext _context;

        public ImageHomeStayTypesDAO(GreenRoamContext context) : base(context) 
        {
            _context = context;
        }
        public async Task<IEnumerable<ImageHomeStayTypes>> GetAllByImageIdAsync(int imageId)
        {
            return await _context.ImageHomeStayTypes
                        .Where(i => i.HomeStayTypesID == imageId)
                        .ToListAsync();
        }


        public async Task<ImageHomeStayTypes> GetImageHomeStayTypesByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.ImageHomeStayTypes
                                       .SingleOrDefaultAsync(i => i.ImageHomeStayTypesID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }

        
    }
}
