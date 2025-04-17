using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.Services;

namespace Service.IService;

public interface IServiceServices
{
    Task<BaseResponse<GetAllServices>> CreateService(CreateServices request);
    Task<BaseResponse<IEnumerable<GetAllServices>>> GetAllServices(int homestayId);
    Task<BaseResponse<GetAllServices>> UpdateService(int serviceId, UpdateServices request);
}