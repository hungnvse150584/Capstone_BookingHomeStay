using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
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
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllHomeStayType>>>> GetAllHomeStayTypes()
        {
            var homeStays = await _homeStayTypeService.GetAllHomeStayTypes();
            return Ok(homeStays);
        }

        [HttpPost]
        [Route("CreateHomeStayType")]
        public async Task<ActionResult<BaseResponse<CreateHomeStayTypeRequest>>> CreateHomeStayType([FromBody] CreateHomeStayTypeRequest typeRequest)
        {
            if (typeRequest == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var homeStayType = await _homeStayTypeService.CreateHomeStayType(typeRequest);
            return homeStayType;
        }

        [HttpGet]
        [Route("GetAllServices")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllServices>>>> GetAllServices()
        {
            var services = await _homeStayTypeService.GetAllServices();
            return Ok(services);
        }

        [HttpPost]
        [Route("CreateService")]
        public async Task<ActionResult<BaseResponse<CreateServices>>> CreateService([FromBody] CreateServices serviceRequest)
        {
            if (serviceRequest == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var services = await _homeStayTypeService.CreateServices(serviceRequest);
            return services;
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
    }
}
