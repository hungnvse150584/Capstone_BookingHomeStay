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
    public class ImageRoomTypeDAO : BaseDAO<ImageRoomTypes>
    {
        private readonly GreenRoamContext _context;

        public ImageRoomTypeDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ImageRoomTypes>> GetAllByImageIdAsync(int imageId)
        {
            return await _context.ImageRoomTypes
                        .Where(i => i.RoomTypesID == imageId)
                        .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<ImageRoomTypes> GetImageHomeStayTypesByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentNullException($"id {id} not found");
            }
            var entity = await _context.ImageRoomTypes
                                       .SingleOrDefaultAsync(i => i.ImageRoomTypesID == id);
            if (entity == null)
            {
                throw new ArgumentNullException($"Entity with id {id} not found");
            }
            return entity;
        }
    }
}
