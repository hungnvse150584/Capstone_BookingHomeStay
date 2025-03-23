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
using Service.RequestAndResponse.Request.Pricing;
using System.Text.Json;
using Service.RequestAndResponse.Request.RoomType;

namespace Service.Service
{
    public class HomeStayRentalService : IHomeStayTypeService
    {
        private readonly IMapper _mapper;
        private readonly IHomeStayRentalRepository _homeStayTypeRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly Cloudinary _cloudinary;
        private readonly IImageHomeStayTypesRepository _imageHomeStayTypesRepository;
        private readonly IPricingRepository _pricingRepository;
        private readonly IRoomTypeService _roomTypeService;
        public HomeStayRentalService(IMapper mapper, IHomeStayRentalRepository homeStayTypeRepository,
             IServiceRepository serviceRepository, Cloudinary cloudinary, IImageHomeStayTypesRepository imageHomeStayTypes, IPricingRepository pricingRepository, IRoomTypeService roomTypeService)
        {
            _mapper = mapper;
            _homeStayTypeRepository = homeStayTypeRepository;
            _serviceRepository = serviceRepository;
            _cloudinary = cloudinary;
            _imageHomeStayTypesRepository = imageHomeStayTypes;
            _pricingRepository = pricingRepository;
            _roomTypeService = roomTypeService;
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

        public async Task<BaseResponse<HomeStayRentals>> CreateHomeStayType(CreateHomeStayTypeRequest request)
        {
            try
            {
                // Deserialize PricingJson
                if (!string.IsNullOrEmpty(request.PricingJson))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    request.Pricing = JsonSerializer.Deserialize<List<PricingForHomeStayRental>>(request.PricingJson, options);

                    if (request.Pricing != null)
                    {
                        foreach (var pricing in request.Pricing)
                        {
                            Console.WriteLine($"After deserialize - PricingForHomeStayRental: UnitPrice={pricing.UnitPrice}, RentPrice={pricing.RentPrice}, StartDate={pricing.StartDate?.ToString() ?? "null"}, EndDate={pricing.EndDate?.ToString() ?? "null"}, IsDefault={pricing.IsDefault}, IsActive={pricing.IsActive}, DayType={pricing.DayType}, Description={pricing.Description}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("request.Pricing is null after deserialize.");
                    }
                }
                else
                {
                    Console.WriteLine("PricingJson is null or empty.");
                }

                // Gán giá trị mặc định cho Status và RentWhole nếu là null
                request.Status ??= true;
                request.RentWhole ??= true;

                // Log để kiểm tra
                Console.WriteLine($"RentWhole: {request.RentWhole.Value}");
                Console.WriteLine($"Status: {request.Status.Value}");
                Console.WriteLine($"PricingJson: {request.PricingJson ?? "null"}");
                Console.WriteLine($"Pricing: {(request.Pricing != null ? $"Count = {request.Pricing.Count}" : "null")}");

                // Kiểm tra RentWhole và Pricing
                if (request.RentWhole.Value && (request.Pricing == null || !request.Pricing.Any()))
                {
                    return new BaseResponse<HomeStayRentals>(
                        "Pricing must be provided when RentWhole is true!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }
                if (!request.RentWhole.Value && request.Pricing != null && request.Pricing.Any())
                {
                    return new BaseResponse<HomeStayRentals>(
                        "Pricing cannot be provided when RentWhole is false!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                var homeStayRental = _mapper.Map<HomeStayRentals>(request);
                homeStayRental.CreateAt = DateTime.Now;

                if (request.RentWhole.Value)
                {
                    homeStayRental.Prices = _mapper.Map<ICollection<Pricing>>(request.Pricing);

                    if (homeStayRental.Prices != null)
                    {
                        foreach (var price in homeStayRental.Prices)
                        {
                            Console.WriteLine($"After mapping - Pricing: UnitPrice={price.UnitPrice}, RentPrice={price.RentPrice}, StartDate={price.StartDate?.ToString() ?? "null"}, EndDate={price.EndDate?.ToString() ?? "null"}, IsDefault={price.IsDefault}, IsActive={price.IsActive}, DayType={price.DayType}, Description={price.Description}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("homeStayRental.Prices is null after mapping.");
                    }
                }

                await _homeStayTypeRepository.AddAsync(homeStayRental);
                Console.WriteLine("Saving HomeStayRentals...");
                await _homeStayTypeRepository.SaveChangesAsync();
                Console.WriteLine("HomeStayRentals saved successfully.");

                if (request.Images != null && request.Images.Any())
                {
                    Console.WriteLine("Uploading images to Cloudinary...");
                    var imageUrls = await UploadImagesToCloudinary(request.Images);
                    Console.WriteLine($"Uploaded {imageUrls.Count} images: {string.Join(", ", imageUrls)}");
                    foreach (var url in imageUrls)
                    {
                        var imageRental = new ImageHomeStayRentals
                        {
                            Image = url,
                            HomeStayRentalID = homeStayRental.HomeStayRentalID,
                        };
                        await _imageHomeStayTypesRepository.AddImageAsync(imageRental);
                    }
                    Console.WriteLine("Saving ImageHomeStayRentals...");
                    await _imageHomeStayTypesRepository.SaveChangesAsync();
                    Console.WriteLine("ImageHomeStayRentals saved successfully.");
                }

                if (request.RentWhole.Value && homeStayRental.Prices != null && homeStayRental.Prices.Any())
                {
                    Console.WriteLine("Saving Prices...");
                    foreach (var price in homeStayRental.Prices)
                    {
                        Console.WriteLine($"Before saving - Pricing: UnitPrice={price.UnitPrice}, RentPrice={price.RentPrice}, StartDate={price.StartDate?.ToString() ?? "null"}, EndDate={price.EndDate?.ToString() ?? "null"}, IsDefault={price.IsDefault}, IsActive={price.IsActive}, DayType={price.DayType}, Description={price.Description}");

                        price.PricingID = 0;
                        price.HomeStayRentalID = homeStayRental.HomeStayRentalID;
                        await _pricingRepository.AddAsync(price);
                    }
                    await _pricingRepository.SaveChangesAsync();
                    Console.WriteLine("Prices saved successfully.");
                }

                return new BaseResponse<HomeStayRentals>(
                    "HomeStayType created successfully!",
                    StatusCodeEnum.Created_201,
                    homeStayRental);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return new BaseResponse<HomeStayRentals>(
                    $"An error occurred while creating HomeStayType: {ex.Message}. InnerException: {ex.InnerException?.Message}",
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
