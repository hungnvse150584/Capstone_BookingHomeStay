using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.HomeStayType;
using Service.RequestAndResponse.Request.RoomType;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.RoomType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IMapper _mapper;
        private readonly IRoomTypeRepository _roomTypeRepository;

        public RoomTypeService(IMapper mapper, IRoomTypeRepository roomTypeRepository)
        {
            _mapper = mapper;
            _roomTypeRepository = roomTypeRepository; 
        }

        public async Task<BaseResponse<CreateRoomTypeRequest>> CreateRoomType(CreateRoomTypeRequest typeRequest)
        {
            RoomTypes roomTypes = _mapper.Map<RoomTypes>(typeRequest);
            await _roomTypeRepository.AddAsync(roomTypes);

            var response = _mapper.Map<CreateRoomTypeRequest>(roomTypes);
            return new BaseResponse<CreateRoomTypeRequest>("Add HomeStayType as base success", StatusCodeEnum.Created_201, response);
        }

        public async Task<BaseResponse<IEnumerable<GetAllRoomType>>> GetAllRoomTypes()
        {
            IEnumerable<RoomTypes> roomType = await _roomTypeRepository.GetAllAsync();
            if (roomType == null)
            {
                return new BaseResponse<IEnumerable<GetAllRoomType>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var roomTypes = _mapper.Map<IEnumerable<GetAllRoomType>>(roomType);
            if (roomTypes == null)
            {
                return new BaseResponse<IEnumerable<GetAllRoomType>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllRoomType>>("Get all RoomType as base success",
                StatusCodeEnum.OK_200, roomTypes);
        }
    }
}
