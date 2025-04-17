using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.Services;

namespace GreenRoam.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServiceController : ControllerBase
{
    private readonly IServiceServices _servicesService;

    public ServiceController(IServiceServices servicesService)
    {
        _servicesService = servicesService;
    }

    [HttpGet("GetAllServices/{homestayId}")]
    public async Task<ActionResult<BaseResponse<IEnumerable<GetAllServices>>>> GetAllServicesByHomeStayId(int homestayId)
    {
        var result = await _servicesService.GetAllServices(homestayId);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    [Route("CreateService")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BaseResponse<Services>>> CreateServices([FromForm] CreateServices request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(new BaseResponse<Services>("Request body cannot be null!", StatusCodeEnum.BadRequest_400, null));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponse<Services>("Invalid request data!", StatusCodeEnum.BadRequest_400, null));
            }

            var service = await _servicesService.CreateService(request);
            return StatusCode((int)service.StatusCode, service);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new BaseResponse<Services>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null));
        }
    }

    [HttpPut("UpdateService/{serviceId}")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BaseResponse<Services>>> UpdateService([FromRoute] int serviceId, [FromForm] UpdateServices request)
    {
        if (serviceId <= 0)
        {
            return BadRequest(new BaseResponse<Services>("Invalid Service ID.", StatusCodeEnum.BadRequest_400, null));
        }

        if (request == null)
        {
            return BadRequest(new BaseResponse<Services>("Request body cannot be null.", StatusCodeEnum.BadRequest_400, null));
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new BaseResponse<Services>("Invalid request data!", StatusCodeEnum.BadRequest_400, null));
        }

        var result = await _servicesService.UpdateService(serviceId, request);

        if (result == null)
        {
            return StatusCode(500, new BaseResponse<Services>("An error occurred while updating the Service.", StatusCodeEnum.InternalServerError_500, null));
        }

        return StatusCode((int)result.StatusCode, result);
    }
}