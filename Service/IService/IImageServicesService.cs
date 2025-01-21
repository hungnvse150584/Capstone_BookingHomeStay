using BusinessObject.Model;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.ImageService;
using Service.RequestAndResponse.Response.ImageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IImageServicesService
    {
        Task<BaseResponse<IEnumerable<GetAllImageService>>> GetAllImageServices();
        Task<BaseResponse<GetAllImageService>> GetImageServiceById(int id);
        Task<BaseResponse<AddImageServicesRequest>> CreateImageService(AddImageServicesRequest imageServiceRequest);
        Task<BaseResponse<UpdateImageServicesRequest>> UpdateImageService(int id, UpdateImageServicesRequest imageServiceRequest);
        Task<BaseResponse<string>> DeleteImageService(int id);
    }
}
