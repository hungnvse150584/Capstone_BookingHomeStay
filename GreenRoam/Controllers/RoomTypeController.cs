using BusinessObject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.RoomType;

namespace GreenRoam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeService _roomTypeService;

        public RoomTypeController(IRoomTypeService roomTypeService)
        {
            _roomTypeService = roomTypeService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<BaseResponse<RoomTypes>>> CreateRoomType([FromBody] CreateRoomTypeRequest request, [FromQuery] int homeStayRentalId)
        {
            var result = await _roomTypeService.CreateRoomType(request, homeStayRentalId);
            if (result.StatusCode == StatusCodeEnum.Created_201)
            {
                return StatusCode(201, result);
            }
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
