using BusinessObject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Room;
using Service.RequestAndResponse.Response.Room;
using Service.Service;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GreenRoam.Controllers
{
    [Route("api/rooms")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [Authorize(Roles = "Admin, Owner, Staff")]
        [HttpGet]
        [Route("GetAllRoom")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllRooms>>>> GetAllRooms()
        {
            var rooms = await _roomService.GetAllRooms();
            return Ok(rooms);
        }

        [Authorize(Roles = "Owner, Staff, Customer")]
        [HttpGet]
        [Route("GetAllRoomByRoomType")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllRooms>>>> GetAllRoomsByRoomTypeId(int roomTypeId)
        {
            var rooms = await _roomService.GetAllRoomsByRoomTypeId(roomTypeId);
            return Ok(rooms);
        }

        [Authorize(Roles = "Owner, Staff, Customer")]
        [HttpGet]
        [Route("GetRoom/{id}")]
        public async Task<ActionResult<BaseResponse<GetAllRooms>>> GetRoomByIdAsync(int id)
        {
            var rooms = await _roomService.GetRoomByIdAsync(id);
            return Ok(rooms);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpPost]
        [Route("CreateRoom")]
        public async Task<ActionResult<BaseResponse<CreateRoomRequest>>> CreateRoom(CreateRoomRequest typeRequest)
        {
            if (typeRequest == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var rooms = await _roomService.CreateRoom(typeRequest);
            return rooms;
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpPut]
        [Route("UpdateRoom")]
        public async Task<ActionResult<BaseResponse<UpdateRoomRequest>>> UpdateRoom(int roomID, UpdateRoomRequest request)
        {
            if (roomID <= 0 || roomID == null)
            {
                return BadRequest("Please Input Id!");
            }
            return await _roomService.UpdateRoom(roomID, request);
        }

        [Authorize(Roles = "Owner, Staff, Customer")]
        [HttpGet]
        [Route("FilterRoomsByRoomTypeAndDates")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllRooms>>>> FilterRoomsByRoomTypeAndDates(
            int roomTypeId, DateTime checkInDate, DateTime checkOutDate)
        {
            if (roomTypeId <= 0)
            {
                return BadRequest(new BaseResponse<IEnumerable<GetAllRooms>>(
                    "Invalid RoomTypeId!",
                    StatusCodeEnum.BadRequest_400,
                    null));
            }

            if (checkInDate >= checkOutDate)
            {
                return BadRequest(new BaseResponse<IEnumerable<GetAllRooms>>(
                    "Check-out date must be after check-in date!",
                    StatusCodeEnum.BadRequest_400,
                    null));
            }

            var rooms = await _roomService.FilterRoomsByRoomTypeAndDates(roomTypeId, checkInDate, checkOutDate);
            return Ok(rooms);
        }

        [Authorize(Roles = "Owner, Staff, Customer")]
        [HttpGet]
        [Route("FilterAllRoomsByHomeStayID")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllRooms>>>> FilterAllRoomsByHomeStayID(
        int homeStayID, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (homeStayID <= 0)
            {
                return BadRequest(new BaseResponse<IEnumerable<GetAllRooms>>(
                    "Invalid HomeStayID!",
                    StatusCodeEnum.BadRequest_400,
                    null));
            }

            if (startDate.HasValue && endDate.HasValue && startDate >= endDate)
            {
                return BadRequest(new BaseResponse<IEnumerable<GetAllRooms>>(
                    "End date must be after start date!",
                    StatusCodeEnum.BadRequest_400,
                    null));
            }

            var rooms = await _roomService.FilterAllRoomsByHomeStayIDAsync(homeStayID, startDate, endDate);
            return Ok(rooms);
        }
    }
}
