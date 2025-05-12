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
        private readonly IHomeStayRepository _homeStayRepository;

        public CommissionRateService(IMapper mapper, ICommissionRateRepository commissionRateRepository,
            IHomeStayRepository homeStayRepository)
        {
            _mapper = mapper;
            _commissionRateRepository = commissionRateRepository;
            _homeStayRepository = homeStayRepository;
        }


        public async Task<BaseResponse<CreateCommissionRateRequest>> CreateCommmissionRate(CreateCommissionRateRequest typeRequest)
        {
            var homeStay = await _homeStayRepository.GetHomeStayDetailByIdAsync(typeRequest.HomeStayID);
            if (homeStay == null)
            {
                return new BaseResponse<CreateCommissionRateRequest>("Cannot find HomeStay", StatusCodeEnum.BadGateway_502, null);
            }

            // Kiểm tra xem HomeStay đã có CommissionRate chưa
            if (homeStay.CommissionRateID.HasValue)
            {
                return new BaseResponse<CreateCommissionRateRequest>(
                    "HomeStay already has a CommissionRate. Use UpdateCommissionRate to modify it.",
                    StatusCodeEnum.BadRequest_400,
                    null);
            }

            if (typeRequest.HostShare + typeRequest.PlatformShare != 100.0)
            {
                return new BaseResponse<CreateCommissionRateRequest>("Total share (Host + Platform) must be 100%", StatusCodeEnum.BadRequest_400, null);
            }

            // Ánh xạ và tạo CommissionRate
            CommissionRate commissionRate = _mapper.Map<CommissionRate>(typeRequest);
            commissionRate.CreateAt = DateTime.UtcNow; // Đảm bảo CreateAt được thiết lập
            commissionRate.UpdateAt = DateTime.UtcNow; // Đảm bảo UpdateAt được thiết lập
            commissionRate.isAccepted = false;
            commissionRate.OwnerAccepted = null;
            await _commissionRateRepository.AddAsync(commissionRate);
            await _commissionRateRepository.SaveChangesAsync(); // Lưu để sinh CommissionRateID

            // Cập nhật CommissionRateID cho HomeStay
            homeStay.CommissionRateID = commissionRate.CommissionRateID;
            await _homeStayRepository.UpdateAsync(homeStay);
            await _homeStayRepository.SaveChangesAsync();

            // Ánh xạ response
            var response = _mapper.Map<CreateCommissionRateRequest>(commissionRate);
            return new BaseResponse<CreateCommissionRateRequest>("Add CommissionRate as base success", StatusCodeEnum.Created_201, response);
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
            return new BaseResponse<IEnumerable<GetAllCommissionRate>>("Get all CommissionRate as base success",
                StatusCodeEnum.OK_200, commissionRates);
        }

        public async Task<BaseResponse<UpdateCommissionRateRequest>> UpdateCommmissionRate(UpdateCommissionRateRequest typeRequest)
        {
            var commissionRate = await _commissionRateRepository.GetCommissionRateByHomeStay(typeRequest.CommissionRateID);      
            if (commissionRate == null)
            {
                return new BaseResponse<UpdateCommissionRateRequest>("CommissionRate not found", StatusCodeEnum.NotFound_404, null);
            }

            // Nếu Admin chấp nhận thì gán WantedHostShare → HostShare
            if (typeRequest.isAccepted == true && commissionRate.WantedHostShare.HasValue &&
               (commissionRate.WantedHostShare.Value > 0 && commissionRate.WantedHostShare.Value < 1))
            {
                commissionRate.HostShare = commissionRate.WantedHostShare.Value;
                commissionRate.PlatformShare = 100 - commissionRate.HostShare; // tự động tính lại
                commissionRate.isAccepted = true;
            }
            else if (typeRequest.isAccepted == false)
            {
                commissionRate.isAccepted = false;
            }

           /* commissionRate.HostShare = typeRequest.HostShare != null ? typeRequest.HostShare : commissionRate.HostShare;
            commissionRate.PlatformShare = typeRequest.PlatformShare != null ? typeRequest.PlatformShare : commissionRate.PlatformShare;*/
            commissionRate.UpdateAt = DateTime.UtcNow; 

            await _commissionRateRepository.UpdateAsync(commissionRate);
            await _commissionRateRepository.SaveChangesAsync();

            var response = _mapper.Map<UpdateCommissionRateRequest>(commissionRate);

            return new BaseResponse<UpdateCommissionRateRequest>("Update CommissionRate successfully", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<UpdateWantedCommissionRateForOwner>> UpdateWantedCommmisionRateForOwner(UpdateWantedCommissionRateForOwner typeRequest)
        {
            var commissionRate = await _commissionRateRepository.GetCommissionRateByHomeStay(typeRequest.CommissionRateID);
            if (commissionRate == null)
            {
                return new BaseResponse<UpdateWantedCommissionRateForOwner>("CommissionRate not found", StatusCodeEnum.NotFound_404, null);
            }
            if (typeRequest.WantedHostShare >= 0 && typeRequest.WantedHostShare < 1)
            {
                commissionRate.WantedHostShare = typeRequest.WantedHostShare;
            }
            else
            {
                return new BaseResponse<UpdateWantedCommissionRateForOwner>("WantedHostShare must be between 0 and 1", StatusCodeEnum.BadRequest_400, null);
            }

            if (typeRequest.ownerAccepted.HasValue)
            {
                commissionRate.OwnerAccepted = typeRequest.ownerAccepted.Value;
            }

            commissionRate.UpdateAt = DateTime.UtcNow;

            await _commissionRateRepository.UpdateAsync(commissionRate);
            await _commissionRateRepository.SaveChangesAsync();

            var response = _mapper.Map<UpdateWantedCommissionRateForOwner>(commissionRate);

            return new BaseResponse<UpdateWantedCommissionRateForOwner>("Update CommissionRate successfully", StatusCodeEnum.OK_200, response);
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
