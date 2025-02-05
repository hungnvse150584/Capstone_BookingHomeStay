using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.ImageService;
using Service.RequestAndResponse.Response.ImageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class ImageServicesService : IImageServicesService
    {
        private readonly IMapper _mapper;
        private readonly IImageServicesRepository _imageServicesRepository;

        public ImageServicesService(IMapper mapper, IImageServicesRepository imageServicesRepository)
        {
            _mapper = mapper;
            _imageServicesRepository = imageServicesRepository;
        }

        public async Task<BaseResponse<IEnumerable<GetAllImageService>>> GetAllImageServices()
        {        
            IEnumerable<ImageServices> imageServices = await _imageServicesRepository.GetAllAsync();
            if (imageServices == null)
            {
                return new BaseResponse<IEnumerable<GetAllImageService>>(
                    "Something went wrong!",
                    StatusCodeEnum.BadGateway_502,
                    null);
            }


            var response = _mapper.Map<IEnumerable<GetAllImageService>>(imageServices);


            if (response == null)
            {
                return new BaseResponse<IEnumerable<GetAllImageService>>(
                    "Something went wrong!",
                    StatusCodeEnum.BadGateway_502,
                    null);
            }

            return new BaseResponse<IEnumerable<GetAllImageService>>(
                "Get all image services success",
                StatusCodeEnum.OK_200,
                response);
        }


        public async Task<BaseResponse<GetAllImageService>> GetImageServiceById(int id)
        {
            var imageService = await _imageServicesRepository.GetImageServiceByIdAsync(id);
            var response = _mapper.Map<GetAllImageService>(imageService);
            return new BaseResponse<GetAllImageService>("Get image service by id success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<AddImageServicesRequest>> CreateImageService(AddImageServicesRequest imageServiceRequest)
        {
            ImageServices imageService = _mapper.Map<ImageServices>(imageServiceRequest);
            await _imageServicesRepository.AddAsync(imageService);
            var response = _mapper.Map<AddImageServicesRequest>(imageService);
            return new BaseResponse<AddImageServicesRequest>("Create image service success", StatusCodeEnum.Created_201, response);
        }

        public async Task<BaseResponse<UpdateImageServicesRequest>> UpdateImageService(int id, UpdateImageServicesRequest imageServiceRequest)
        {
            ImageServices imageService = await _imageServicesRepository.GetByIdAsync(id);
            _mapper.Map(imageServiceRequest, imageService);
            await _imageServicesRepository.UpdateAsync(imageService);
            var response = _mapper.Map<UpdateImageServicesRequest>(imageService);
            return new BaseResponse<UpdateImageServicesRequest>("Update image service success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<string>> DeleteImageService(int id)
        {
            var imageService = await _imageServicesRepository.GetByIdAsync(id);
            await _imageServicesRepository.DeleteAsync(imageService);
            return new BaseResponse<string>("Delete image service success", StatusCodeEnum.OK_200, "Deleted successfully");
        }
    }
}
