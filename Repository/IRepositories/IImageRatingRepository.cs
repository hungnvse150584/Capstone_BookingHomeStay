using BusinessObject.Model;
using Repository.IBaseRepository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository.IRepositories
{
    public interface IImageRatingRepository : IBaseRepository<ImageRating>
    {
        Task<ImageRating> AddImageAsync(ImageRating image);
        Task<IEnumerable<ImageRating>> GetImagesByRatingIdAsync(int ratingId);
        Task<ImageRating> GetImageByIdAsync(int imageId);
        Task<ImageRating> UpdateImageAsync(ImageRating image);
        Task SaveChangesAsync();
    }
}