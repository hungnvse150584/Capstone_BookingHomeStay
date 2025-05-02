using AutoMapper;
using BusinessObject.Model;
using Microsoft.AspNetCore.Http;
using Repository.IRepositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Rating;
using Service.RequestAndResponse.Response.Ratings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class RatingService : IRatingService
    {
        private readonly IMapper _mapper;
        private readonly IRatingRepository _ratingRepository;
        private readonly IBookingRepository _bookingRepository; // Thêm IBookingRepository

        public RatingService(
            IMapper mapper,
            IRatingRepository ratingRepository,
            IBookingRepository bookingRepository)
        {
            _mapper = mapper;
            _ratingRepository = ratingRepository;
            _bookingRepository = bookingRepository;
        }

        // Tạo Rating mới
        public async Task<BaseResponse<CreateRatingResponse>> CreateRatingAsync(CreateRatingRequest request)
        {
            try
            {
                // Step 1: Kiểm tra dữ liệu bắt buộc
                if (string.IsNullOrEmpty(request.AccountID) || request.HomeStayID <= 0 || request.BookingID <= 0)
                {
                    return new BaseResponse<CreateRatingResponse>(
                        "Required fields are missing (AccountID, HomeStayID, BookingID)!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                if (request.CleaningRate == null || request.ServiceRate == null || request.FacilityRate == null)
                {
                    return new BaseResponse<CreateRatingResponse>(
                        "Rating fields (CleaningRate, ServiceRate, FacilityRate) are required!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                if (request.CleaningRate < 0 || request.ServiceRate < 0 || request.FacilityRate < 0)
                {
                    return new BaseResponse<CreateRatingResponse>(
                        "Rating values must be non-negative!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }
                // Step 2: Kiểm tra xem Booking có tồn tại và hợp lệ không
                var booking = await _bookingRepository.GetBookingByIdAsync(request.BookingID);
                if (booking == null)
                {
                    return new BaseResponse<CreateRatingResponse>(
                        "Booking not found!",
                        StatusCodeEnum.NotFound_404,
                        null);
                }

                // Thêm logging để debug
                Console.WriteLine($"BookingID: {booking.BookingID}, Status: {(int)booking.Status}, Status Enum: {booking.Status}");

                // Kiểm tra xem booking có thuộc về AccountID và HomeStayID không
                if (booking.AccountID != request.AccountID || booking.HomeStayID != request.HomeStayID)
                {
                    return new BaseResponse<CreateRatingResponse>(
                        "Booking does not belong to this AccountID or HomeStayID!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                // Step 3: Kiểm tra trạng thái của Booking
                if (booking.Status != BookingStatus.Completed)
                {
                    return new BaseResponse<CreateRatingResponse>(
                        "You can only rate after the booking is completed!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }
            

                // Step 4: Kiểm tra xem Booking đã có Rating chưa
                if (booking.RatingID != null)
                {
                    return new BaseResponse<CreateRatingResponse>(
                        "A rating already exists for this booking!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                // Step 5: Tính SumRate từ 3 giá trị CleaningRate, ServiceRate, FacilityRate
                double sumRate = (request.CleaningRate.Value + request.ServiceRate.Value + request.FacilityRate.Value) / 3.0;

                // Step 6: Tạo Rating
                var rating = new Rating
                {
                    SumRate = sumRate,
                    CleaningRate = request.CleaningRate.Value,
                    ServiceRate = request.ServiceRate.Value,
                    FacilityRate = request.FacilityRate.Value,
                    Content = request.Content,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    AccountID = request.AccountID,
                    HomeStayID = request.HomeStayID,
                    BookingID = request.BookingID // Gán BookingID
                };

                await _ratingRepository.AddAsync(rating);
                await _ratingRepository.SaveChangesAsync();

                // Step 7: Cập nhật Booking để liên kết với Rating
                booking.RatingID = rating.RatingID;
                await _bookingRepository.UpdateBookingAsync(booking);

                // Step 8: Ánh xạ dữ liệu để trả về
                var response = _mapper.Map<CreateRatingResponse>(rating);

                return new BaseResponse<CreateRatingResponse>(
                    "Rating created successfully!",
                    StatusCodeEnum.Created_201,
                    response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<CreateRatingResponse>(
                    $"Something went wrong! Error: {ex.Message}. Inner Exception: {ex.InnerException?.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }

        // Cập nhật Rating
        public async Task<BaseResponse<CreateRatingResponse>> UpdateRatingAsync(int ratingId, UpdateRatingRequest request)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (ratingId <= 0)
                {
                    return new BaseResponse<CreateRatingResponse>(
                        "RatingID must be greater than 0",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                if (request.CleaningRate < 0 || request.ServiceRate < 0 || request.FacilityRate < 0)
                {
                    return new BaseResponse<CreateRatingResponse>(
                        "Rating values must be non-negative!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                // Kiểm tra Rating tồn tại
                var rating = await _ratingRepository.GetByIdAsync(ratingId);
                if (rating == null)
                {
                    return new BaseResponse<CreateRatingResponse>(
                        "Rating not found!",
                        StatusCodeEnum.NotFound_404,
                        null);
                }

                // Tính lại SumRate từ 3 giá trị mới
                double sumRate = (request.CleaningRate + request.ServiceRate + request.FacilityRate) / 3.0;

                // Cập nhật thông tin Rating
                rating.SumRate = sumRate;
                rating.CleaningRate = request.CleaningRate;
                rating.ServiceRate = request.ServiceRate;
                rating.FacilityRate = request.FacilityRate;
                rating.Content = request.Content;
                rating.UpdatedAt = DateTime.UtcNow;

                await _ratingRepository.UpdateAsync(rating);
                await _ratingRepository.SaveChangesAsync();

                var response = _mapper.Map<CreateRatingResponse>(rating);
                return new BaseResponse<CreateRatingResponse>(
                    "Rating updated successfully!",
                    StatusCodeEnum.OK_200,
                    response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<CreateRatingResponse>(
                    $"Something went wrong! Error: {ex.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }

        // Xóa Rating
        public async Task<BaseResponse<string>> DeleteRatingAsync(int ratingId)
        {
            try
            {
                var rating = await _ratingRepository.GetByIdAsync(ratingId);
                if (rating == null)
                {
                    return new BaseResponse<string>(
                        "Rating not found!",
                        StatusCodeEnum.NotFound_404,
                        null);
                }

                await _ratingRepository.DeleteAsync(rating);
                return new BaseResponse<string>(
                    "Rating deleted successfully!",
                    StatusCodeEnum.OK_200,
                    "Deleted successfully");
            }
            catch (Exception ex)
            {
                return new BaseResponse<string>(
                    $"Something went wrong! Error: {ex.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }

        // Lấy Rating theo HomeStayID
        public async Task<BaseResponse<IEnumerable<CreateRatingResponse>>> GetRatingByHomeStayIdAsync(int homeStayId)
        {
            try
            {
                var ratings = await _ratingRepository.GetRatingByHomeStayId(homeStayId);
                var response = _mapper.Map<IEnumerable<CreateRatingResponse>>(ratings);
                return new BaseResponse<IEnumerable<CreateRatingResponse>>(
                    "Ratings retrieved successfully!",
                    StatusCodeEnum.OK_200,
                    response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<CreateRatingResponse>>(
                    $"Something went wrong! Error: {ex.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }

        // Lấy Rating theo AccountID
        public async Task<BaseResponse<IEnumerable<CreateRatingResponse>>> GetRatingByAccountIdAsync(string accountId)
        {
            try
            {
                var ratings = await _ratingRepository.GetRatingByAccountId(accountId);
                var response = _mapper.Map<IEnumerable<CreateRatingResponse>>(ratings);
                return new BaseResponse<IEnumerable<CreateRatingResponse>>(
                    "Ratings retrieved successfully!",
                    StatusCodeEnum.OK_200,
                    response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<CreateRatingResponse>>(
                    $"Something went wrong! Error: {ex.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }

        // Lấy Rating theo AccountID và HomeStayID
        public async Task<BaseResponse<CreateRatingResponse>> GetRatingByUserIdAndHomeStayAsync(string accountId, int homeStayId)
        {
            try
            {
                var rating = await _ratingRepository.GetRatingByUserIdAndHomeStay(accountId, homeStayId);
                var response = _mapper.Map<CreateRatingResponse>(rating);
                return new BaseResponse<CreateRatingResponse>(
                    "Rating retrieved successfully!",
                    StatusCodeEnum.OK_200,
                    response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<CreateRatingResponse>(
                    $"Something went wrong! Error: {ex.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }

        // Lấy điểm trung bình Rating theo HomeStayID
        public async Task<BaseResponse<double>> GetAverageRatingAsync(int homeStayId)
        {
            try
            {
                var average = await _ratingRepository.GetAverageRating(homeStayId);
                return new BaseResponse<double>(
                    "Average rating retrieved successfully!",
                    StatusCodeEnum.OK_200,
                    average);
            }
            catch (Exception ex)
            {
                return new BaseResponse<double>(
                    $"Something went wrong! Error: {ex.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    0);
            }
        }
    }
}