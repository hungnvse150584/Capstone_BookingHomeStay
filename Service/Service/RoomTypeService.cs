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
        private readonly IHomeStayRentalRepository _homeStayRentalRepository; // Thêm repository
        private readonly Cloudinary _cloudinary;
        private readonly IImageRoomTypesRepository _imageRoomTypesRepository;
        public RoomTypeService(
            IMapper mapper,
            IRoomTypeRepository roomTypeRepository,
            IHomeStayRentalRepository homeStayRentalRepository,
            Cloudinary cloudinary) // Thêm vào constructor
        {
            _mapper = mapper;
            _roomTypeRepository = roomTypeRepository;
            _homeStayRentalRepository = homeStayRentalRepository;
            _cloudinary = cloudinary;
        }

        public async Task<BaseResponse<CreateRoomTypeResponse>> CreateRoomType(CreateRoomTypeRequest request, int homeStayRentalId)
        {
            try
            {
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

                var roomType = _mapper.Map<RoomTypes>(request);
                roomType.CreateAt = DateTime.UtcNow;
                roomType.UpdateAt = DateTime.UtcNow;
                roomType.HomeStayRentalID = homeStayRentalId;

                await _roomTypeRepository.AddAsync(roomType);
                await _roomTypeRepository.SaveChangesAsync();

                // Thêm log để kiểm tra RoomTypesID
                Console.WriteLine($"RoomTypesID after SaveChanges: {roomType.RoomTypesID}");

                // Upload hình ảnh nếu có
                if (request.Images != null && request.Images.Any())
                {
                    Console.WriteLine("Uploading images to Cloudinary...");
                    var imageUrls = await UploadImagesToCloudinary(request.Images);
                    Console.WriteLine($"Uploaded {imageUrls.Count} images: {string.Join(", ", imageUrls)}");

                    // Kiểm tra RoomTypesID trước khi sử dụng
                    if (roomType.RoomTypesID == 0)
                    {
                        throw new Exception("RoomTypesID was not generated after saving RoomType.");
                    }

                    // Lưu từng URL vào ImageRoomTypes
                    foreach (var url in imageUrls)
                    {
                        var imageRoomType = new ImageRoomTypes
                        {
                            Image = url,
                            RoomTypesID = roomType.RoomTypesID
                        };
                        await _imageRoomTypesRepository.AddImageAsync(imageRoomType);
                    }

                    Console.WriteLine("Saving ImageRoomTypes...");
                    await _imageRoomTypesRepository.SaveChangesAsync();
                    Console.WriteLine("ImageRoomTypes saved successfully.");
                }

                // Ánh xạ RoomTypes sang CreateRoomTypeResponse
                var response = _mapper.Map<CreateRoomTypeResponse>(roomType);

                return new BaseResponse<CreateRoomTypeResponse>(
                    "RoomType created successfully!",
                    StatusCodeEnum.Created_201,
                    response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<CreateRoomTypeResponse>(
                    $"An error occurred while creating RoomType: {ex.Message}. InnerException: {ex.InnerException?.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }


        private async Task<List<string>> UploadImagesToCloudinary(List<IFormFile> files)
        {
            if (files == null || !files.Any())
            {
                return new List<string>();
            }

            var urls = new List<string>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    continue;
                }

                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "HomeStayRentalImages"
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
