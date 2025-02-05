using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.District;
using Service.RequestAndResponse.Request.HomeStayType;
using Service.RequestAndResponse.Request.Properties;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.Properties;
using Service.RequestAndResponse.Response.Services;
using Service.RequestAndResponse.Response.Streets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class HomeStayTypeService : IHomeStayTypeService
    {
        private readonly IMapper _mapper;
        private readonly IHomeStayTypeRepository _homeStayTypeRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IServiceRepository _serviceRepository;

        public HomeStayTypeService(IMapper mapper, IHomeStayTypeRepository homeStayTypeRepository,
            IPropertyRepository propertyRepository, IServiceRepository serviceRepository)
        {
            _mapper = mapper;
            _homeStayTypeRepository = homeStayTypeRepository;
            _propertyRepository = propertyRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<BaseResponse<IEnumerable<GetAllHomeStayType>>> GetAllHomeStayTypes()
        {
            IEnumerable<HomeStayTypes> homeStayType = await _homeStayTypeRepository.GetAllAsync();
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
            HomeStayTypes homeStayTypes = _mapper.Map<HomeStayTypes>(typeRequest);
            await _homeStayTypeRepository.AddAsync(homeStayTypes);

            var response = _mapper.Map<CreateHomeStayTypeRequest>(homeStayTypes);
            return new BaseResponse<CreateHomeStayTypeRequest>("Add HomeStayType as base success", StatusCodeEnum.Created_201, response);
        }

        public async Task<BaseResponse<IEnumerable<GetAllProperties>>> GetAllProperties()
        {
            IEnumerable<Property> property = await _propertyRepository.GetAllAsync();
            if (property == null)
            {
                return new BaseResponse<IEnumerable<GetAllProperties>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var properties = _mapper.Map<IEnumerable<GetAllProperties>>(property);
            if (properties == null)
            {
                return new BaseResponse<IEnumerable<GetAllProperties>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllProperties>>("Get all properties as base success",
                StatusCodeEnum.OK_200, properties);
        }

        public async Task<BaseResponse<CreatePropertyRequest>> CreateProperties(CreatePropertyRequest propertyRequest)
        {
            Property properties = _mapper.Map<Property>(propertyRequest);
            await _propertyRepository.AddAsync(properties);

            var response = _mapper.Map<CreatePropertyRequest>(properties);
            return new BaseResponse<CreatePropertyRequest>("Add Property as base success", StatusCodeEnum.Created_201, response);
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
    }
}
