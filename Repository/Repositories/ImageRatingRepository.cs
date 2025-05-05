using BusinessObject.Model;
using DataAccessObject;
using Repository.BaseRepository;
using Repository.IRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class ImageRatingRepository : BaseRepository<ImageRating>, IImageRatingRepository
    {
        private readonly ImageRatingDAO _imageRatingDao;

        public ImageRatingRepository(ImageRatingDAO imageRatingDao) : base(imageRatingDao)
        {
            _imageRatingDao = imageRatingDao;
        }

        public async Task<ImageRating> AddImageAsync(ImageRating image)
        {
            return await _imageRatingDao.AddImageAsync(image);
        }

        public async Task<IEnumerable<ImageRating>> GetImagesByRatingIdAsync(int ratingId)
        {
            return await _imageRatingDao.GetImagesByRatingIdAsync(ratingId);
        }

        public async Task<ImageRating> GetImageByIdAsync(int imageId)
        {
            return await _imageRatingDao.GetImageByIdAsync(imageId);
        }

        public async Task<ImageRating> UpdateImageAsync(ImageRating image)
        {
            return await _imageRatingDao.UpdateImageAsync(image);
        }

        public async Task SaveChangesAsync()
        {
            await _imageRatingDao.SaveChangesAsync();
        }
    }
}