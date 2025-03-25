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
    public class ImageHomeStayDAO : BaseDAO<ImageHomeStay>
    {
        private readonly GreenRoamContext _context;
        public ImageHomeStayDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }
        public async Task<ImageHomeStay> AddImageAsync(ImageHomeStay image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image), "ImageHomeStay cannot be null");
            }

            if (image.HomeStayID <= 0)
            {
                throw new ArgumentException("HomeStayID must be a positive value", nameof(image.HomeStayID));
            }

            // Kiểm tra xem HomeStayID có tồn tại không (tùy chọn)
            var homeStayExists = await _context.HomeStays.AnyAsync(h => h.HomeStayID == image.HomeStayID);
            if (!homeStayExists)
            {
                throw new ArgumentException($"HomeStay with ID {image.HomeStayID} does not exist");
            }

            _context.ImageHomeStays.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        // Lấy danh sách ảnh theo HomeStayID
        public async Task<IEnumerable<ImageHomeStay>> GetImagesByHomeStayIdAsync(int homeStayId)
        {
            if (homeStayId <= 0)
            {
                throw new ArgumentException("HomeStayID must be a positive value", nameof(homeStayId));
            }

            return await _context.ImageHomeStays
                .Where(i => i.HomeStayID == homeStayId)
                .Include(i => i.HomeStay) // Bao gồm thông tin HomeStay nếu cần
                .ToListAsync();
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        // Lấy một ảnh theo ID
        public async Task<ImageHomeStay> GetImageByIdAsync(int imageId)
        {
            if (imageId <= 0)
            {
                throw new ArgumentException("ImageHomeStayID must be a positive value", nameof(imageId));
            }

            var image = await _context.ImageHomeStays
                .Include(i => i.HomeStay) // Bao gồm thông tin HomeStay nếu cần
                .SingleOrDefaultAsync(i => i.ImageHomeStayID == imageId);

            if (image == null)
            {
                throw new ArgumentNullException(nameof(imageId), $"ImageHomeStay with ID {imageId} not found");
            }

            return image;
        }
        public async Task<ImageHomeStay> UpdateImageAsync(ImageHomeStay image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image), "ImageHomeStay cannot be null");
            }

            if (image.ImageHomeStayID <= 0)
            {
                throw new ArgumentException("ImageHomeStayID must be a positive value", nameof(image.ImageHomeStayID));
            }

            var existingImage = await _context.ImageHomeStays
                .FirstOrDefaultAsync(i => i.ImageHomeStayID == image.ImageHomeStayID);

            if (existingImage == null)
            {
                throw new ArgumentException($"ImageHomeStay with ID {image.ImageHomeStayID} not found");
            }

          
            existingImage.Image = image.Image;
            existingImage.HomeStayID = image.HomeStayID;

          
            _context.ImageHomeStays.Update(existingImage);
            await _context.SaveChangesAsync();

            return existingImage;
        }

    }
}
