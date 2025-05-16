using AutoMapper;
using BusinessObject.Model;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
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
        private readonly IImageRoomRepository _imageRoomRepository;
        private readonly Cloudinary _cloudinary;

        public RoomService(IMapper mapper, IRoomRepository roomRepository, IImageRoomRepository imageRoomRepository, Cloudinary cloudinary)
        {
            _mapper = mapper;
            _roomRepository = roomRepository;
            _imageRoomRepository = imageRoomRepository;
            _cloudinary = cloudinary;
        }

        private async Task<List<string>> UploadImagesToCloudinary(List<IFormFile> files)
        {
            if (files == null || !files.Any()) return new List<string>();
            var urls = new List<string>();
            foreach (var file in files)
            {
                if (file == null || file.Length == 0) continue;
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "RoomImages"
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    urls.Add(uploadResult.SecureUrl.ToString());
                }
                else
                {
                    throw new Exception($"Failed to upload image to Cloudinary: {uploadResult.Error.Message}");
                }
            }
            return urls;
        }

        private string ExtractPublicIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            var parts = url.Split('/');
            var fileNameWithExtension = parts.Last();
            var publicId = fileNameWithExtension.Split('.').First();
            return $"RoomImages/{publicId}";
        }

        public async Task<BaseResponse<IEnumerable<GetAllRooms>>> GetAllRooms()
        {
            try
            {
                var rooms = await _roomRepository.GetAllRoomsAsync();
                if (rooms == null || !rooms.Any())
                {
                    return new BaseResponse<IEnumerable<GetAllRooms>>("No rooms found!", StatusCodeEnum.OK_200, new List<GetAllRooms>());
                }
                var responses = _mapper.Map<IEnumerable<GetAllRooms>>(rooms);
                return new BaseResponse<IEnumerable<GetAllRooms>>("Get all rooms successfully", StatusCodeEnum.OK_200, responses);
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<GetAllRooms>>($"Error getting rooms: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
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

        public async Task<BaseResponse<GetAllRooms>> CreateRoom(CreateRoomRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.roomNumber) || request.RoomTypesID <= 0)
                {
                    return new BaseResponse<GetAllRooms>("Room number and RoomTypesID are required!", StatusCodeEnum.BadRequest_400, null);
                }

                var room = _mapper.Map<Room>(request);
                room.isActive = true;

                await _roomRepository.AddAsync(room);
                await _roomRepository.SaveChangesAsync();
              
                if (request.Images?.Any() == true)
                {
                    var imageUrls = await UploadImagesToCloudinary(request.Images);
                    foreach (var url in imageUrls)
                    {
                        await _imageRoomRepository.AddImageAsync(new ImageRoom { Image = url, RoomID = room.RoomID });
                    }
                    await _imageRoomRepository.SaveChangesAsync();
                }

                // Ánh xạ sang GetAllRooms thay vì CreateRoomRequest
                var response = _mapper.Map<GetAllRooms>(room);
                return new BaseResponse<GetAllRooms>("Room created successfully", StatusCodeEnum.Created_201, response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<GetAllRooms>($"Error creating room: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
        }
        public async Task<BaseResponse<UpdateRoomRequest>> UpdateRoom(int roomId, UpdateRoomRequest request)
        {
            try
            {
                var room = await _roomRepository.GetRoomByIdAsync(roomId);
                if (room == null)
                {
                    return new BaseResponse<UpdateRoomRequest>("Room not found!", StatusCodeEnum.NotFound_404, null);
                }

                _mapper.Map(request, room);

               
                await _roomRepository.UpdateAsync(room);
                await _roomRepository.SaveChangesAsync();

                var response = _mapper.Map<UpdateRoomRequest>(room);
                return new BaseResponse<UpdateRoomRequest>("Room updated successfully", StatusCodeEnum.OK_200, response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<UpdateRoomRequest>($"Error updating room: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
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
        public async Task<BaseResponse<IEnumerable<GetAllRooms>>> FilterAllRoomsByHomeStayRentalIDAsync(int homeStayRentalID, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var rooms = await _roomRepository.FilterAllRoomsByHomeStayRentalIDAsync(homeStayRentalID, startDate, endDate);

                if (rooms == null || !rooms.Any())
                {
                    return new BaseResponse<IEnumerable<GetAllRooms>>(
                        "Không tìm thấy phòng cho HomeStayRentalID này.",
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
