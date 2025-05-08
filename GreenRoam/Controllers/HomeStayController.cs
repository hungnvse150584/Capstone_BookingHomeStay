using BusinessObject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Request.Pricing;
using Service.RequestAndResponse.Response.Accounts;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.ImageHomeStay;
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

        //[Authorize(Roles = "Admin, Owner")]
        [HttpGet]
        [Route("GetAllRegisterHomeStay")]
        public async Task<ActionResult<BaseResponse<IEnumerable<HomeStayResponse>>>> GetAllHomeStayRegister()
        {
            var homeStays = await _homestayService.GetAllHomeStayRegisterFromBase();
            return Ok(homeStays);
        }


        //[Authorize(Roles = "Admin, Owner")]
        [HttpGet]
        [Route("GetAllHomeStayWithOwnerName")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllHomeStayWithOwnerName>>>> GetAllHomeStayWithOwnerName()
        {
            var homeStays = await _homestayService.GetAllHomeStayWithOwnerName();
            return Ok(homeStays);
        }


        //[Authorize(Roles = "Admin, Owner")]
        [HttpGet]
        [Route("GetHomeStayDetail/{id}")]
        public async Task<ActionResult<BaseResponse<SimpleHomeStayResponse>>> GetHomeStayDetailById(int id)
        {
            if (id == 0 || id == null)
            {
                return BadRequest("Please Input Id!");
            }
            return await _homestayService.GetHomeStayDetailByIdFromBase(id);
        }


        //[Authorize(Roles = "Admin, Owner")]
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


        //[Authorize(Roles = "Admin, Owner, Staff, Customer")]
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

        //[Authorize(Roles = "Customer")]
        [HttpGet]
        [Route("GetNearestHomeStay")]
        public async Task<ActionResult<BaseResponse<IEnumerable<SimpleHomeStayResponse>>>> GetNearestHomeStays(double userLat, double userLon, int pageIndex = 1, int pageSize = 5)
        {
            if(userLat <= 0 || userLon <= 0)
            {
                return BadRequest("Please Input userLat, userLon!");
            }
            var response = await _homestayService.GetNearestHomeStays(userLat, userLon, pageIndex, pageSize);
            return Ok(response);
        }

        //[Authorize(Roles = "Admin, Owner")]
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

        //[Authorize(Roles = "Owner")]
        [HttpPost]
        [Route("CreateWithRentalsAndPricing")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>>> CreateHomeStayWithRentalsAndPricing([FromForm] CreateHomeStayWithRentalsAndPricingRequest request)
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

                var homeStays = await _homestayService.CreateHomeStayWithRentalsAndPricingAsync(request);
                return StatusCode((int)homeStays.StatusCode, homeStays);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse<List<HomeStay>>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null));
            }
        }

        //[Authorize(Roles = "Owner")]
        [HttpPut]
        [Route("UpdateHomeStay/{homestayId}")]
        public async Task<ActionResult<BaseResponse<HomeStay>>> UpdateHomeStay(int homestayId, [FromBody] UpdateHomeStayRequest request)
        {
            if (homestayId <= 0)
            {
                return BadRequest(new BaseResponse<HomeStay>("Invalid HomeStay ID.", StatusCodeEnum.BadRequest_400, null));
            }

            if (request == null)
            {
                return BadRequest(new BaseResponse<HomeStay>("Request body cannot be null.", StatusCodeEnum.BadRequest_400, null));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponse<HomeStay>("Invalid request data.", StatusCodeEnum.BadRequest_400, null));
            }

            var result = await _homestayService.UpdateHomeStay(homestayId, request);
            return StatusCode((int)result.StatusCode, result);
        }

        //[Authorize(Roles = "Owner, Staff")]
        [HttpPut("UpdateImages/{homeStayId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse<ImageHomeStayResponse>>> UpdateHomeStayImages(
            [FromRoute] int homeStayId,
            [FromForm] UpdateHomeStayImagesBodyRequest request)
        {
            // Kiểm tra HomeStayID hợp lệ
            if (homeStayId <= 0)
            {
                return BadRequest(new BaseResponse<ImageHomeStayResponse>(
                    "Invalid HomeStay ID.",
                    StatusCodeEnum.BadRequest_400,
                    null));
            }

            // Gọi service để xử lý cập nhật hình ảnh
            var result = await _homestayService.UpdateHomeStayImages(homeStayId, request);

            // Trả về kết quả với mã trạng thái tương ứng
            return StatusCode((int)result.StatusCode, result);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("ChangeHomeStayStatus")]
        public async Task<ActionResult<BaseResponse<HomeStayResponse>>> ChangeHomeStayStatus(int homestayId, HomeStayStatus status, int? commissionRateID = null)
        {
            if (homestayId <= 0)
            {
                return BadRequest("Invalid HomeStay ID.");
            }
            return await _homestayService.ChangeHomeStayStatus(homestayId, status, commissionRateID);
        }

        //[Authorize(Roles = "Owner")]
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

        //[Authorize(Roles = "Customer")]
        [HttpGet("filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BaseResponse<IEnumerable<HomeStayResponse>>>> FilterHomeStays([FromQuery] FilterHomeStayRequest request)
        {
            var result = await _homestayService.FilterHomeStaysAsync(request);
            return StatusCode((int)result.StatusCode, result);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("adminDashBoard/GetOwnersWithHomeStayStats")]
        public async Task<BaseResponse<List<GetOwnerUser>>> GetOwnersWithHomeStayStats()
        {
            return await _homestayService.GetOwnersWithHomeStayStats();
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet("GetTrendingHomeStays")]
        public async Task<BaseResponse<List<GetTrendingHomeStay>>> GetTrendingHomeStays(int top = 10)
        {
            return await _homestayService.GetTrendingHomeStays(top);
        }
    }
}
