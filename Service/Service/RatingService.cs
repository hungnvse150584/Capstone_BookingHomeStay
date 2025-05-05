using AutoMapper;
using BusinessObject.Model;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Repository.IRepositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Rating;
using Service.RequestAndResponse.Response.ImageRating;
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
        private readonly IBookingRepository _bookingRepository;
        private readonly IImageRatingRepository _imageRatingRepository;
        private readonly Cloudinary _cloudinary;

        public RatingService(IMapper mapper, IRatingRepository ratingRepository, IBookingRepository bookingRepository, IImageRatingRepository imageRatingRepository, Cloudinary cloudinary)
        {
            _mapper = mapper;
            _ratingRepository = ratingRepository;
            _bookingRepository = bookingRepository;
            _imageRatingRepository = imageRatingRepository;
            _cloudinary = cloudinary;
        }

        // Tạo Rating mới
        public async Task<BaseResponse<CreateRatingResponse>> CreateRatingAsync(CreateRatingRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AccountID) || request.HomeStayID <= 0 || request.BookingID <= 0)
                    return new BaseResponse<CreateRatingResponse>("Required fields are missing!", StatusCodeEnum.BadRequest_400, null);

                if (request.CleaningRate == null || request.ServiceRate == null || request.FacilityRate == null)
                    return new BaseResponse<CreateRatingResponse>("Rating fields are required!", StatusCodeEnum.BadRequest_400, null);

                if (request.CleaningRate < 0 || request.ServiceRate < 0 || request.FacilityRate < 0)
                    return new BaseResponse<CreateRatingResponse>("Rating values must be non-negative!", StatusCodeEnum.BadRequest_400, null);

                var booking = await _bookingRepository.GetBookingByIdAsync(request.BookingID);
                if (booking == null)
                    return new BaseResponse<CreateRatingResponse>("Booking not found!", StatusCodeEnum.NotFound_404, null);

                if (booking.AccountID != request.AccountID || booking.HomeStayID != request.HomeStayID)
                    return new BaseResponse<CreateRatingResponse>("Booking does not belong to this AccountID or HomeStayID!", StatusCodeEnum.BadRequest_400, null);

                if (booking.Status != BookingStatus.Completed)
                    return new BaseResponse<CreateRatingResponse>("You can only rate after the booking is completed!", StatusCodeEnum.BadRequest_400, null);

                if (booking.RatingID != null)
                    return new BaseResponse<CreateRatingResponse>("A rating already exists for this booking!", StatusCodeEnum.BadRequest_400, null);

                double sumRate = (request.CleaningRate.Value + request.ServiceRate.Value + request.FacilityRate.Value) / 3.0;
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
                    BookingID = request.BookingID
                };

                await _ratingRepository.AddAsync(rating);
                await _ratingRepository.SaveChangesAsync();

                if (request.Images != null && request.Images.Any())
                {
                    var imageUrls = await UploadImagesToCloudinary(request.Images);
                    foreach (var url in imageUrls)
                    {
                        var imageRating = new ImageRating { Image = url, RatingID = rating.RatingID };
                        await _imageRatingRepository.AddImageAsync(imageRating);
                    }
                    await _imageRatingRepository.SaveChangesAsync();
                }

                // Cập nhật Booking
                booking.RatingID = rating.RatingID;
                booking.IsRating = true; // Đặt IsRating thành true khi Rating được tạo
                await _bookingRepository.UpdateBookingAsync(booking);

                var savedRating = await _ratingRepository.GetByIdAsync(rating.RatingID, includeAccount: true);
                var response = _mapper.Map<CreateRatingResponse>(savedRating);
                response.ImageRatings = _mapper.Map<ICollection<ImageRatingResponse>>(await _imageRatingRepository.GetImagesByRatingIdAsync(rating.RatingID));
                return new BaseResponse<CreateRatingResponse>("Rating created successfully!", StatusCodeEnum.Created_201, response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<CreateRatingResponse>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
        }

        // Cập nhật Rating
        public async Task<BaseResponse<CreateRatingResponse>> UpdateRatingAsync(int ratingId, UpdateRatingRequest request)
        {
            try
            {
                if (ratingId <= 0)
                    return new BaseResponse<CreateRatingResponse>("RatingID must be greater than 0", StatusCodeEnum.BadRequest_400, null);

                if (request.CleaningRate < 0 || request.ServiceRate < 0 || request.FacilityRate < 0)
                    return new BaseResponse<CreateRatingResponse>("Rating values must be non-negative!", StatusCodeEnum.BadRequest_400, null);

                var rating = await _ratingRepository.GetByIdAsync(ratingId, includeAccount: true);
                if (rating == null)
                    return new BaseResponse<CreateRatingResponse>("Rating not found!", StatusCodeEnum.NotFound_404, null);

                double sumRate = (double)((request.CleaningRate + request.ServiceRate + request.FacilityRate) / 3.0);
                rating.SumRate = sumRate;
                rating.CleaningRate = (double)request.CleaningRate;
                rating.ServiceRate = (double)request.ServiceRate;
                rating.FacilityRate = (double)request.FacilityRate;
                rating.Content = request.Content;
                rating.UpdatedAt = DateTime.UtcNow;

                if (request.Images != null && request.Images.Any())
                {
                    var existingImages = await _imageRatingRepository.GetImagesByRatingIdAsync(ratingId);
                    foreach (var existingImage in existingImages)
                    {
                        var publicId = ExtractPublicIdFromUrl(existingImage.Image);
                        if (!string.IsNullOrEmpty(publicId))
                        {
                            var deletionParams = new DeletionParams(publicId);
                            await _cloudinary.DestroyAsync(deletionParams);
                        }
                        await _imageRatingRepository.DeleteAsync(existingImage);
                    }

                    var imageUrls = await UploadImagesToCloudinary(request.Images);
                    foreach (var url in imageUrls)
                    {
                        var imageRating = new ImageRating { Image = url, RatingID = rating.RatingID };
                        await _imageRatingRepository.AddImageAsync(imageRating);
                    }
                    await _imageRatingRepository.SaveChangesAsync();
                }

                await _ratingRepository.UpdateAsync(rating);
                await _ratingRepository.SaveChangesAsync();

                var updatedRating = await _ratingRepository.GetByIdAsync(ratingId, includeAccount: true);
                var response = _mapper.Map<CreateRatingResponse>(updatedRating);
                response.ImageRatings = _mapper.Map<ICollection<ImageRatingResponse>>(await _imageRatingRepository.GetImagesByRatingIdAsync(rating.RatingID));
                return new BaseResponse<CreateRatingResponse>("Rating updated successfully!", StatusCodeEnum.OK_200, response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<CreateRatingResponse>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
        }

        private async Task<List<string>> UploadImagesToCloudinary(List<IFormFile> images)
        {
            var imageUrls = new List<string>();
            foreach (var image in images)
            {
                var uploadParams = new ImageUploadParams { File = new FileDescription(image.FileName, image.OpenReadStream()), Folder = "RatingImages" };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    imageUrls.Add(uploadResult.SecureUrl.ToString());
            }
            return imageUrls;
        }

        private string ExtractPublicIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;
            var parts = url.Split('/');
            var fileNameWithExtension = parts.Last();
            var publicId = fileNameWithExtension.Split('.').First();
            var folder = "RatingImages/";
            return $"{folder}{publicId}";
        }

        // Xóa Rating
        public async Task<BaseResponse<string>> DeleteRatingAsync(int ratingId)
        {
            try
            {
                var rating = await _ratingRepository.GetByIdAsync(ratingId);
                if (rating == null)
                    return new BaseResponse<string>("Rating not found!", StatusCodeEnum.NotFound_404, null);

                // Tìm Booking liên quan đến Rating
                var booking = await _bookingRepository.GetBookingByIdAsync((int)rating.BookingID);
                if (booking != null)
                {
                    booking.RatingID = null; // Xóa liên kết với Rating
                    booking.IsRating = false; // Đặt lại IsRating thành false
                    await _bookingRepository.UpdateBookingAsync(booking);
                }

                var images = await _imageRatingRepository.GetImagesByRatingIdAsync(ratingId);
                foreach (var image in images)
                {
                    var publicId = ExtractPublicIdFromUrl(image.Image);
                    if (!string.IsNullOrEmpty(publicId))
                    {
                        var deletionParams = new DeletionParams(publicId);
                        await _cloudinary.DestroyAsync(deletionParams);
                    }
                    await _imageRatingRepository.DeleteAsync(image);
                }

                await _ratingRepository.DeleteAsync(rating);
                return new BaseResponse<string>("Rating deleted successfully!", StatusCodeEnum.OK_200, "Deleted successfully");
            }
            catch (Exception ex)
            {
                return new BaseResponse<string>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
        }

        public async Task<BaseResponse<(IEnumerable<CreateRatingResponse> Data, int TotalCount)>> GetRatingByHomeStayIdAsync(int homeStayId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Khai báo rõ ràng kiểu dữ liệu của tuple
                var result = await _ratingRepository.GetRatingByHomeStayIdAsync(homeStayId, includeAccount: true, pageNumber, pageSize);
                var ratings = result.Data;
                var totalCount = result.TotalCount;
                var response = new List<CreateRatingResponse>();
                foreach (var rating in ratings)
                {
                    var ratingResponse = _mapper.Map<CreateRatingResponse>(rating);
                    ratingResponse.ImageRatings = _mapper.Map<ICollection<ImageRatingResponse>>(await _imageRatingRepository.GetImagesByRatingIdAsync(rating.RatingID));
                    response.Add(ratingResponse);
                }
                return new BaseResponse<(IEnumerable<CreateRatingResponse> Data, int TotalCount)>("Ratings retrieved successfully!", StatusCodeEnum.OK_200, (response, totalCount));
            }
            catch (Exception ex)
            {
                return new BaseResponse<(IEnumerable<CreateRatingResponse> Data, int TotalCount)>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, (null, 0));
            }
        }

        public async Task<BaseResponse<(IEnumerable<CreateRatingResponse> Data, int TotalCount)>> GetRatingByAccountIdAsync(string accountId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Khai báo rõ ràng kiểu dữ liệu của tuple
                var result = await _ratingRepository.GetRatingByAccountIdAsync(accountId, includeAccount: true, pageNumber, pageSize);
                var ratings = result.Data;
                var totalCount = result.TotalCount;
                var response = new List<CreateRatingResponse>();
                foreach (var rating in ratings)
                {
                    var ratingResponse = _mapper.Map<CreateRatingResponse>(rating);
                    ratingResponse.ImageRatings = _mapper.Map<ICollection<ImageRatingResponse>>(await _imageRatingRepository.GetImagesByRatingIdAsync(rating.RatingID));
                    response.Add(ratingResponse);
                }
                return new BaseResponse<(IEnumerable<CreateRatingResponse> Data, int TotalCount)>("Ratings retrieved successfully!", StatusCodeEnum.OK_200, (response, totalCount));
            }
            catch (Exception ex)
            {
                return new BaseResponse<(IEnumerable<CreateRatingResponse> Data, int TotalCount)>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, (null, 0));
            }
        }

        // Lấy Rating theo AccountID và HomeStayID
        public async Task<BaseResponse<(IEnumerable<CreateRatingResponse> Data, int TotalCount)>> GetRatingByUserIdAndHomeStayAsync(string accountId, int homeStayId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _ratingRepository.GetAllRatingByUserIdAndHomeStayAsync(accountId, homeStayId, includeAccount: true, pageNumber, pageSize);
                var response = new List<CreateRatingResponse>();
                foreach (var rating in result.Data)
                {
                    var ratingResponse = _mapper.Map<CreateRatingResponse>(rating);
                    ratingResponse.ImageRatings = _mapper.Map<ICollection<ImageRatingResponse>>(await _imageRatingRepository.GetImagesByRatingIdAsync(rating.RatingID));
                    response.Add(ratingResponse);
                }
                return new BaseResponse<(IEnumerable<CreateRatingResponse> Data, int TotalCount)>("Ratings retrieved successfully!", StatusCodeEnum.OK_200, (response, result.TotalCount));
            }
            catch (Exception ex)
            {
                return new BaseResponse<(IEnumerable<CreateRatingResponse> Data, int TotalCount)>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, (null, 0));
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