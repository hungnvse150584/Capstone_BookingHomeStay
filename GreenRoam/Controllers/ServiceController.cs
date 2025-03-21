using BusinessObject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.Services;
using Service.Service;

namespace GreenRoam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceServices _servicesService;
        public ServiceController(IServiceServices servicesService)
        {
            _servicesService = servicesService;
        }

        [HttpGet]
        [Route("GetAllServices")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllServices>>>> GetAllServices()
        {
            var services = await _servicesService.GetAllServices();
            return Ok(services);
        }

        [HttpPost]
        [Route("CreateService")]
        public async Task<ActionResult<BaseResponse<List<Services>>>> CreateServices([FromForm] CreateServices request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new BaseResponse<List<Services>>("Request body cannot be null!", StatusCodeEnum.BadRequest_400, null));
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new BaseResponse<List<Services>>("Invalid request data!", StatusCodeEnum.BadRequest_400, null));
                }

                var service = await _servicesService.CreateService(request);
                return StatusCode((int)service.StatusCode, service);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse<List<Services>>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null));
            }
        }
    }
}
