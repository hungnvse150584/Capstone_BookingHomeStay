using BusinessObject.Model;
using DataAccessObject.BaseDAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessObject
{
    public class ImageRatingDAO : BaseDAO<ImageRating>
    {
        private readonly GreenRoamContext _context;

        public ImageRatingDAO(GreenRoamContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ImageRating> AddImageAsync(ImageRating image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image), "ImageRating cannot be null");
            if (image.RatingID <= 0) throw new ArgumentException("RatingID must be a positive value", nameof(image.RatingID));

            var ratingExists = await _context.Rating.AnyAsync(r => r.RatingID == image.RatingID);
            if (!ratingExists) throw new ArgumentException($"Rating with ID {image.RatingID} does not exist");

            _context.ImageRatings.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<IEnumerable<ImageRating>> GetImagesByRatingIdAsync(int ratingId)
        {
            if (ratingId <= 0) throw new ArgumentException("RatingID must be a positive value", nameof(ratingId));
            return await _context.ImageRatings.Where(i => i.RatingID == ratingId).Include(i => i.Ratings).ToListAsync();
        }

        public async Task<ImageRating> GetImageByIdAsync(int imageId)
        {
            if (imageId <= 0) throw new ArgumentException("ImageRatingID must be a positive value", nameof(imageId));
            var image = await _context.ImageRatings.Include(i => i.Ratings).SingleOrDefaultAsync(i => i.ImageRatingID == imageId);
            if (image == null) throw new ArgumentNullException(nameof(imageId), $"ImageRating with ID {imageId} not found");
            return image;
        }

        public async Task<ImageRating> UpdateImageAsync(ImageRating image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image), "ImageRating cannot be null");
            if (image.ImageRatingID <= 0) throw new ArgumentException("ImageRatingID must be a positive value", nameof(image.ImageRatingID));

            var existingImage = await _context.ImageRatings.FirstOrDefaultAsync(i => i.ImageRatingID == image.ImageRatingID);
            if (existingImage == null) throw new ArgumentException($"ImageRating with ID {image.ImageRatingID} not found");

            existingImage.Image = image.Image;
            existingImage.RatingID = image.RatingID;
            _context.ImageRatings.Update(existingImage);
            await _context.SaveChangesAsync();
            return existingImage;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}