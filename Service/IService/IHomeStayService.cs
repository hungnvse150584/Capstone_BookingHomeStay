using BusinessObject.Model;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Response.Accounts;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.ImageHomeStay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IHomeStayService
    {
        Task<BaseResponse<List<HomeStay>>> RegisterHomeStay(CreateHomeStayRequest request);
        Task<BaseResponse<HomeStay>> UpdateHomeStay(int homestayId, UpdateHomeStayRequest request);
        Task<BaseResponse<HomeStayResponse>> ChangeHomeStayStatus(int homestayId, HomeStayStatus status, int? commissionRateID = null);
        Task<BaseResponse<IEnumerable<HomeStayResponse>>> GetAllHomeStayRegisterFromBase();
        Task<BaseResponse<IEnumerable<GetAllHomeStayWithOwnerName>>> GetAllHomeStayWithOwnerName();
        Task<BaseResponse<SimpleHomeStayResponse>> GetHomeStayDetailByIdFromBase(int id);
        Task<BaseResponse<HomeStayResponse>> GetOwnerHomeStayByIdFromBase(string accountId);
        Task<BaseResponse<IEnumerable<SimpleHomeStayResponse>>> GetSimpleHomeStaysByAccount(string accountId);
        Task<BaseResponse<string>> DeleteHomeStay(int id);
        Task<BaseResponse<ImageHomeStayResponse>> UpdateHomeStayImages(int homeStayId, UpdateHomeStayImagesBodyRequest request);
        Task<BaseResponse<IEnumerable<HomeStayResponse>>> FilterHomeStaysAsync(FilterHomeStayRequest request);

        /*Task<BaseResponse<IEnumerable<SimpleHomeStayResponse>>> GetNearestHomeStays(double userLat, double userLon, int topN = 5);*/
        Task<BaseResponse<IEnumerable<SimpleHomeStayResponse>>> GetNearestHomeStays(double userLat, double userLon, int pageIndex = 1, int pageSize = 5);
        Task<BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>> CreateHomeStayWithRentalsAndPricingAsync(CreateHomeStayWithRentalsAndPricingRequest request);
        Task<BaseResponse<List<GetOwnerUser>>> GetOwnersWithHomeStayStats();
    }
}
