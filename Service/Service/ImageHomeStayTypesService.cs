using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.ImageHomeStayTypes;
using Service.RequestAndResponse.Request.ImageService;
using Service.RequestAndResponse.Response.ImageHomeStayTypes;
using Service.RequestAndResponse.Response.ImageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class ImageHomeStayTypesService : IImageHomeStayTypesService
    {
        private readonly IMapper _mapper;
        private readonly IImageHomeStayTypesRepository _imageTypeRepository;

        public ImageHomeStayTypesService(IMapper mapper, IImageHomeStayTypesRepository imageTypeRepository)
        {
            _mapper = mapper;
            _imageTypeRepository = imageTypeRepository;
        }

        public async Task<BaseResponse<AddImageHomeStayTypesRequest>> CreateImageHomeStayTypes(AddImageHomeStayTypesRequest imageTypesRequest)
        {
            ImageHomeStayRentals imageType = _mapper.Map<ImageHomeStayRentals>(imageTypesRequest);
            await _imageTypeRepository.AddAsync(imageType);
            var response = _mapper.Map<AddImageHomeStayTypesRequest>(imageType);
            return new BaseResponse<AddImageHomeStayTypesRequest>("Create image tpye success", StatusCodeEnum.Created_201, response);
        }

        public async Task<BaseResponse<string>> DeleteImageHomeStayTypes(int id)
        {
            var imageType = await _imageTypeRepository.GetByIdAsync(id);
            await _imageTypeRepository.DeleteAsync(imageType);
            return new BaseResponse<string>("Delete image type success", StatusCodeEnum.OK_200, "Deleted successfully");
        }

        public async Task<BaseResponse<IEnumerable<GetAllImageHomeStayType>>> GetAllImageHomeStayTypes()
        {
            IEnumerable<ImageHomeStayRentals> imageType = await _imageTypeRepository.GetAllAsync();
            if (imageType == null)
            {
                return new BaseResponse<IEnumerable<GetAllImageHomeStayType>>(
                    "Something went wrong!",
                    StatusCodeEnum.BadGateway_502,
                    null);
            }


            var response = _mapper.Map<IEnumerable<GetAllImageHomeStayType>>(imageType);


            if (response == null)
            {
                return new BaseResponse<IEnumerable<GetAllImageHomeStayType>>(
                    "Something went wrong!",
                    StatusCodeEnum.BadGateway_502,
                    null);
            }

 
            return new BaseResponse<IEnumerable<GetAllImageHomeStayType>>(
                "Get all image services success",
                StatusCodeEnum.OK_200,
                response);
        }

        public async Task<BaseResponse<GetAllImageHomeStayType>> GetImageHomeStayTypesById(int id)
        {
            var imageService = await _imageTypeRepository.GetImageHomeStayTypesByIdAsync(id);
            var response = _mapper.Map<GetAllImageHomeStayType>(imageService);
            return new BaseResponse<GetAllImageHomeStayType>("Get image type by id success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<UpdateImageHomeStayTypesRequest>> UpdateImageHomeStayTypes(int id, UpdateImageHomeStayTypesRequest imageTypesRequest)
        {
            ImageHomeStayRentals imageType = await _imageTypeRepository.GetByIdAsync(id);
            _mapper.Map(imageTypesRequest, imageType);
            await _imageTypeRepository.UpdateAsync(imageType);
            var response = _mapper.Map<UpdateImageHomeStayTypesRequest>(imageType);
            return new BaseResponse<UpdateImageHomeStayTypesRequest>("Update image tpye success", StatusCodeEnum.OK_200, response);
        }
    }
}
