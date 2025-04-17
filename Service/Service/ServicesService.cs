﻿using AutoMapper;
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

    public async Task<BaseResponse<Services>> CreateService(CreateServices request)
    {
        try
        {
            // Ánh xạ dữ liệu từ request sang entity
            var serviceEntity = _mapper.Map<Services>(request);
            serviceEntity.CreateAt = DateTime.UtcNow;
            serviceEntity.UpdateAt = DateTime.UtcNow;

            // Lưu service
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

            return new BaseResponse<Services>("Service created successfully", StatusCodeEnum.Created_201, serviceEntity);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            return new BaseResponse<Services>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
        }
    }

    public async Task<BaseResponse<Services>> UpdateService(int serviceId, UpdateServices request)
    {
        try
        {
            // Kiểm tra service có tồn tại không
            var serviceExist = await _servicesRepository.GetByIdAsync(serviceId);
            if (serviceExist == null)
            {
                return new BaseResponse<Services>("Cannot find service", StatusCodeEnum.BadGateway_502, null);
            }

            // Ánh xạ dữ liệu từ request sang entity
            var updatedService = _mapper.Map(request, serviceExist);
            updatedService.CreateAt = serviceExist.CreateAt;
            updatedService.UpdateAt = DateTime.UtcNow;

            // Lưu service
            await _servicesRepository.UpdateAsync(updatedService);
            await _servicesRepository.SaveChangesAsync();

            //// Upload ảnh nếu có
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

            return new BaseResponse<Services>("Update Service successfully", StatusCodeEnum.OK_200, updatedService);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            return new BaseResponse<Services>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
        }
    }

    public async Task<BaseResponse<IEnumerable<GetAllServices>>> GetAllServices(int homestayId)
    {
        var serviceList = await _servicesRepository.GetAllServiceAsync(homestayId);
        if (serviceList == null)
        {
            return new BaseResponse<IEnumerable<GetAllServices>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
        }

        var serviceDtos = _mapper.Map<IEnumerable<GetAllServices>>(serviceList);
        return new BaseResponse<IEnumerable<GetAllServices>>("Get all Services by HomeStayID success",
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