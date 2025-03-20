using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.CancellationPolicy;
using Service.RequestAndResponse.Request.CommissionRates;
using Service.RequestAndResponse.Response.CancellationPolicyRequest;
using Service.RequestAndResponse.Response.CommissionRate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface ICancellationPolicyService
    {
        Task<BaseResponse<IEnumerable<GetAllCancellationPolicy>>> GetAllCancellationPolicy();
        Task<BaseResponse<GetAllCancellationPolicy>> GeCancellationPolicyByHomeStay(int homeStayID);
        Task<BaseResponse<CreateCancellationPolicyRequest>> CreateCancellationPolicyRequest(CreateCancellationPolicyRequest typeRequest);
        Task<BaseResponse<UpdateCancellationPolicyRequest>> UpdateCancellationPolicyRequest(UpdateCancellationPolicyRequest typeRequest);
        Task<BaseResponse<string>> DeleteCancellationPolicy(int id);
    }
}
