using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.ImageService;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.ImageService;
using Service.RequestAndResponse.Response.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IServiceServices
    {
        Task<BaseResponse<IEnumerable<GetAllServices>>> GetAllServices();
        Task<BaseResponse<GetAllServices>> GetServiceById(int id);
        Task<BaseResponse<CreateServices>> CreateService(CreateServices serviceRequest);
        Task<BaseResponse<UpdateServices>> UpdateService(int id, UpdateServices serviceRequest);
        Task<BaseResponse<string>> DeleteService(int id);
    }
}
