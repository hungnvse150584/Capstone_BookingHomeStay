using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.CommissionRates;
using Service.RequestAndResponse.Request.RoomType;
using Service.RequestAndResponse.Response.CommissionRate;
using Service.RequestAndResponse.Response.RoomType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface ICommissionRateService
    {
        Task<BaseResponse<IEnumerable<GetAllCommissionRate>>> GetAllCommissionRates();
        Task<BaseResponse<GetAllCommissionRate>> GetCommissionRateByHomeStay(int homeStayID);
        Task<BaseResponse<CreateCommissionRateRequest>> CreateCommmisionRate(CreateCommissionRateRequest typeRequest);
        Task<BaseResponse<UpdateCommissionRateRequest>> UpdateCommmisionRate(UpdateCommissionRateRequest typeRequest);
        Task<BaseResponse<string>> DeleteCommissionRate(int id);

    }
}
