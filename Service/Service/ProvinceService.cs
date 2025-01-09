using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Province;
using Service.RequestAndResponse.Response.Provinces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class ProvinceService : IProvinceService
    {
        private readonly IMapper _mapper;
        private readonly IProvinceRepository _provinceRepository;

        public ProvinceService(IMapper mapper, IProvinceRepository provinceRepoitory)
        {
            _mapper = mapper;
            _provinceRepository = provinceRepoitory;
        }

        public async Task<BaseResponse<AddProvinceRequest>> CreateProvinceFromBase(AddProvinceRequest provinceRequest)
        {
            Province province = _mapper.Map<Province>(provinceRequest);
            await _provinceRepository.AddAsync(province);

            var response = _mapper.Map<AddProvinceRequest>(province);
            return new BaseResponse<AddProvinceRequest>("Add province as base success", StatusCodeEnum.Created_201, response);
        }

        public async Task<BaseResponse<IEnumerable<GetAllProvince>>> GetAllProvinceFromBase()
        {
            IEnumerable<Province> province = await _provinceRepository.GetAllAsync();
            if (province == null)
            {
                return new BaseResponse<IEnumerable<GetAllProvince>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var provinces = _mapper.Map<IEnumerable<GetAllProvince>>(province);
            if (provinces == null)
            {
                return new BaseResponse<IEnumerable<GetAllProvince>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllProvince>>("Get all province as base success",
                StatusCodeEnum.OK_200, provinces);
        }

        public async Task<BaseResponse<GetAllProvince>> GetProvinceByIdFromBase(int id)
        {
           Province province = await _provinceRepository.GetByIdAsync(id);
           var result = _mapper.Map<GetAllProvince>(province);
           return new BaseResponse<GetAllProvince>("Get province as base success",StatusCodeEnum.OK_200, result);

        }

        public async Task<BaseResponse<UpdateProvinceRequest>> UpdateProvinceFromBase(int id, UpdateProvinceRequest provinceRequest)
        {
            Province provinceExist = await _provinceRepository.GetByIdAsync(id);
            _mapper.Map(provinceRequest, provinceExist);
            await _provinceRepository.UpdateAsync(provinceExist);

            var result = _mapper.Map<UpdateProvinceRequest>(provinceExist);
            return new BaseResponse<UpdateProvinceRequest>("Update Province as base success", StatusCodeEnum.OK_200, result);
        }
    }
}
