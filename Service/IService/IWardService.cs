using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.District;
using Service.RequestAndResponse.Request.Ward;
using Service.RequestAndResponse.Response.Districts;
using Service.RequestAndResponse.Response.Wards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IWardService
    {
        Task<BaseResponse<IEnumerable<GetAllWard>>> GetAllWardFromBase();
        Task<BaseResponse<GetAllWard>> GetWardByIdFromBase(int id);
        Task<BaseResponse<UpdateWardRequest>> UpdateWardFromBase(int id, UpdateWardRequest wardRequest);
        Task<BaseResponse<AddWardRequest>> CreateWardFromBase(AddWardRequest wardRequest);
    }
}
