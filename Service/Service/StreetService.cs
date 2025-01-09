using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.District;
using Service.RequestAndResponse.Request.Street;
using Service.RequestAndResponse.Response.Districts;
using Service.RequestAndResponse.Response.Streets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class StreetService : IStreetService
    {
        private readonly IMapper _mapper;
        private readonly IStreetRepository _streetRepository;

        public StreetService(IMapper mapper, IStreetRepository streetRepository)
        {
            _mapper = mapper;
            _streetRepository = streetRepository;
        }

        public async Task<BaseResponse<AddStreetRequest>> CreateStreetFromBase(AddStreetRequest streetRequest)
        {
            Street street = _mapper.Map<Street>(streetRequest);
            await _streetRepository.AddAsync(street);

            var response = _mapper.Map<AddStreetRequest>(street);
            return new BaseResponse<AddStreetRequest>("Add street as base success", StatusCodeEnum.Created_201, response);
        }

        public async Task<BaseResponse<IEnumerable<GetAllStreet>>> GetAllStreetFromBase()
        {
            IEnumerable<Street> street = await _streetRepository.GetAllAsync();
            if (street == null)
            {
                return new BaseResponse<IEnumerable<GetAllStreet>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var streets = _mapper.Map<IEnumerable<GetAllStreet>>(street);
            if (streets == null)
            {
                return new BaseResponse<IEnumerable<GetAllStreet>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllStreet>>("Get all street as base success",
                StatusCodeEnum.OK_200, streets);
        }

        public async Task<BaseResponse<GetAllStreet>> GetStreetByIdFromBase(int id)
        {
            Street street = await _streetRepository.GetByIdAsync(id);
            var result = _mapper.Map<GetAllStreet>(street);
            return new BaseResponse<GetAllStreet>("Get street as base success", StatusCodeEnum.OK_200, result);
        }

        public async Task<BaseResponse<UpdateStreetRequest>> UpdateStreetFromBase(int id, UpdateStreetRequest streetRequest)
        {
            Street streetExist = await _streetRepository.GetByIdAsync(id);
            _mapper.Map(streetRequest, streetExist);
            await _streetRepository.UpdateAsync(streetExist);

            var result = _mapper.Map<UpdateStreetRequest>(streetExist);
            return new BaseResponse<UpdateStreetRequest>("Update street as base success", StatusCodeEnum.OK_200, result);
        }
    }
}
