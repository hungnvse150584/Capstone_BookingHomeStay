using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
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

        [HttpGet]
        [Route("GetAllStaffsByHomeStay/{homeStayID}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllStaff>>>> GetAllStaffByHomeStay(int homeStayID)
        {
            var staffs = await _staffService.GetAllStaffByHomeStay(homeStayID);
            return Ok(staffs);
        }
        [HttpGet]
        [Route("GetAllStaffsByOwner/{accountId}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllStaff>>>> GetAllStaffByOwner(string accountID)
        {
            var staffs = await _staffService.GetAllStaffByOwner(accountID);
            return Ok(staffs);
        }

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
    } 
}
