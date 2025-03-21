using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
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
        [Route("GetAllHomeStayWithOwnerName")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllHomeStayWithOwnerName>>>> GetAllHomeStayWithOwnerName()
        {
            var homeStays = await _homestayService.GetAllHomeStayWithOwnerName();
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
        //GetSimple
        [HttpGet("GetSimpleByAccount/{accountId}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<SimpleHomeStayResponse>>>> GetSimpleHomeStaysByAccount(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                return BadRequest("Please provide a valid accountId.");
            }
            var response = await _homestayService.GetSimpleHomeStaysByAccount(accountId);
            return StatusCode((int)response.StatusCode, response);
        }

        //[HttpPost]
        //[Route("CreateHomeStay")]
        //public async Task<ActionResult<BaseResponse<List<HomeStay>>>> RegisterHomeStay(CreateHomeStayRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState); 
        //    }

        //    var homeStays = await _homestayService.RegisterHomeStay(request);

        //    if (homeStays == null)
        //    {
        //        return StatusCode(500, "An error occurred while processing the request.");
        //    }

        //    return Ok(homeStays);
        //}

        [HttpPost]
        [Route("CreateHomeStay")]
        public async Task<ActionResult<BaseResponse<List<HomeStay>>>> RegisterHomeStay([FromForm] CreateHomeStayRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new BaseResponse<List<HomeStay>>("Request body cannot be null!", StatusCodeEnum.BadRequest_400, null));
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new BaseResponse<List<HomeStay>>("Invalid request data!", StatusCodeEnum.BadRequest_400, null));
                }

                var homeStays = await _homestayService.RegisterHomeStay(request);
                return StatusCode((int)homeStays.StatusCode, homeStays);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse<List<HomeStay>>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null));
            }
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
        [HttpDelete]
        [Route("DeleteHomeStay/{id}")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteHomeStay(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Please provide a valid Id.");
            }
            return await _homestayService.DeleteHomeStay(id);
        }
    }
}
