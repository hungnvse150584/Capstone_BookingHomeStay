using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.District;
using Service.RequestAndResponse.Request.Ward;
using Service.RequestAndResponse.Response.Districts;
using Service.RequestAndResponse.Response.Wards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class WardService : IWardService
    {
        private readonly IMapper _mapper;
        private readonly IWardRepository _wardRepository;

        public WardService(IMapper mapper, IWardRepository wardRepository)
        {
            _mapper = mapper;
            _wardRepository = wardRepository;
        }

        public async Task<BaseResponse<AddWardRequest>> CreateWardFromBase(AddWardRequest wardRequest)
        {
            Ward ward = _mapper.Map<Ward>(wardRequest);
            await _wardRepository.AddAsync(ward);

            var response = _mapper.Map<AddWardRequest>(ward);
            return new BaseResponse<AddWardRequest>("Add ward as base success", StatusCodeEnum.Created_201, response);
        }

        public async Task<BaseResponse<IEnumerable<GetAllWard>>> GetAllWardFromBase()
        {
            IEnumerable<Ward> ward = await _wardRepository.GetAllAsync();
            if (ward == null)
            {
                return new BaseResponse<IEnumerable<GetAllWard>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var wards = _mapper.Map<IEnumerable<GetAllWard>>(ward);
            if (wards == null)
            {
                return new BaseResponse<IEnumerable<GetAllWard>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllWard>>("Get all ward as base success",
                StatusCodeEnum.OK_200, wards);
        }

        public async Task<BaseResponse<GetAllWard>> GetWardByIdFromBase(int id)
        {
            Ward ward = await _wardRepository.GetByIdAsync(id);
            var result = _mapper.Map<GetAllWard>(ward);
            return new BaseResponse<GetAllWard>("Get ward as base success", StatusCodeEnum.OK_200, result);
        }

        public async Task<BaseResponse<UpdateWardRequest>> UpdateWardFromBase(int id, UpdateWardRequest wardRequest)
        {
            Ward wardExist = await _wardRepository.GetByIdAsync(id);
            _mapper.Map(wardRequest, wardExist);
            await _wardRepository.UpdateAsync(wardExist);

            var result = _mapper.Map<UpdateWardRequest>(wardRequest);
            return new BaseResponse<UpdateWardRequest>("Update ward as base success", StatusCodeEnum.OK_200, result);
        }
    }
}
