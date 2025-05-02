using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Rating;
using Service.RequestAndResponse.Response.Ratings;

namespace Service.IService
{
    public interface IRatingService
    {
        Task<BaseResponse<CreateRatingResponse>> CreateRatingAsync(CreateRatingRequest request);
        Task<BaseResponse<CreateRatingResponse>> UpdateRatingAsync(int ratingId, UpdateRatingRequest request);
        Task<BaseResponse<string>> DeleteRatingAsync(int ratingId);
        Task<BaseResponse<IEnumerable<CreateRatingResponse>>> GetRatingByHomeStayIdAsync(int homeStayId);
        Task<BaseResponse<IEnumerable<CreateRatingResponse>>> GetRatingByAccountIdAsync(string accountId);
        Task<BaseResponse<CreateRatingResponse>> GetRatingByUserIdAndHomeStayAsync(string accountId, int homeStayId);
        Task<BaseResponse<double>> GetAverageRatingAsync(int homeStayId);
    }
}