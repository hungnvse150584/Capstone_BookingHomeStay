using BusinessObject.Model;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.HomeStays;
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
        Task<BaseResponse<ServiceWithTotalPriceResponse>> CreateService(CreateServices request);
        Task<BaseResponse<IEnumerable<ServiceWithTotalPriceResponse>>> GetAllServices(int homestayId);
        Task<BaseResponse<ServiceWithTotalPriceResponse>> UpdateService(int serviceId, UpdateServices request);
        //Task<BaseResponse<Services>> UpdateServiceByHomeStayId(int homeStayId, UpdateServices request);
    }
}
