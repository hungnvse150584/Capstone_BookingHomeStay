using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.CancellationPolicy;
using Service.RequestAndResponse.Request.CommissionRates;
using Service.RequestAndResponse.Response.CancellationPolicyRequest;
using Service.RequestAndResponse.Response.CommissionRate;
using Service.RequestAndResponse.Response.HomeStays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class CancellationPolicyService : ICancellationPolicyService
    {
        private readonly IMapper _mapper;
        private readonly ICancellationPolicyRepository _cancellationPolicyRepository;
        private readonly IHomeStayRepository _homeStayRepository;

        public CancellationPolicyService(IMapper mapper, ICancellationPolicyRepository cancellationPolicyRepository, IHomeStayRepository homestayRepository)
        {
            _mapper = mapper;
            _cancellationPolicyRepository = cancellationPolicyRepository;
            _homeStayRepository = homestayRepository;
        }

        public async Task<BaseResponse<CreateCancellationPolicyRequest>> CreateCancellationPolicyRequest(CreateCancellationPolicyRequest typeRequest)
        {
            try
            {
                var homeStay = await _homeStayRepository.GetHomeStayDetailByIdAsync(typeRequest.HomeStayID);
                if (homeStay == null)
                {
                    return new BaseResponse<CreateCancellationPolicyRequest>("Cannot find HomeStay", StatusCodeEnum.BadGateway_502, null);
                }

                // Kiểm tra xem HomeStay đã có CommissionRate chưa
                if (homeStay.CancellationID.HasValue)
                {
                    return new BaseResponse<CreateCancellationPolicyRequest>(
                        "HomeStay already has a CancellationPolicy. Use UpdateCommissionRate to modify it.",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }
                // Ánh xạ từ request sang entity CancellationPolicy
                var cancellationPolicy = _mapper.Map<CancellationPolicy>(typeRequest);

                // Đặt giá trị CreateAt và UpdateAt
                cancellationPolicy.CreateAt = DateTime.UtcNow;
                cancellationPolicy.UpdateAt = DateTime.UtcNow;

                // Thêm vào cơ sở dữ liệu
                var createdPolicy = await _cancellationPolicyRepository.AddAsync(cancellationPolicy);

                homeStay.CancellationID = cancellationPolicy.CancellationID;
                await _homeStayRepository.UpdateAsync(homeStay);
                await _homeStayRepository.SaveChangesAsync();

                // Ánh xạ từ entity sang response
                var response = _mapper.Map<CreateCancellationPolicyRequest>(createdPolicy);

                return new BaseResponse<CreateCancellationPolicyRequest>("Cancellation Policy created successfully", StatusCodeEnum.Created_201, response);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi tạo
                return new BaseResponse<CreateCancellationPolicyRequest>($"An error occurred: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
        }

        public async Task<BaseResponse<string>> DeleteCancellationPolicy(int id)
        {
            var cp = await _cancellationPolicyRepository.GetByIdAsync(id);
            await _cancellationPolicyRepository.DeleteAsync(cp);
            return new BaseResponse<string>("Delete Cancellation Policy success", StatusCodeEnum.OK_200, "Deleted successfully");
        }

        public async Task<BaseResponse<GetAllCancellationPolicy>> GetCancellationPolicyByHomeStay(int? homeStayID)
        {
            var cp = await _cancellationPolicyRepository.GetCancellationPolicyByHomeStayAsync(homeStayID);
            if (cp == null)
            {
                return new BaseResponse<GetAllCancellationPolicy>("Cancellation Policy not found", StatusCodeEnum.NotFound_404, null);
            }

            var response = _mapper.Map<GetAllCancellationPolicy>(cp);
            return new BaseResponse<GetAllCancellationPolicy>("Successfully retrieved Cancellation Policy for HomeStay", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<GetAllCancellationPolicy>> GetCancellationPolicyByID(int cancellationID)
        {
            var cp = await _cancellationPolicyRepository.GetByIdAsync(cancellationID);
            if (cp == null)
            {
                return new BaseResponse<GetAllCancellationPolicy>("Cancellation Policy not found", StatusCodeEnum.NotFound_404, null);
            }

            var response = _mapper.Map<GetAllCancellationPolicy>(cp);
            return new BaseResponse<GetAllCancellationPolicy>("Successfully retrieved Cancellation Policy for HomeStay", StatusCodeEnum.OK_200, response);
        }

        public async Task<BaseResponse<IEnumerable<GetAllCancellationPolicy>>> GetAllCancellationPolicy()
        {
            IEnumerable<CancellationPolicy> cp = await _cancellationPolicyRepository.GetAllAsync();
            if (cp == null)
            {
                return new BaseResponse<IEnumerable<GetAllCancellationPolicy>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var commissionRates = _mapper.Map<IEnumerable<GetAllCancellationPolicy>>(cp);
            if (commissionRates == null)
            {
                return new BaseResponse<IEnumerable<GetAllCancellationPolicy>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllCancellationPolicy>>("Get all Cancellation Policy as base success",
                StatusCodeEnum.OK_200, commissionRates);
        }

        public async Task<BaseResponse<UpdateCancellationPolicyRequest>> UpdateCancellationPolicyRequest(UpdateCancellationPolicyRequest typeRequest)
        {
            try
            {
                // Lấy đối tượng CancellationPolicy theo ID
                var policy = await _cancellationPolicyRepository.GetByIdAsync(typeRequest.CancellationID);

                if (policy == null)
                {
                    return new BaseResponse<UpdateCancellationPolicyRequest>("Cancellation Policy not found", StatusCodeEnum.NotFound_404, null);
                }

                // Cập nhật thông tin
                policy.DayBeforeCancel = typeRequest.DayBeforeCancel;
                policy.RefundPercentage = typeRequest.RefundPercentage;
                policy.UpdateAt = DateTime.UtcNow;

                // Cập nhật vào cơ sở dữ liệu
                var updatedPolicy = await _cancellationPolicyRepository.UpdateAsync(policy);

                // Ánh xạ từ entity sang response
                var response = _mapper.Map<UpdateCancellationPolicyRequest>(updatedPolicy);

                return new BaseResponse<UpdateCancellationPolicyRequest>("Cancellation Policy updated successfully", StatusCodeEnum.OK_200, response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<UpdateCancellationPolicyRequest>($"An error occurred: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
        }
    }
}
