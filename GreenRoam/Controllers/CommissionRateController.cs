using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.CommissionRates;
using Service.RequestAndResponse.Response.CommissionRate;

namespace GreenRoam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissionRateController : ControllerBase
    {
        private readonly ICommissionRateService _commissionService;
        public CommissionRateController(ICommissionRateService commissionService)
        {
            _commissionService = commissionService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAll")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllCommissionRate>>>> GetAllCommissionRates()
        {
            var result = await _commissionService.GetAllCommissionRates();
            return StatusCode((int)result.StatusCode, result);
        }

        [Authorize(Roles = "Admin, Owner, Staff, Customer")]
        [HttpGet("GetByHomeStay/{homeStayID}")]
        public async Task<ActionResult<BaseResponse<GetAllCommissionRate>>> GetCommissionRateByHomeStay(int homeStayID)
        {
            if (homeStayID <= 0)
            {
                return BadRequest("Invalid HomeStay ID.");
            }
            var result = await _commissionService.GetCommissionRateByHomeStay(homeStayID);
            return StatusCode((int)result.StatusCode, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Create")]
        public async Task<ActionResult<BaseResponse<CreateCommissionRateRequest>>> CreateCommissionRate([FromBody] CreateCommissionRateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new BaseResponse<CreateCommissionRateRequest>("Request body cannot be null!", StatusCodeEnum.BadRequest_400, null));
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponse<CreateCommissionRateRequest>("Invalid request data!", StatusCodeEnum.BadRequest_400, null));
            }
            var result = await _commissionService.CreateCommmisionRate(request);
            return StatusCode((int)result.StatusCode, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Update")]
        public async Task<ActionResult<BaseResponse<UpdateCommissionRateRequest>>> UpdateCommissionRate([FromBody] UpdateCommissionRateRequest request)
        {
            if (request == null)
            {
                return BadRequest(new BaseResponse<UpdateCommissionRateRequest>("Request body cannot be null!", StatusCodeEnum.BadRequest_400, null));
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponse<UpdateCommissionRateRequest>("Invalid request data!", StatusCodeEnum.BadRequest_400, null));
            }
            var result = await _commissionService.UpdateCommmisionRate(request);
            return StatusCode((int)result.StatusCode, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteCommissionRate(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Please provide a valid CommissionRate ID.");
            }
            var result = await _commissionService.DeleteCommissionRate(id);
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
