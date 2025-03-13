using AutoMapper;
using BusinessObject.Model;
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

namespace Service.Service
{
    public class HomeStayRentalService : IHomeStayTypeService
    {
        private readonly IMapper _mapper;
        private readonly IHomeStayRentalRepository _homeStayTypeRepository;
        private readonly IServiceRepository _serviceRepository;

        public HomeStayRentalService(IMapper mapper, IHomeStayRentalRepository homeStayTypeRepository,
             IServiceRepository serviceRepository)
        {
            _mapper = mapper;
            _homeStayTypeRepository = homeStayTypeRepository;
            _serviceRepository = serviceRepository;
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

        public async Task<BaseResponse<CreateHomeStayTypeRequest>> CreateHomeStayType(CreateHomeStayTypeRequest typeRequest)
        {
            HomeStayRentals homeStayTypes = _mapper.Map<HomeStayRentals>(typeRequest);
            await _homeStayTypeRepository.AddAsync(homeStayTypes);

            var response = _mapper.Map<CreateHomeStayTypeRequest>(homeStayTypes);
            return new BaseResponse<CreateHomeStayTypeRequest>("Add HomeStayType as base success", StatusCodeEnum.Created_201, response);
        }

        public async Task<BaseResponse<CreateServices>> CreateServices(CreateServices servicesRequest)
        {
            Services services = _mapper.Map<Services>(servicesRequest);
            await _serviceRepository.AddAsync(services);

            var response = _mapper.Map<CreateServices>(services);
            return new BaseResponse<CreateServices>("Add Services as base success", StatusCodeEnum.Created_201, response);
        }

        public async Task<BaseResponse<IEnumerable<GetAllServices>>> GetAllServices()
        {
            IEnumerable<Services> service = await _serviceRepository.GetAllAsync();
            if (service == null)
            {
                return new BaseResponse<IEnumerable<GetAllServices>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var services = _mapper.Map<IEnumerable<GetAllServices>>(service);
            if (services == null)
            {
                return new BaseResponse<IEnumerable<GetAllServices>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllServices>>("Get all services as base success",
                StatusCodeEnum.OK_200, services);
        }

        public async Task<BaseResponse<UpdateHomeStayTypeRequest>> UpdateHomeStayType(int id, UpdateHomeStayTypeRequest updateRequest)
        {
            HomeStayTypes homeStayType = await _homeStayTypeRepository.GetByIdAsync(id);
            if (homeStayType == null)
            {
                return new BaseResponse<UpdateHomeStayTypeRequest>("HomeStayType not found", StatusCodeEnum.NotFound_404, null);
            }

            _mapper.Map(updateRequest, homeStayType);
            await _homeStayTypeRepository.UpdateAsync(homeStayType);

            var response = _mapper.Map<UpdateHomeStayTypeRequest>(homeStayType);
            return new BaseResponse<UpdateHomeStayTypeRequest>("Update HomeStayType success", StatusCodeEnum.OK_200, response);
        }
    }
}
