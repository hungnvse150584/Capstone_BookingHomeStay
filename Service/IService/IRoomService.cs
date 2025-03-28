using BusinessObject.Model;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Request.Room;
using Service.RequestAndResponse.Request.RoomType;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.Room;
using Service.RequestAndResponse.Response.RoomType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IRoomService
    {
        Task<BaseResponse<IEnumerable<GetAllRooms>>> GetAllRooms();

        Task<BaseResponse<IEnumerable<GetAllRooms>>> GetAvailableRoomFilter(DateTime checkInDate, DateTime checkOutDate);

        Task<BaseResponse<IEnumerable<GetAllRooms>>> GetAllRoomsByRoomTypeId(int roomTypeId);

        Task<BaseResponse<GetAllRooms>> GetRoomByIdAsync(int id);

        Task<BaseResponse<CreateRoomRequest>> CreateRoom(CreateRoomRequest typeRequest);

        Task<BaseResponse<UpdateRoomRequest>> UpdateRoom(int roomID, UpdateRoomRequest request);

        Task<BaseResponse<GetAllRooms>> ChangeRoomStatus(int roomID, bool? isUsed, bool? isActive);
        Task<BaseResponse<IEnumerable<GetAllRooms>>> FilterRoomsByRoomTypeAndDates(int roomTypeId, DateTime checkInDate, DateTime checkOutDate);
    }
}
