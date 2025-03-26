using BusinessObject.Model;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.HomeStay;
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
        Task<BaseResponse<HomeStay>> UpdateHomeStay(int homestayId,CreateHomeStayRequest request);
        Task<BaseResponse<HomeStayResponse>> ChangeHomeStayStatus(int homestayId, HomeStayStatus status, int? commissionRateID = null);
        Task<BaseResponse<IEnumerable<HomeStayResponse>>> GetAllHomeStayRegisterFromBase();
        Task<BaseResponse<IEnumerable<GetAllHomeStayWithOwnerName>>> GetAllHomeStayWithOwnerName();
        Task<BaseResponse<HomeStayResponse>> GetHomeStayDetailByIdFromBase(int id);
        Task<BaseResponse<HomeStayResponse>> GetOwnerHomeStayByIdFromBase(string accountId);
        Task<BaseResponse<IEnumerable<SimpleHomeStayResponse>>> GetSimpleHomeStaysByAccount(string accountId);
        Task<BaseResponse<string>> DeleteHomeStay(int id);
        Task<BaseResponse<ImageHomeStayResponse>> UpdateHomeStayImages(int homeStayId, UpdateHomeStayImagesBodyRequest request);
        Task<BaseResponse<IEnumerable<HomeStayResponse>>> FilterHomeStaysAsync(FilterHomeStayRequest request);
    }
}
