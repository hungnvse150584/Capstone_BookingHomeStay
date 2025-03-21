using BusinessObject.Model;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Request.HomeStayType;
using Service.RequestAndResponse.Request.RoomType;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.RoomType;
using Service.RequestAndResponse.Response.Services;
using Service.Service;

namespace GreenRoam.Controllers
{
    [Route("api/homestaytype")]
    [ApiController]
    public class HomeStayTypeController : ControllerBase
    {
        private readonly IHomeStayTypeService _homeStayTypeService;
        private readonly IRoomTypeService _roomTypeService;
        
        public HomeStayTypeController(IHomeStayTypeService homeStayTypeService, IRoomTypeService roomTypeService)
        {
            _homeStayTypeService = homeStayTypeService;
            _roomTypeService = roomTypeService;
        }

        [HttpGet]
        [Route("GetAllHomeStayTypes")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllHomeStayType>>>> GetAllHomeStayTypesByHomeStayId(int homestayId)
        {
            var homeStays = await _homeStayTypeService.GetAllHomeStayTypesByHomeStayID(homestayId);
            return Ok(homeStays);
        }

        [HttpPost]
        [Route("CreateHomeStayRental")]
        public async Task<ActionResult<BaseResponse<List<HomeStayRentals>>>> CreateHomeStayRental([FromForm] CreateHomeStayTypeRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new BaseResponse<List<HomeStayRentals>>("Request body cannot be null!", StatusCodeEnum.BadRequest_400, null));
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new BaseResponse<List<HomeStayRentals>>("Invalid request data!", StatusCodeEnum.BadRequest_400, null));
                }

                var homeStays = await _homeStayTypeService.CreateHomeStayType(request);
                return StatusCode((int)homeStays.StatusCode, homeStays);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseResponse<List<HomeStayRentals>>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null));
            }
        }


        //RoomTypes
        [HttpGet]
        [Route("GetAllRoomTypes")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllRoomType>>>> GetAllRoomTypes()
        {
            var roomTypes = await _roomTypeService.GetAllRoomTypes();
            return Ok(roomTypes);
        }

        [HttpPost]
        [Route("CreateRoomType")]
        public async Task<ActionResult<BaseResponse<CreateRoomTypeRequest>>> CreateRoomType([FromBody] CreateRoomTypeRequest typeRequest)
        {
            if (typeRequest == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var roomType = await _roomTypeService.CreateRoomType(typeRequest);
            return roomType;
        }
        [HttpDelete]
        [Route("DeleteHomeStayRental/{id}")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteHomeStayRentals(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Please provide a valid Id.");
            }
            return await _homeStayTypeService.DeleteHomeStayRental(id);
        }
    }
}
