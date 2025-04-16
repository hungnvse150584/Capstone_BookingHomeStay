using AutoMapper;
using BusinessObject.Model;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Repository.IRepositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.Services;
using System.Net;

namespace Service.Service;

public class ServicesService : IServiceServices
{
    private readonly IMapper _mapper;
    private readonly IServiceRepository _servicesRepository;
    private readonly IImageServicesRepository _imageServiceRepository;
    private readonly Cloudinary _cloudinary;

    public ServicesService(
        IMapper mapper,
        IServiceRepository servicesRepository,
        IImageServicesRepository imageServiceRepository,
        Cloudinary cloudinary)
    {
        _mapper = mapper;
        _servicesRepository = servicesRepository;
        _imageServiceRepository = imageServiceRepository;
        _cloudinary = cloudinary;
    }

    public async Task<BaseResponse<ServiceWithTotalPriceResponse>> CreateService(CreateServices request)
    {
        try
        {
            // Ánh xạ dữ liệu từ request sang entity
            var serviceEntity = _mapper.Map<Services>(request);
            serviceEntity.CreateAt = DateTime.UtcNow;
            serviceEntity.UpdateAt = DateTime.UtcNow;

            // Tính TotalPrice dựa trên ServiceType
            double totalPrice = 0;
            if (request.ServiceType == ServiceType.Day)
            {
                // Thuê theo ngày: Tính số ngày từ StartDate và EndDate
                if (!request.StartDate.HasValue || !request.EndDate.HasValue)
                {
                    return new BaseResponse<ServiceWithTotalPriceResponse>(
                        "StartDate and EndDate are required for ServiceType Day!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                if (request.StartDate >= request.EndDate)
                {
                    return new BaseResponse<ServiceWithTotalPriceResponse>(
                        "EndDate must be greater than StartDate!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                var numberOfDays = (request.EndDate.Value - request.StartDate.Value).Days;
                if (numberOfDays <= 0)
                {
                    return new BaseResponse<ServiceWithTotalPriceResponse>(
                        "Number of rental days must be greater than 0!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                totalPrice = request.servicesPrice * numberOfDays;
            }
            else if (request.ServiceType == ServiceType.Quantity)
            {
                // Thuê theo số lượng: Tính dựa trên Quantity
                if (request.Quantity <= 0)
                {
                    return new BaseResponse<ServiceWithTotalPriceResponse>(
                        "Quantity must be greater than 0 for ServiceType Quantity!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                totalPrice = (double)(request.servicesPrice * request.Quantity);
            }

            // Lưu service (không lưu TotalPrice)
            await _servicesRepository.AddAsync(serviceEntity);
            await _servicesRepository.SaveChangesAsync();

            // Upload ảnh nếu có
            if (request.Images != null && request.Images.Any())
            {
                var imageUrls = await UploadImagesToCloudinary(request.Images);
                foreach (var url in imageUrls)
                {
                    var imageService = new ImageServices
                    {
                        Image = url,
                        ServicesID = serviceEntity.ServicesID
                    };
                    await _imageServiceRepository.AddImageAsync(imageService);
                }
            }

            // Ánh xạ sang ServiceWithTotalPriceResponse và thêm TotalPrice
            var response = _mapper.Map<ServiceWithTotalPriceResponse>(serviceEntity);
            response.TotalPrice = totalPrice;

            return new BaseResponse<ServiceWithTotalPriceResponse>("Service created successfully", StatusCodeEnum.Created_201, response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            return new BaseResponse<ServiceWithTotalPriceResponse>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
        }
    }

    public async Task<BaseResponse<ServiceWithTotalPriceResponse>> UpdateService(int serviceId, UpdateServices request)
    {
        try
        {
            // Kiểm tra service có tồn tại không
            var serviceExist = await _servicesRepository.GetByIdAsync(serviceId);
            if (serviceExist == null)
            {
                return new BaseResponse<ServiceWithTotalPriceResponse>("Cannot find service", StatusCodeEnum.BadGateway_502, null);
            }

            // Ánh xạ dữ liệu từ request sang entity
            var updatedService = _mapper.Map(request, serviceExist);
            updatedService.CreateAt = serviceExist.CreateAt;
            updatedService.UpdateAt = DateTime.UtcNow;

            // Tính TotalPrice dựa trên ServiceType
            double totalPrice = 0;
            if (request.ServiceType == ServiceType.Day)
            {
                // Thuê theo ngày: Tính số ngày từ StartDate và EndDate
                if (!request.StartDate.HasValue || !request.EndDate.HasValue)
                {
                    return new BaseResponse<ServiceWithTotalPriceResponse>(
                        "StartDate and EndDate are required for ServiceType Day!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                if (request.StartDate >= request.EndDate)
                {
                    return new BaseResponse<ServiceWithTotalPriceResponse>(
                        "EndDate must be greater than StartDate!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                var numberOfDays = (request.EndDate.Value - request.StartDate.Value).Days;
                if (numberOfDays <= 0)
                {
                    return new BaseResponse<ServiceWithTotalPriceResponse>(
                        "Number of rental days must be greater than 0!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                totalPrice = request.servicesPrice * numberOfDays;
            }
            else if (request.ServiceType == ServiceType.Quantity)
            {
                // Thuê theo số lượng: Tính dựa trên Quantity
                if (request.Quantity <= 0)
                {
                    return new BaseResponse<ServiceWithTotalPriceResponse>(
                        "Quantity must be greater than 0 for ServiceType Quantity!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                totalPrice = (double)(request.servicesPrice * request.Quantity);
            }

            // Lưu service (không lưu TotalPrice)
            await _servicesRepository.UpdateAsync(updatedService);
            await _servicesRepository.SaveChangesAsync();

            // Upload ảnh nếu có
            //if (request.Images != null && request.Images.Any())
            //{
            //    var imageUrls = await UploadImagesToCloudinary(request.Images);
            //    foreach (var url in imageUrls)
            //    {
            //        var imageService = new ImageServices
            //        {
            //            Image = url,
            //            ServicesID = updatedService.ServicesID
            //        };
            //        await _imageServiceRepository.AddImageAsync(imageService);
            //    }
            //}

            // Ánh xạ sang ServiceWithTotalPriceResponse và thêm TotalPrice
            var response = _mapper.Map<ServiceWithTotalPriceResponse>(updatedService);
            response.TotalPrice = totalPrice;

            return new BaseResponse<ServiceWithTotalPriceResponse>("Update Service successfully", StatusCodeEnum.OK_200, response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            return new BaseResponse<ServiceWithTotalPriceResponse>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
        }
    }

    // Phương thức GetAllServices không cần sửa
    public async Task<BaseResponse<IEnumerable<ServiceWithTotalPriceResponse>>> GetAllServices(int homestayId)
    {
        var serviceList = await _servicesRepository.GetAllServiceAsync(homestayId);
        if (serviceList == null)
        {
            return new BaseResponse<IEnumerable<ServiceWithTotalPriceResponse>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
        }

        var serviceDtos = _mapper.Map<IEnumerable<ServiceWithTotalPriceResponse>>(serviceList);
        return new BaseResponse<IEnumerable<ServiceWithTotalPriceResponse>>("Get all Services by HomeStayID success",
            StatusCodeEnum.OK_200, serviceDtos);
    }

    private async Task<List<string>> UploadImagesToCloudinary(List<IFormFile> files)
    {
        var urls = new List<string>();

        if (files == null || !files.Any())
        {
            return urls;
        }

        foreach (var file in files)
        {
            if (file == null || file.Length == 0)
                continue;

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "ServiceImages"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.StatusCode == HttpStatusCode.OK)
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
}