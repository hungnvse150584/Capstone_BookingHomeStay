using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.District;
using Service.RequestAndResponse.Request.Province;
using Service.RequestAndResponse.Response.Districts;
using Service.RequestAndResponse.Response.Provinces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IDistrictService
    {
        Task<BaseResponse<IEnumerable<GetAllDistrict>>> GetAllDistrictFromBase();
        Task<BaseResponse<GetAllDistrict>> GetDistrictByIdFromBase(int id);
        Task<BaseResponse<UpdateDistrictRequest>> UpdateDistrictFromBase(int id, UpdateDistrictRequest districtRequest);
        Task<BaseResponse<AddDistrictRequest>> CreateDistrictFromBase(AddDistrictRequest districtRequest);
    }
}
