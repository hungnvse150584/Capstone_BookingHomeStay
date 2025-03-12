using Service.RequestAndResponse.BaseResponse;
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

        Task<BaseResponse<CreateRoomTypeRequest>> CreateRoomType(CreateRoomTypeRequest typeRequest);
    }
}
