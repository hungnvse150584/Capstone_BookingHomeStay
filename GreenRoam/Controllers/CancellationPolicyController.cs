using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.CancellationPolicy;
using Service.RequestAndResponse.Response.CancellationPolicyRequest;
using Service.Service;

namespace GreenRoam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancellationPolicyController : ControllerBase
    {
        private readonly ICancellationPolicyService _cancellationPolicyService;

        public CancellationPolicyController(ICancellationPolicyService cancellationPolicyService)
        {
            _cancellationPolicyService = cancellationPolicyService;
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("Create")]
        public async Task<ActionResult<BaseResponse<CreateCancellationPolicyRequest>>> CreateCancellationPolicy([FromBody] CreateCancellationPolicyRequest request)
        {
            if (request == null)
            {
                return BadRequest(new BaseResponse<CreateCancellationPolicyRequest>("Request body cannot be null!", StatusCodeEnum.BadRequest_400, null));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponse<CreateCancellationPolicyRequest>("Invalid request data!", StatusCodeEnum.BadRequest_400, null));
            }

            var response = await _cancellationPolicyService.CreateCancellationPolicyRequest(request);
            return StatusCode((int)response.StatusCode, response);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpGet("GetAll")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllCancellationPolicy>>>> GetAll()
        {
            var response = await _cancellationPolicyService.GetAllCancellationPolicy();
            return StatusCode((int)response.StatusCode, response);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<BaseResponse<GetAllCancellationPolicy>>> GetCancellationPolicyById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Please provide a valid ID.");
            }

            var response = await _cancellationPolicyService.GetCancellationPolicyByID(id);
            return StatusCode((int)response.StatusCode, response);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpGet("GetByHomeStayId/{homeStayID}")]
        public async Task<ActionResult<BaseResponse<GetAllCancellationPolicy>>> GetCancellationPolicyByHomeStay(int? homeStayID)
        {
            if (homeStayID <= 0)
            {
                return BadRequest("Please provide a valid ID.");
            }

            var response = await _cancellationPolicyService.GetCancellationPolicyByHomeStay(homeStayID);

            if (response == null)
            {
                return BadRequest("Cannot Find CancellationPolicy!");
            }

            return StatusCode((int)response.StatusCode, response);
        }

        [Authorize(Roles = "Owner")]
        [HttpPut("Update")]
        public async Task<ActionResult<BaseResponse<UpdateCancellationPolicyRequest>>> UpdateCancellationPolicy([FromBody] UpdateCancellationPolicyRequest request)
        {
            if (request == null)
            {
                return BadRequest(new BaseResponse<UpdateCancellationPolicyRequest>("Request body cannot be null!", StatusCodeEnum.BadRequest_400, null));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponse<UpdateCancellationPolicyRequest>("Invalid request data!", StatusCodeEnum.BadRequest_400, null));
            }

            var response = await _cancellationPolicyService.UpdateCancellationPolicyRequest(request);
            return StatusCode((int)response.StatusCode, response);
        }

        [Authorize(Roles = "Owner")]
        [HttpDelete]
        [Route("DeleteCancellationPolicy/{id}")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteCancellationPolicy(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Please provide a valid Id.");
            }
            return await _cancellationPolicyService.DeleteCancellationPolicy(id);
        }

    }
}
