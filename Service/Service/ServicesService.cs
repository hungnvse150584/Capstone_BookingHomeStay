using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Service
{
    public class ServicesService : IServiceServices
    {
        private readonly IMapper _mapper;
        private readonly IServiceRepository _servicesRepository;

        public ServicesService(IMapper mapper, IServiceRepository servicesRepository)
        {
            _mapper = mapper;
            _servicesRepository = servicesRepository;
        }

        public async Task<BaseResponse<IEnumerable<GetAllServices>>> GetAllServices()
        {
            IEnumerable<Services> services = await _servicesRepository.GetAllAsync();
            if (services == null)
            {
                return new BaseResponse<IEnumerable<GetAllServices>>(
                    "Something went wrong!",
                    StatusCodeEnum.BadGateway_502,
                    null);
            }

            var response = _mapper.Map<IEnumerable<GetAllServices>>(services);
            return new BaseResponse<IEnumerable<GetAllServices>>(
                "Get all services success",
                StatusCodeEnum.OK_200,
                response);
        }

        public async Task<BaseResponse<GetAllServices>> GetServiceById(int id)
        {
            var service = await _servicesRepository.GetByIdAsync(id);
            if (service == null)
            {
                return new BaseResponse<GetAllServices>("Service not found", StatusCodeEnum.NotFound_404, null);
            }
            var response = _mapper.Map<GetAllServices>(service);
            return new BaseResponse<GetAllServices>("Get service by id success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<CreateServices>> CreateService(CreateServices serviceRequest)
        {
            Services service = _mapper.Map<Services>(serviceRequest);
            await _servicesRepository.AddAsync(service);
            var response = _mapper.Map<CreateServices>(service);
            return new BaseResponse<CreateServices>("Create service success", StatusCodeEnum.Created_201, response);
        }

        public async Task<BaseResponse<UpdateServices>> UpdateService(int id, UpdateServices serviceRequest)
        {
            var service = await _servicesRepository.GetByIdAsync(id);
            if (service == null)
            {
                return new BaseResponse<UpdateServices>("Service not found", StatusCodeEnum.NotFound_404, null);
            }
            _mapper.Map(serviceRequest, service);
            await _servicesRepository.UpdateAsync(service);
            var response = _mapper.Map<UpdateServices>(service);
            return new BaseResponse<UpdateServices>("Update service success", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<string>> DeleteService(int id)
        {
            var service = await _servicesRepository.GetByIdAsync(id);
            if (service == null)
            {
                return new BaseResponse<string>("Service not found", StatusCodeEnum.NotFound_404, null);
            }
            await _servicesRepository.DeleteAsync(service);
            return new BaseResponse<string>("Delete service success", StatusCodeEnum.OK_200, "Deleted successfully");
        }

       
    }
}
