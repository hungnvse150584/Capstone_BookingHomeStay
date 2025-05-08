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
        Task<BaseResponse<(IEnumerable<CreateRatingResponse> Data, int TotalCount)>> GetRatingByHomeStayIdAsync(int homeStayId, int pageNumber = 1, int pageSize = 10);
        Task<BaseResponse<(IEnumerable<CreateRatingResponse> Data, int TotalCount)>> GetRatingByAccountIdAsync(string accountId, int pageNumber = 1, int pageSize = 10);
        Task<BaseResponse<(IEnumerable<CreateRatingResponse> Data, int TotalCount)>> GetRatingByUserIdAndHomeStayAsync(string accountId, int homeStayId, int pageNumber = 1, int pageSize = 10);
        Task<BaseResponse<double>> GetAverageRatingAsync(int homeStayId);
        Task<BaseResponse<CreateRatingResponse>> GetRatingDetailByRatingIDAsync(int ratingId);
    }
}