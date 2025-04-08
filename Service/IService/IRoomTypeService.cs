using BusinessObject.Model;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Room;
using Service.RequestAndResponse.Request.RoomType;
using Service.RequestAndResponse.Response.RoomType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IRoomTypeService
    {
        Task<BaseResponse<IEnumerable<GetAllRoomType>>> GetAllRoomTypes();

        Task<BaseResponse<CreateRoomTypeResponse>> CreateRoomType(CreateRoomTypeRequest request, int homeStayRentalId);

        Task<BaseResponse<UpdateRoomTypeRequest>> UpdateRoomType(int roomID, UpdateRoomTypeRequest request);

        Task<BaseResponse<IEnumerable<GetAllRoomTypeByRental>>> GetAllRoomTypeByHomeStayRentalID(int homeStayRentalId);

        Task<BaseResponse<GetAllRoomTypeByRental>> GetRoomTypeByID(int roomTypeId);
    }
}
