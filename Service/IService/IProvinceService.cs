using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Province;
using Service.RequestAndResponse.Response.Provinces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IProvinceService
    {
        Task<BaseResponse<IEnumerable<GetAllProvince>>> GetAllProvinceFromBase();
        Task<BaseResponse<GetAllProvince>> GetProvinceByIdFromBase(int id);
        Task<BaseResponse<UpdateProvinceRequest>> UpdateProvinceFromBase(int id, UpdateProvinceRequest provinceRequest);
        Task<BaseResponse<AddProvinceRequest>> CreateProvinceFromBase(AddProvinceRequest provinceRequest);
    }
}
