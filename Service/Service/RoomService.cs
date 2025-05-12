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
            updatedRoom.isActive = roomExist.isActive;
            updatedRoom.RoomTypesID = roomExist.RoomTypesID;

            await _roomRepository.UpdateAsync(updatedRoom);
            var updatedRoomResponse = _mapper.Map<UpdateRoomRequest>(roomExist);

            return new BaseResponse<UpdateRoomRequest>("Update Room successfully", StatusCodeEnum.OK_200, updatedRoomResponse);
        }

        public async Task<BaseResponse<GetAllRooms>> ChangeRoomStatus(int roomID, bool? isActive)
        {
            var roomExist = await _roomRepository.GetRoomByIdAsync(roomID);

            if (roomExist == null)
            {
                return new BaseResponse<GetAllRooms>("Cannot find Room", StatusCodeEnum.NotFound_404, null);
            }

            var room = await _roomRepository.ChangeRoomStatusAsync(roomExist.RoomID,isActive);
            var roomResponse = _mapper.Map<GetAllRooms>(room);

            return new BaseResponse<GetAllRooms>("Update Room status successfully", StatusCodeEnum.OK_200, roomResponse);
        }
        public async Task<BaseResponse<IEnumerable<GetAllRooms>>> FilterRoomsByRoomTypeAndDates(int roomTypeId, DateTime checkInDate, DateTime checkOutDate)
        {
            try
            {
                // Gọi repository để lấy danh sách Room theo RoomTypeId và CheckInDate/CheckOutDate
                var rooms = await _roomRepository.FilterRoomsByRoomTypeAndDates(roomTypeId, checkInDate, checkOutDate);

                if (rooms == null || !rooms.Any())
                {
                    return new BaseResponse<IEnumerable<GetAllRooms>>(
                        "No available rooms found for the specified RoomTypeId and dates.",
                        StatusCodeEnum.OK_200,
                        new List<GetAllRooms>());
                }

                // Ánh xạ sang GetAllRooms
                var roomResponses = _mapper.Map<IEnumerable<GetAllRooms>>(rooms);

                return new BaseResponse<IEnumerable<GetAllRooms>>(
                    "Rooms retrieved successfully!",
                    StatusCodeEnum.OK_200,
                    roomResponses);
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<GetAllRooms>>(
                    $"An error occurred while retrieving rooms: {ex.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }
        public async Task<BaseResponse<IEnumerable<GetAllRooms>>> FilterAllRoomsByHomeStayIDAsync(int homeStayID, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var rooms = await _roomRepository.FilterAllRoomsByHomeStayIDAsync(homeStayID, startDate, endDate);

                if (rooms == null || !rooms.Any())
                {
                    return new BaseResponse<IEnumerable<GetAllRooms>>(
                        "Không tìm thấy phòng cho HomeStayID này.",
                        StatusCodeEnum.OK_200,
                        new List<GetAllRooms>());
                }

                Console.WriteLine("Phòng trước khi ánh xạ:");
                foreach (var room in rooms)
                {
                    Console.WriteLine($"RoomID: {room.RoomID}, RoomTypesID: {room.RoomTypesID}, RoomTypeName: {room.RoomTypes?.Name}");
                    if (room.RoomTypes?.Prices != null)
                    {
                        Console.WriteLine($"Giá: {string.Join(", ", room.RoomTypes.Prices.Select(p => $"RentPrice: {p.RentPrice}, DayType: {p.DayType}, IsActive: {p.IsActive}"))}");
                    }
                    else
                    {
                        Console.WriteLine("Không có giá cho RoomTypes này.");
                    }
                }

                var roomResponses = _mapper.Map<IEnumerable<GetAllRooms>>(rooms);

                Console.WriteLine("Phòng sau khi ánh xạ:");
                foreach (var roomResponse in roomResponses)
                {
                    Console.WriteLine($"RoomID: {roomResponse.RoomID}, RoomTypeName: {roomResponse.RoomTypeName ?? "null"}, RentPrice: {roomResponse.RentPrice?.ToString() ?? "null"}");
                }

                return new BaseResponse<IEnumerable<GetAllRooms>>(
                    "Lấy danh sách phòng thành công!",
                    StatusCodeEnum.OK_200,
                    roomResponses);
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<GetAllRooms>>(
                    $"Đã xảy ra lỗi khi lấy danh sách phòng: {ex.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }
    }
}
