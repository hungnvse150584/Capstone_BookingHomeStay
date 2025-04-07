using AutoMapper;
using BusinessObject.Model;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.HomeStayType;
using Service.RequestAndResponse.Request.Pricing;
using Service.RequestAndResponse.Request.Room;
using Service.RequestAndResponse.Request.RoomType;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.RoomType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Service.RequestAndResponse.Request.RoomType.CreateRoomTypeRequest;

namespace Service.Service
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IMapper _mapper;
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IHomeStayRentalRepository _homeStayRentalRepository; // Thêm repository
        private readonly Cloudinary _cloudinary;
        private readonly IImageRoomTypesRepository _imageRoomTypesRepository;
        private readonly IPricingRepository _pricingRepository; 
        private readonly IRoomRepository _roomRepository;
        public RoomTypeService(
             IMapper mapper,
             IRoomTypeRepository roomTypeRepository,
             IHomeStayRentalRepository homeStayRentalRepository,
             Cloudinary cloudinary,
             IImageRoomTypesRepository imageRoomTypesRepository,
             IPricingRepository pricingRepository,
             IRoomRepository roomRepository)
        {
            _mapper = mapper;
            _roomTypeRepository = roomTypeRepository;
            _homeStayRentalRepository = homeStayRentalRepository;
            _cloudinary = cloudinary;
            _imageRoomTypesRepository = imageRoomTypesRepository;
            _pricingRepository = pricingRepository;
            _roomRepository = roomRepository;
        }

        public async Task<BaseResponse<CreateRoomTypeResponse>> CreateRoomType(CreateRoomTypeRequest request, int homeStayRentalId)
        {
            try
            {
                // Kiểm tra request không phải là null
                if (request == null)
                {
                    return new BaseResponse<CreateRoomTypeResponse>(
                        "Request cannot be null!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                // Kiểm tra HomeStayRentalID có tồn tại và RentWhole = false
                var homeStayRental = await _homeStayRentalRepository.GetByIdAsync(homeStayRentalId);
                if (homeStayRental == null)
                {
                    return new BaseResponse<CreateRoomTypeResponse>(
                        "HomeStayRental not found!",
                        StatusCodeEnum.NotFound_404,
                        null);
                }

                if (homeStayRental.RentWhole)
                {
                    return new BaseResponse<CreateRoomTypeResponse>(
                        "Cannot create RoomType because RentWhole is true!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                // Xử lý Pricing: Ưu tiên PricingJson
                List<PricingForHomeStayRental> pricingList = null;
                if (!string.IsNullOrEmpty(request.PricingJson))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    Console.WriteLine("Processing PricingJson...");
                    Console.WriteLine($"PricingJson: {request.PricingJson}");

                    // Kiểm tra xem PricingJson có phải là danh sách không
                    if (request.PricingJson.TrimStart().StartsWith("["))
                    {
                        // Nếu là danh sách, deserialize trực tiếp thành List<PricingForRoomType>
                        pricingList = JsonSerializer.Deserialize<List<PricingForHomeStayRental>>(request.PricingJson, options);
                    }
                    else
                    {
                        // Nếu là đối tượng đơn lẻ, deserialize thành PricingForRoomType rồi đưa vào danh sách
                        var singlePricing = JsonSerializer.Deserialize<PricingForHomeStayRental>(request.PricingJson, options);
                        pricingList = new List<PricingForHomeStayRental> { singlePricing };
                    }

                    if (pricingList != null)
                    {
                        Console.WriteLine("Using Pricing from PricingJson.");
                        foreach (var pricing in pricingList)
                        {
                            Console.WriteLine($"Pricing from PricingJson: UnitPrice={pricing.UnitPrice}, RentPrice={pricing.RentPrice}, StartDate={pricing.StartDate}, EndDate={pricing.EndDate}, IsDefault={pricing.IsDefault}, IsActive={pricing.IsActive}, DayType={pricing.DayType}, Description={pricing.Description}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("pricingList is null after deserialize from PricingJson.");
                    }
                }
                else
                {
                    // Nếu PricingJson rỗng, sử dụng Pricing từ request (nếu có)
                    if (request.Pricing != null && request.Pricing.Any())
                    {
                        Console.WriteLine("Using Pricing directly from request.");
                        pricingList = request.Pricing;
                        foreach (var pricing in pricingList)
                        {
                            Console.WriteLine($"Pricing from request: UnitPrice={pricing.UnitPrice}, RentPrice={pricing.RentPrice}, StartDate={pricing.StartDate}, EndDate={pricing.EndDate}, IsDefault={pricing.IsDefault}, IsActive={pricing.IsActive}, DayType={pricing.DayType}, Description={pricing.Description}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Both PricingJson and Pricing are null or empty.");
                    }
                }

                // Xử lý Rooms: Ưu tiên RoomsJson
                //List<CreateRoomRequest> roomsList = null;
                //if (!string.IsNullOrEmpty(request.RoomsJson))
                //{
                //    var options = new JsonSerializerOptions
                //    {
                //        PropertyNameCaseInsensitive = true
                //    };

                //    Console.WriteLine("Processing RoomsJson...");
                //    Console.WriteLine($"RoomsJson: {request.RoomsJson}");

                //    if (request.RoomsJson.TrimStart().StartsWith("["))
                //    {
                //        roomsList = JsonSerializer.Deserialize<List<CreateRoomRequest>>(request.RoomsJson, options);
                //    }
                //    else
                //    {
                //        var singleRoom = JsonSerializer.Deserialize<CreateRoomRequest>(request.RoomsJson, options);
                //        roomsList = new List<CreateRoomRequest> { singleRoom };
                //    }

                //    if (roomsList != null)
                //    {
                //        Console.WriteLine("Using Rooms from RoomsJson.");
                //        foreach (var room in roomsList)
                //        {
                //            Console.WriteLine($"Room from RoomsJson: roomNumber={room.roomNumber}, isUsed={room.isUsed}, isActive={room.isActive}, RoomTypesID={room.RoomTypesID}");
                //        }
                //    }
                //    else
                //    {
                //        Console.WriteLine("roomsList is null after deserialize from RoomsJson.");
                //    }
                //}
                //else
                //{
                //    if (request.Rooms != null && request.Rooms.Any())
                //    {
                //        Console.WriteLine("Using Rooms directly from request.");
                //        roomsList = request.Rooms;
                //        foreach (var room in roomsList)
                //        {
                //            Console.WriteLine($"Room from request: roomNumber={room.roomNumber}, isUsed={room.isUsed}, isActive={room.isActive}, RoomTypesID={room.RoomTypesID}");
                //        }
                //    }
                //    else
                //    {
                //        Console.WriteLine("Both RoomsJson and Rooms are null or empty.");
                //    }
                //}

                var roomType = _mapper.Map<RoomTypes>(request);
                roomType.CreateAt = DateTime.UtcNow;
                roomType.UpdateAt = DateTime.UtcNow;
                roomType.HomeStayRentalID = homeStayRentalId;

                //Luôn true
                roomType.Status = true;

                await _roomTypeRepository.AddAsync(roomType);
                await _roomTypeRepository.SaveChangesAsync();

                // Thêm log để kiểm tra RoomTypesID
                Console.WriteLine($"RoomTypesID after SaveChanges: {roomType.RoomTypesID}");

                // Kiểm tra RoomTypesID trước khi sử dụng
                if (roomType.RoomTypesID == 0)
                {
                    throw new Exception("RoomTypesID was not generated after saving RoomType.");
                }

                // Xử lý Pricing nếu có
                var pricings = new List<Pricing>();
                if (pricingList != null && pricingList.Any())
                {
                    Console.WriteLine("Saving Pricings...");
                    foreach (var pricingItem in pricingList)
                    {
                        var pricing = _mapper.Map<Pricing>(pricingItem);
                        pricing.RoomTypesID = roomType.RoomTypesID;
                        pricings.Add(pricing);
                        await _pricingRepository.AddAsync(pricing);
                    }
                    await _pricingRepository.SaveChangesAsync();
                    Console.WriteLine("Pricings saved successfully.");
                }

                // Xử lý Rooms nếu có
                //var rooms = new List<Room>();
                //if (roomsList != null && roomsList.Any())
                //{
                //    Console.WriteLine("Saving Rooms...");
                //    foreach (var roomItem in roomsList)
                //    {
                //        var room = _mapper.Map<Room>(roomItem);
                //        room.RoomTypesID = roomType.RoomTypesID;
                //        rooms.Add(room);
                //        await _roomRepository.AddAsync(room);
                //    }
                //    await _roomRepository.SaveChangesAsync();
                //    Console.WriteLine("Rooms saved successfully.");
                //}

                // Upload hình ảnh nếu có
                var imageRoomTypes = new List<ImageRoomTypes>();
                if (request.Images != null && request.Images.Any())
                {
                    Console.WriteLine("Uploading images to Cloudinary...");
                    var imageUrls = await UploadImagesToCloudinary(request.Images);
                    Console.WriteLine($"Uploaded {imageUrls.Count} images: {string.Join(", ", imageUrls)}");

                    foreach (var url in imageUrls)
                    {
                        var imageRoomType = new ImageRoomTypes
                        {
                            Image = url,
                            RoomTypesID = roomType.RoomTypesID
                        };
                        imageRoomTypes.Add(imageRoomType);
                        await _imageRoomTypesRepository.AddImageAsync(imageRoomType);
                    }

                    Console.WriteLine("Saving ImageRoomTypes...");
                    await _imageRoomTypesRepository.SaveChangesAsync();
                    Console.WriteLine("ImageRoomTypes saved successfully.");
                }

                // Cập nhật các collection cho roomType để ánh xạ vào response
                roomType.ImageRoomTypes = imageRoomTypes;
                roomType.Prices = pricings;
                //roomType.Rooms = rooms;

                // Ánh xạ RoomTypes sang CreateRoomTypeResponse
                var response = _mapper.Map<CreateRoomTypeResponse>(roomType);

                return new BaseResponse<CreateRoomTypeResponse>(
                    "RoomType created successfully!",
                    StatusCodeEnum.Created_201,
                    response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return new BaseResponse<CreateRoomTypeResponse>(
                    $"An error occurred while creating RoomType: {ex.Message}. InnerException: {ex.InnerException?.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }


        //private async Task<List<string>> UploadImagesToCloudinary(List<IFormFile> files)
        //{
        //    if (files == null || !files.Any())
        //    {
        //        return new List<string>();
        //    }

        //    var urls = new List<string>();

        //    foreach (var file in files)
        //    {
        //        if (file == null || file.Length == 0)
        //        {
        //            continue;
        //        }

        //        using var stream = file.OpenReadStream();
        //        var uploadParams = new ImageUploadParams
        //        {
        //            File = new FileDescription(file.FileName, stream),
        //            Folder = "HomeStayRentalImages"
        //        };

        //        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        //        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            urls.Add(uploadResult.SecureUrl.ToString());
        //        }
        //        else
        //        {
        //            throw new Exception($"Failed to upload image to Cloudinary: {uploadResult.Error.Message}");
        //        }
        //    }

        //    return urls;
        //}
        private async Task<List<string>> UploadImagesToCloudinary(List<IFormFile> files)
        {
            if (files == null || !files.Any())
            {
                Console.WriteLine("No images to upload.");
                return new List<string>();
            }

            if (_cloudinary == null)
            {
                throw new InvalidOperationException("Cloudinary service is not initialized.");
            }

            var urls = new List<string>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    Console.WriteLine("Skipping empty file.");
                    continue;
                }

                if (string.IsNullOrEmpty(file.FileName))
                {
                    Console.WriteLine("Skipping file with null or empty FileName.");
                    continue;
                }

                try
                {
                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "RoomTypeImages"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        urls.Add(uploadResult.SecureUrl.ToString());
                        Console.WriteLine($"Successfully uploaded image: {file.FileName}, URL: {uploadResult.SecureUrl}");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to upload image {file.FileName}: {uploadResult.Error?.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error uploading image {file.FileName}: {ex.Message}");
                }
            }

            return urls;
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
        public async Task<BaseResponse<IEnumerable<GetAllRoomTypeByRental>>> GetAllRoomTypeByHomeStayRentalID(int homeStayRentalId)
        {
            try
            {
                var homeStayRental = await _homeStayRentalRepository.GetByIdAsync(homeStayRentalId);
                if (homeStayRental == null)
                {
                    return new BaseResponse<IEnumerable<GetAllRoomTypeByRental>>(
                        "HomeStayRental not found!",
                        StatusCodeEnum.NotFound_404,
                        null);
                }

                var roomTypes = await _roomTypeRepository.GetAllRoomTypeByHomeStayRentalID(homeStayRentalId);
                if (roomTypes == null || !roomTypes.Any())
                {
                    return new BaseResponse<IEnumerable<GetAllRoomTypeByRental>>(
                        "No RoomTypes found for the specified HomeStayRentalID.",
                        StatusCodeEnum.OK_200,
                        new List<GetAllRoomTypeByRental>());
                }

                // Thêm log để kiểm tra dữ liệu Prices
                foreach (var roomType in roomTypes)
                {
                    Console.WriteLine($"RoomTypesID: {roomType.RoomTypesID}, Prices Count: {(roomType.Prices != null ? roomType.Prices.Count : 0)}");
                    if (roomType.Prices != null && roomType.Prices.Any())
                    {
                        foreach (var price in roomType.Prices)
                        {
                            Console.WriteLine($"Price: PricingID={price.PricingID}, UnitPrice={price.UnitPrice}, RentPrice={price.RentPrice}");
                        }
                    }
                }

                var roomTypeResponses = _mapper.Map<IEnumerable<GetAllRoomTypeByRental>>(roomTypes);

                return new BaseResponse<IEnumerable<GetAllRoomTypeByRental>>(
                    "RoomTypes retrieved successfully!",
                    StatusCodeEnum.OK_200,
                    roomTypeResponses);
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<GetAllRoomTypeByRental>>(
                    $"An error occurred while retrieving RoomTypes: {ex.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }

        public async Task<BaseResponse<UpdateRoomTypeRequest>> UpdateRoomType(int roomID, UpdateRoomTypeRequest request)
        {
            var roomTypeExist = await _roomTypeRepository.GetByIdAsync(roomID);

            if (roomTypeExist == null)
            {
                return new BaseResponse<UpdateRoomTypeRequest>("Cannot find Room", StatusCodeEnum.BadGateway_502, null);
            }
            var homeStayMatch = await _homeStayRentalRepository.GetHomeStayTypesByIdAsync(roomTypeExist.HomeStayRentalID);
            if (homeStayMatch == null)
            {
                return new BaseResponse<UpdateRoomTypeRequest>("Cannot find HomeStayRental match with your Roomtype", StatusCodeEnum.BadGateway_502, null);
            }

            var updatedRoomType = _mapper.Map(request, roomTypeExist);

            updatedRoomType.Name = roomTypeExist.Name;
            updatedRoomType.Description = roomTypeExist.Description;
            updatedRoomType.numberBedRoom = roomTypeExist.numberBedRoom;
            updatedRoomType.numberBathRoom = roomTypeExist.numberBathRoom;
            updatedRoomType.numberWifi = roomTypeExist.numberWifi;
            updatedRoomType.Status = roomTypeExist.Status;
            updatedRoomType.MaxAdults = roomTypeExist.MaxAdults;
            updatedRoomType.MaxChildren = roomTypeExist.MaxChildren;
            updatedRoomType.MaxPeople = updatedRoomType.MaxAdults + updatedRoomType.MaxChildren;

            await _roomTypeRepository.UpdateAsync(updatedRoomType);
            var updatedRoomTypeResponse = _mapper.Map<UpdateRoomTypeRequest>(roomTypeExist);

            return new BaseResponse<UpdateRoomTypeRequest>("Update Room successfully", StatusCodeEnum.OK_200, updatedRoomTypeResponse);
        }
    }
}
