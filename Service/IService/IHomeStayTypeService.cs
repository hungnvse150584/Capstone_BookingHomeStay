using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.HomeStayType;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IHomeStayTypeService
    {
        Task<BaseResponse<IEnumerable<GetAllHomeStayType>>> GetAllHomeStayTypes();

        Task<BaseResponse<CreateHomeStayTypeRequest>> CreateHomeStayType(CreateHomeStayTypeRequest typeRequest);

        Task<BaseResponse<IEnumerable<GetAllServices>>> GetAllServices();

        Task<BaseResponse<CreateServices>> CreateServices(CreateServices servicesRequest);
        Task<BaseResponse<string>> DeleteHomeStayRental(int id);
    }
}
