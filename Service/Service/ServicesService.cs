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
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Service.Service
{
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
               
                var serviceEntity = _mapper.Map<Services>(request);
                serviceEntity.CreateAt = DateTime.Now;


                // 
                await _servicesRepository.AddAsync(serviceEntity);
                await _servicesRepository.SaveChangesAsync(); 

             
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

        public async Task<BaseResponse<Services>> UpdateService(int serviceId, CreateServices request)
        {
            try
            {
         
                var serviceExist = await _servicesRepository.GetByIdAsync(serviceId);
                if (serviceExist == null)
                {
                    return new BaseResponse<Services>("Cannot find service", StatusCodeEnum.BadGateway_502, null);
                }

                var updatedService = _mapper.Map(request, serviceExist);

             
                updatedService.CreateAt = serviceExist.CreateAt;
                updatedService.UpdateAt = DateTime.Now; 

              
                await _servicesRepository.UpdateAsync(updatedService);
                await _servicesRepository.SaveChangesAsync();


                if (request.Images != null && request.Images.Any())
                {
                    var imageUrls = await UploadImagesToCloudinary(request.Images);

                    foreach (var url in imageUrls)
                    {
                        var imageService = new ImageServices
                        {
                            Image = url,
                            ServicesID = updatedService.ServicesID
                        };
                        await _imageServiceRepository.AddImageAsync(imageService);
                    }
                
                    await _servicesRepository.SaveChangesAsync();
                }

                return new BaseResponse<Services>("Update Service successfully", StatusCodeEnum.OK_200, updatedService);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return new BaseResponse<Services>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
        }
        public async Task<BaseResponse<Services>> UpdateServiceByHomeStayId(int homeStayId, UpdateServices request)
        {
            try
            {
                // Lấy service đúng theo ID từ request
                var service = await _servicesRepository.GetByIdAsync(request.servicesID);

                if (service == null || service.HomeStayID != homeStayId)
                {
                    return new BaseResponse<Services>("Service not found for this HomeStay", StatusCodeEnum.NotFound_404, null);
                }

                // Ánh xạ dữ liệu
                var updatedService = _mapper.Map(request, service);
                updatedService.CreateAt = service.CreateAt;
                updatedService.UpdateAt = DateTime.Now;

                await _servicesRepository.UpdateAsync(updatedService);
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
                            ServicesID = updatedService.ServicesID
                        };
                        await _imageServiceRepository.AddImageAsync(imageService);
                    }
                    await _servicesRepository.SaveChangesAsync();
                }

                return new BaseResponse<Services>("Update Service successfully", StatusCodeEnum.OK_200, updatedService);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new BaseResponse<Services>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
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
}
