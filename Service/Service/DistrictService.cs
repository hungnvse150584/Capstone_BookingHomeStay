using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.District;
using Service.RequestAndResponse.Request.Province;
using Service.RequestAndResponse.Response.Districts;
using Service.RequestAndResponse.Response.Provinces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class DistrictService : IDistrictService
    {
        private readonly IMapper _mapper;
        private readonly IDistrictRepository _districtRepository;

        public DistrictService(IMapper mapper, IDistrictRepository districtRepository)
        {
            _mapper = mapper;
            _districtRepository = districtRepository;
        }

        public async Task<BaseResponse<AddDistrictRequest>> CreateDistrictFromBase(AddDistrictRequest districtRequest)
        {
            District district = _mapper.Map<District>(districtRequest);
            await _districtRepository.AddAsync(district);

            var response = _mapper.Map<AddDistrictRequest>(district);
            return new BaseResponse<AddDistrictRequest>("Add district as base success", StatusCodeEnum.Created_201, response);
        }

        public async Task<BaseResponse<IEnumerable<GetAllDistrict>>> GetAllDistrictFromBase()
        {
            IEnumerable<District> district = await _districtRepository.GetAllAsync();
            if (district == null)
            {
                return new BaseResponse<IEnumerable<GetAllDistrict>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var districts = _mapper.Map<IEnumerable<GetAllDistrict>>(district);
            if (districts == null)
            {
                return new BaseResponse<IEnumerable<GetAllDistrict>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllDistrict>>("Get all district as base success",
                StatusCodeEnum.OK_200, districts);
        }

        public async Task<BaseResponse<GetAllDistrict>> GetDistrictByIdFromBase(int id)
        {
            District district = await _districtRepository.GetByIdAsync(id);
            var result = _mapper.Map<GetAllDistrict>(district);
            return new BaseResponse<GetAllDistrict>("Get district as base success", StatusCodeEnum.OK_200, result);
        }

        public async Task<BaseResponse<UpdateDistrictRequest>> UpdateDistrictFromBase(int id, UpdateDistrictRequest districtRequest)
        {
            District districtExist = await _districtRepository.GetByIdAsync(id);
            _mapper.Map(districtRequest, districtExist);
            await _districtRepository.UpdateAsync(districtExist);

            var result = _mapper.Map<UpdateDistrictRequest>(districtExist);
            return new BaseResponse<UpdateDistrictRequest>("Update district as base success", StatusCodeEnum.OK_200, result);
        }
    }
}
