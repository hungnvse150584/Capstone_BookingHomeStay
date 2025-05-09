using BusinessObject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Staffs;
using Service.RequestAndResponse.Response.Staffs;
using Service.Service;

namespace GreenRoam.Controllers
{
    [Route("api/Manage-Staff")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpGet]
        [Route("GetAllStaffsByHomeStay/{homeStayID}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllStaff>>>> GetAllStaffByHomeStay(int homeStayID)
        {
            var staffs = await _staffService.GetAllStaffByHomeStay(homeStayID);
            return Ok(staffs);
        }

        [Authorize(Roles = "Owner")]
        [HttpGet]
        [Route("GetAllStaffsByOwner/{accountID}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllStaff>>>> GetAllStaffByOwner(string accountID)
        {
            var staffs = await _staffService.GetAllStaffByOwner(accountID);
            return Ok(staffs);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpGet]
        [Route("GetStaffsByID/{accountID}")]
        public async Task<ActionResult<BaseResponse<GetAllStaff>>> GetStaffByID(string accountID)
        {
            if (string.IsNullOrEmpty(accountID))
            {
                return BadRequest("Please Input userId!");
            }
            return await _staffService.GetStaffByID(accountID);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost]
        [Route("CreateStaffAccount")]
        public async Task<ActionResult<BaseResponse<Staff>>> CreateStaffAccount(CreateStaffRequest request)
        {
            if (request == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var staff = await _staffService.CreateStaffAccount(request);
            return staff;
        }

        [Authorize(Roles = "Owner")]
        [HttpPut]
        [Route("UpdateStaffAccount/{userId}")]
        public async Task<ActionResult<BaseResponse<Staff>>> UpdateStaffAccount(string userId, UpdateStaffRequest request)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new BaseResponse<Staff>("Invalid userId.", StatusCodeEnum.BadRequest_400, null));
            }

            if (request == null)
            {
                return BadRequest(new BaseResponse<Staff>("Request body cannot be null.", StatusCodeEnum.BadRequest_400, null));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponse<Staff>("Invalid request data.", StatusCodeEnum.BadRequest_400, null));
            }

            var result = await _staffService.UpdateStaffAccount(userId, request);
            return StatusCode((int)result.StatusCode, result);
        }
    } 
}
