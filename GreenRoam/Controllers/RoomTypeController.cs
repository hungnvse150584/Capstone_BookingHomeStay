using BusinessObject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.RoomType;
using Service.RequestAndResponse.Response.RoomType;

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
    
        [HttpGet("GetAllRoomTypeByHomeStayRentalID/{homeStayRentalId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllRoomTypeByHomeStayRentalID(int homeStayRentalId)
        {
            if (homeStayRentalId <= 0)
            {
                return BadRequest(new BaseResponse<IEnumerable<GetAllRoomType>>(
                    "Invalid HomeStayRentalID!",
                    StatusCodeEnum.BadRequest_400,
                    null));
            }

            var result = await _roomTypeService.GetAllRoomTypeByHomeStayRentalID(homeStayRentalId);
            return StatusCode((int)result.StatusCode, result);
        }
        [HttpPost("CreateRoomType")]
        public async Task<IActionResult> CreateRoomType([FromForm] CreateRoomTypeRequest request, [FromQuery] int homeStayRentalId)
        {
           
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new BaseResponse<CreateRoomTypeResponse>(
                    $"Validation errors: {string.Join(", ", errors)}",
                    StatusCodeEnum.BadRequest_400,
                    null));
            }

            var result = await _roomTypeService.CreateRoomType(request, homeStayRentalId);
            return StatusCode((int)result.StatusCode, result);
        }

    }
}
