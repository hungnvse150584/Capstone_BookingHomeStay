using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.ImageRating;
using Service.RequestAndResponse.Response.ImageRating;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IImageRatingService
    {
        Task<BaseResponse<UploadImageRatingRequest>> UploadImageAsync(UploadImageRatingRequest request);
        Task<BaseResponse<ImageRatingResponse>> UpdateImageAsync(int imageId, UpdateImageRatingRequest request);
        Task<BaseResponse<string>> DeleteImageAsync(int imageId);
        Task<BaseResponse<IEnumerable<ImageRatingResponse>>> GetImagesByRatingIdAsync(int ratingId);
    }
}