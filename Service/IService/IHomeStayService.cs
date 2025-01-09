using BusinessObject.Model;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.Provinces;
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
        Task<BaseResponse<HomeStay>> UpdateHomeStay(int homestayId,UpdateHomeStayRequest request);
        Task<BaseResponse<HomeStayResponse>> ChangeHomeStayStatus(int homestayId, HomeStayStatus status);
        Task<BaseResponse<IEnumerable<HomeStayResponse>>> GetAllHomeStayRegisterFromBase();
        Task<BaseResponse<HomeStayResponse>> GetHomeStayDetailByIdFromBase(int id);
        Task<BaseResponse<HomeStayResponse>> GetOwnerHomeStayByIdFromBase(string accountId);
    }
}
