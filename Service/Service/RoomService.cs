using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
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

namespace Service.Service
{
    public class RoomService : IRoomService
    {
        private readonly IMapper _mapper;
        private readonly IRoomRepository _roomRepository;

        public RoomService(IMapper mapper, IRoomRepository roomRepository)
        {
            _mapper = mapper;
            _roomRepository = roomRepository;
        }

        

        public async Task<BaseResponse<IEnumerable<GetAllRooms>>> GetAllRooms()
        {
            IEnumerable<Room> room = await _roomRepository.GetAllRoomsAsync();
            if (room == null)
            {
                return new BaseResponse<IEnumerable<GetAllRooms>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var rooms = _mapper.Map<IEnumerable<GetAllRooms>>(room);
            if (rooms == null)
            {
                return new BaseResponse<IEnumerable<GetAllRooms>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllRooms>>("Get all Room as base success",
                StatusCodeEnum.OK_200, rooms);
        }

        public async Task<BaseResponse<IEnumerable<GetAllRooms>>> GetAllRoomsByRoomTypeId(int roomTypeId)
        {
            IEnumerable<Room> room = await _roomRepository.GetAllRoomsByRoomTypeIdAsync(roomTypeId);
            if (room == null)
            {
                return new BaseResponse<IEnumerable<GetAllRooms>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var rooms = _mapper.Map<IEnumerable<GetAllRooms>>(room);
            if (rooms == null)
            {
                return new BaseResponse<IEnumerable<GetAllRooms>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllRooms>>("Get all Room as base success",
                StatusCodeEnum.OK_200, rooms);
        }

        public async Task<BaseResponse<IEnumerable<GetAllRooms>>> GetAvailableRoomFilter(DateTime checkInDate, DateTime checkOutDate)
        {
            if(checkInDate >= checkOutDate)
            {
                return new BaseResponse<IEnumerable<GetAllRooms>>("Check-out date must be after check-in date.",
                StatusCodeEnum.BadGateway_502, null);
            }

            IEnumerable<Room> room = await _roomRepository.GetAvailableRoomFilter(checkInDate, checkOutDate);
            if (room == null)
            {
                return new BaseResponse<IEnumerable<GetAllRooms>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var rooms = _mapper.Map<IEnumerable<GetAllRooms>>(room);
            if (rooms == null)
            {
                return new BaseResponse<IEnumerable<GetAllRooms>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllRooms>>("Get all Room as base success",
                StatusCodeEnum.OK_200, rooms);
        }

        public async Task<BaseResponse<GetAllRooms>> GetRoomByIdAsync(int id)
        {
            Room room = await _roomRepository.GetRoomByIdAsync(id);
            var result = _mapper.Map<GetAllRooms>(room);
            return new BaseResponse<GetAllRooms>("Get Room as base success", StatusCodeEnum.OK_200, result);
        }

        public async Task<BaseResponse<CreateRoomRequest>> CreateRoom(CreateRoomRequest typeRequest)
        {
            Room rooms = _mapper.Map<Room>(typeRequest);
            await _roomRepository.AddAsync(rooms);

            var response = _mapper.Map<CreateRoomRequest>(rooms);
            response.isActive = true;
            response.isUsed = true;
            return new BaseResponse<CreateRoomRequest>("Add Room as base success", StatusCodeEnum.Created_201, response);
        }

        public async Task<BaseResponse<UpdateRoomRequest>> UpdateRoom(int roomID, UpdateRoomRequest request)
        {
            var roomExist = await _roomRepository.GetRoomByIdAsync(roomID);

            if (roomExist == null)
            {
                return new BaseResponse<UpdateRoomRequest>("Cannot find Room", StatusCodeEnum.BadGateway_502, null);
            }

            var updatedRoom = _mapper.Map(request, roomExist);

            updatedRoom.roomNumber = roomExist.roomNumber;
            updatedRoom.isUsed = roomExist.isUsed;
            updatedRoom.isActive = roomExist.isActive;
            updatedRoom.RoomTypesID = roomExist.RoomTypesID;

            await _roomRepository.UpdateAsync(updatedRoom);
            var updatedRoomResponse = _mapper.Map<UpdateRoomRequest>(roomExist);

            return new BaseResponse<UpdateRoomRequest>("Update Room successfully", StatusCodeEnum.OK_200, updatedRoomResponse);
        }

        public async Task<BaseResponse<GetAllRooms>> ChangeRoomStatus(int roomID, bool? isUsed, bool? isActive)
        {
            var roomExist = await _roomRepository.GetRoomByIdAsync(roomID);

            if (roomExist == null)
            {
                return new BaseResponse<GetAllRooms>("Cannot find Room", StatusCodeEnum.NotFound_404, null);
            }

            var room = await _roomRepository.ChangeRoomStatusAsync(roomExist.RoomID, isUsed,isActive);
            var roomResponse = _mapper.Map<GetAllRooms>(room);

            return new BaseResponse<GetAllRooms>("Update Room status successfully", StatusCodeEnum.OK_200, roomResponse);
        }
    }
}
