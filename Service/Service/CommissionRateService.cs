using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.CommissionRates;
using Service.RequestAndResponse.Request.RoomType;
using Service.RequestAndResponse.Response.CommissionRate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class CommissionRateService : ICommissionRateService
    {
        private readonly IMapper _mapper;
        private readonly ICommissionRateRepository _commissionRateRepository;

        public CommissionRateService(IMapper mapper, ICommissionRateRepository commissionRateRepository)
        {
            _mapper = mapper;
            _commissionRateRepository = commissionRateRepository;
        }

        public async Task<BaseResponse<CreateCommissionRateRequest>> CreateCommmisionRate(CreateCommissionRateRequest typeRequest)
        {
            CommissionRate roomTypes = _mapper.Map<CommissionRate>(typeRequest);
            await _commissionRateRepository.AddAsync(roomTypes);

            var response = _mapper.Map<CreateCommissionRateRequest>(roomTypes);
            return new BaseResponse<CreateCommissionRateRequest>("Add HomeStayType as base success", StatusCodeEnum.Created_201, response);
        }

        public async Task<BaseResponse<IEnumerable<GetAllCommissionRate>>> GetAllCommissionRates()
        {
            IEnumerable<CommissionRate> commissionRate = await _commissionRateRepository.GetAllCommissionRate();
            if (commissionRate == null)
            {
                return new BaseResponse<IEnumerable<GetAllCommissionRate>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var commissionRates = _mapper.Map<IEnumerable<GetAllCommissionRate>>(commissionRate);
            if (commissionRates == null)
            {
                return new BaseResponse<IEnumerable<GetAllCommissionRate>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllCommissionRate>>("Get all RoomType as base success",
                StatusCodeEnum.OK_200, commissionRates);
        }

        public async Task<BaseResponse<UpdateCommissionRateRequest>> UpdateCommmisionRate(UpdateCommissionRateRequest typeRequest)
        {
            var commissionRate = await _commissionRateRepository.GetCommissionRateByHomeStay(typeRequest.CommissionRateID);      
            if (commissionRate == null)
            {
                return new BaseResponse<UpdateCommissionRateRequest>("CommissionRate not found", StatusCodeEnum.NotFound_404, null);
            }

            commissionRate.HostShare = typeRequest.HostShare != null ? typeRequest.HostShare : commissionRate.HostShare;
            commissionRate.PlatformShare = typeRequest.PlatformShare != null ? typeRequest.PlatformShare : commissionRate.PlatformShare;
            commissionRate.UpdateAt = DateTime.UtcNow; 

            await _commissionRateRepository.UpdateAsync(commissionRate);
            await _commissionRateRepository.SaveChangesAsync();

            var response = _mapper.Map<UpdateCommissionRateRequest>(commissionRate);

            return new BaseResponse<UpdateCommissionRateRequest>("Update CommissionRate successfully", StatusCodeEnum.OK_200, response);
        }
        public async Task<BaseResponse<GetAllCommissionRate>> GetCommissionRateByHomeStay(int homeStayID)
        {
            var commissionRate = await _commissionRateRepository.GetCommissionRateByHomeStay(homeStayID);
            if (commissionRate == null)
            {
                return new BaseResponse<GetAllCommissionRate>("CommissionRate not found", StatusCodeEnum.NotFound_404, null);
            }

            var response = _mapper.Map<GetAllCommissionRate>(commissionRate);
            return new BaseResponse<GetAllCommissionRate>("Successfully retrieved CommissionRate for HomeStay", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<string>> DeleteCommissionRate(int id)
        {
            var cp = await _commissionRateRepository.GetCommissionRateByHomeStay(id);
            await _commissionRateRepository.DeleteAsync(cp);
            return new BaseResponse<string>("Delete Commission Rate success", StatusCodeEnum.OK_200, "Deleted successfully");
        }
    }
}
