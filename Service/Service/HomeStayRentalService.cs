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
using Service.RequestAndResponse.Request.HomeStayType;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class HomeStayRentalService : IHomeStayTypeService
    {
        private readonly IMapper _mapper;
        private readonly IHomeStayRentalRepository _homeStayTypeRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly Cloudinary _cloudinary;
        private readonly IImageHomeStayTypesRepository _imageHomeStayTypesRepository;

        public HomeStayRentalService(IMapper mapper, IHomeStayRentalRepository homeStayTypeRepository,
             IServiceRepository serviceRepository, Cloudinary cloudinary, IImageHomeStayTypesRepository imageHomeStayTypes)
        {
            _mapper = mapper;
            _homeStayTypeRepository = homeStayTypeRepository;
            _serviceRepository = serviceRepository;
            _cloudinary = cloudinary;
            _imageHomeStayTypesRepository = imageHomeStayTypes;
        }

        public async Task<BaseResponse<IEnumerable<GetAllHomeStayType>>> GetAllHomeStayTypesByHomeStayID(int homestayId)
        {
            IEnumerable<HomeStayRentals> homeStayType = await _homeStayTypeRepository.GetAllHomeStayTypesAsync(homestayId);
            if (homeStayType == null)
            {
                return new BaseResponse<IEnumerable<GetAllHomeStayType>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var homeStayTypes = _mapper.Map<IEnumerable<GetAllHomeStayType>>(homeStayType);
            if (homeStayTypes == null)
            {
                return new BaseResponse<IEnumerable<GetAllHomeStayType>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllHomeStayType>>("Get all HomeStayType as base success",
                StatusCodeEnum.OK_200, homeStayTypes);
        }

        public async Task<BaseResponse<List<HomeStayRentals>>> CreateHomeStayType(CreateHomeStayTypeRequest typeRequest)
        {
            try
            {
               
                var rentalEntity = _mapper.Map<HomeStayRentals>(typeRequest);

             
                rentalEntity.CreateAt = DateTime.Now;
                //rentalEntity.UpdateAt = DateTime.Now;

               
                await _homeStayTypeRepository.AddAsync(rentalEntity);
                await _homeStayTypeRepository.SaveChangesAsync();

              
                if (typeRequest.Images != null && typeRequest.Images.Any())
                {
                    var imageUrls = await UploadImagesToCloudinary(typeRequest.Images);
                    foreach (var url in imageUrls)
                    {
                        var imageRental = new ImageHomeStayRentals
                        {
                            Image = url,
                            HomeStayRentalID = rentalEntity.HomeStayRentalID
                        };
                        await _imageHomeStayTypesRepository.AddImageAsync(imageRental);
                    }
                }

         
                return new BaseResponse<List<HomeStayRentals>>("Create HomeStay Rental successfully", StatusCodeEnum.Created_201, new List<HomeStayRentals> { rentalEntity });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return new BaseResponse<List<HomeStayRentals>>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
        }
        private async Task<List<string>> UploadImagesToCloudinary(List<IFormFile> files)
        {
            if (files == null || !files.Any())
            {
                return new List<string>(); // Trả về danh sách rỗng nếu không có file
            }

            var urls = new List<string>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    continue; // Bỏ qua file không hợp lệ
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






        public async Task<BaseResponse<string>> DeleteHomeStayRental(int id)
        {
            var rental = await _homeStayTypeRepository.GetByIdAsync(id);
            await _homeStayTypeRepository.DeleteAsync(rental);
            return new BaseResponse<string>("Delete homestay rental success", StatusCodeEnum.OK_200, "Deleted successfully");
        }

        public async Task<BaseResponse<IEnumerable<GetAllHomeStayType>>> GetAllHomeStayTypes()
        {
            IEnumerable<HomeStayRentals> homeStayType = await _homeStayTypeRepository.GetAllAsync();
            if (homeStayType == null)
            {
                return new BaseResponse<IEnumerable<GetAllHomeStayType>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var homeStayTypes = _mapper.Map<IEnumerable<GetAllHomeStayType>>(homeStayType);
            if (homeStayTypes == null)
            {
                return new BaseResponse<IEnumerable<GetAllHomeStayType>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllHomeStayType>>("Get all HomeStayType as base success",
                StatusCodeEnum.OK_200, homeStayTypes);
        }
    } 
}
