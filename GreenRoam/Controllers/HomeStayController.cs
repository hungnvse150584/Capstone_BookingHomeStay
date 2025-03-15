using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Response.HomeStays;
using Service.Service;

namespace GreenRoam.Controllers
{
    [Route("api/homestay")]
    [ApiController]
    public class HomeStayController : ControllerBase
    {
        private readonly IHomeStayService _homestayService;
        public HomeStayController(IHomeStayService homestayService)
        {
            _homestayService = homestayService;
        }

        [HttpGet]
        [Route("GetAllRegisterHomeStay")]
        public async Task<ActionResult<BaseResponse<IEnumerable<HomeStayResponse>>>> GetAllHomeStayRegister()
        {
            var homeStays = await _homestayService.GetAllHomeStayRegisterFromBase();
            return Ok(homeStays);
        }

        [HttpGet]
        [Route("GetRegisterHomeStay/{id}")]
        public async Task<ActionResult<BaseResponse<HomeStayResponse>>> GetHomeStayDetailById(int id)
        {
            if (id == 0 || id == null)
            {
                return BadRequest("Please Input Id!");
            }
            return await _homestayService.GetHomeStayDetailByIdFromBase(id);
        }

        [HttpGet]
        [Route("GetOwnerHomeStay/{accountId}")]
        public async Task<ActionResult<BaseResponse<HomeStayResponse>>> GetOwnerHomeStayById(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                return BadRequest("Please Input Id!");
            }
            return await _homestayService.GetOwnerHomeStayByIdFromBase(accountId);
        }

        [HttpPost]
        [Route("CreateHomeStay")]
        public async Task<ActionResult<BaseResponse<List<HomeStay>>>> RegisterHomeStay(CreateHomeStayRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            var homeStays = await _homestayService.RegisterHomeStay(request);

            if (homeStays == null)
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
            
            return Ok(homeStays);
        }

        [HttpPut]
        [Route("UpdateHomeStay")]
        public async Task<ActionResult<BaseResponse<HomeStay>>> UpdateHomeStay(int homestayId, UpdateHomeStayRequest request)
        {
            if (homestayId <= 0)
            {
                return BadRequest("Invalid HomeStay ID.");
            }

            if (request == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _homestayService.UpdateHomeStay(homestayId, request);

            if (result == null)
            {
                return StatusCode(500, "An error occurred while updating the HomeStay.");
            }

            return Ok(result);
        }

        [HttpPut]
        [Route("ChangeHomeStayStatus")]
        public async Task<ActionResult<BaseResponse<HomeStayResponse>>> ChangeHomeStayStatus(int homestayId, HomeStayStatus status)
        {
            if (homestayId <= 0)
            {
                return BadRequest("Invalid HomeStay ID.");
            }
            return await _homestayService.ChangeHomeStayStatus(homestayId, status);
        }
    }
}
