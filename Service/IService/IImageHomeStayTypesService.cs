using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.ImageHomeStayTypes;
using Service.RequestAndResponse.Request.ImageService;
using Service.RequestAndResponse.Response.ImageHomeStayTypes;
using Service.RequestAndResponse.Response.ImageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IImageHomeStayTypesService
    {
        Task<BaseResponse<IEnumerable<GetAllImageHomeStayType>>> GetAllImageHomeStayTypes();
        Task<BaseResponse<GetAllImageHomeStayType>> GetImageHomeStayTypesById(int id);
        Task<BaseResponse<AddImageHomeStayTypesRequest>> CreateImageHomeStayTypes(AddImageHomeStayTypesRequest imageTypesRequest);
        Task<BaseResponse<UpdateImageHomeStayTypesRequest>> UpdateImageHomeStayTypes(int id, UpdateImageHomeStayTypesRequest imageTypesRequest);
        Task<BaseResponse<string>> DeleteImageHomeStayTypes(int id);
    }
}
