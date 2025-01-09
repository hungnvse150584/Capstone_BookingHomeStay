using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Street;
using Service.RequestAndResponse.Request.Ward;
using Service.RequestAndResponse.Response.Streets;
using Service.RequestAndResponse.Response.Wards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IStreetService
    {
        Task<BaseResponse<IEnumerable<GetAllStreet>>> GetAllStreetFromBase();
        Task<BaseResponse<GetAllStreet>> GetStreetByIdFromBase(int id);
        Task<BaseResponse<UpdateStreetRequest>> UpdateStreetFromBase(int id, UpdateStreetRequest streetRequest);
        Task<BaseResponse<AddStreetRequest>> CreateStreetFromBase(AddStreetRequest streetRequest);
    }
}
