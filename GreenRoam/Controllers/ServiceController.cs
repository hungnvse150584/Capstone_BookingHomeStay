using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.ImageHomeStayTypes;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.ImageHomeStayTypes;
using Service.RequestAndResponse.Response.Services;

namespace GreenRoam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceServices _serviceServices;
        public ServiceController(IServiceServices serviceServices)
        {
            _serviceServices = serviceServices;
        }
        [HttpGet]
        [Route("GetAllServices")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllServices>>>> GetAllServices()
        {
            var service = await _serviceServices.GetAllServices();
            return Ok(service);
        }
        [HttpGet]
        [Route("GetServices/{id}")]
        public async Task<ActionResult<BaseResponse<GetAllServices>>> GetAllServicesById(int id)
        {
            if (id == 0 || id == null)
            {
                return BadRequest("Please provide a valid Id.");
            }
            return await _serviceServices.GetServiceById(id);
        }

        [HttpPost]
        [Route("CreateService")]
        public async Task<ActionResult<BaseResponse<CreateServices>>> CreateService([FromBody] CreateServices createServices)
        {
            if (createServices == null)
            {
                return BadRequest("Please provide all required information.");
            }
            var service = await _serviceServices.CreateService(createServices);
            return CreatedAtAction(nameof(GetAllServicesById), new { id = service.Data.ServicesID }, service);
        }
        [HttpPut]
        [Route("UpdateService/{id}")]
        public async Task<ActionResult<BaseResponse<UpdateServices>>> UpdateService(int id, [FromBody] UpdateServices updateServices)
        {
            if (id <= 0)
            {
                return BadRequest("Please provide a valid Id.");
            }
            return await _serviceServices.UpdateService(id, updateServices);
        }
        [HttpDelete]
        [Route("DeleteImageService/{id}")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteService(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Please provide a valid Id.");
            }
            return await _serviceServices.DeleteService(id);
        }
    }
}
