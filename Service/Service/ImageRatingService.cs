using AutoMapper;
using BusinessObject.Model;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Repository.IRepositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.ImageRating;
using Service.RequestAndResponse.Response.ImageRating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Service
{
    public class ImageRatingService : IImageRatingService
    {
        private readonly IImageRatingRepository _imageRatingRepository;
        private readonly IRatingRepository _ratingRepository;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

        public ImageRatingService(IImageRatingRepository imageRatingRepository, IRatingRepository ratingRepository, IMapper mapper, Cloudinary cloudinary)
        {
            _imageRatingRepository = imageRatingRepository;
            _ratingRepository = ratingRepository;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }

        public async Task<BaseResponse<UploadImageRatingRequest>> UploadImageAsync(UploadImageRatingRequest request)
        {
            if (request == null || request.File == null || request.File.Length == 0)
                return new BaseResponse<UploadImageRatingRequest>("No file uploaded!", StatusCodeEnum.BadRequest_400, null);

            if (request.RatingID <= 0)
                return new BaseResponse<UploadImageRatingRequest>("RatingID must be a positive value", StatusCodeEnum.BadRequest_400, null);

            var rating = await _ratingRepository.GetByIdAsync(request.RatingID);
            if (rating == null)
                return new BaseResponse<UploadImageRatingRequest>($"Cannot find Rating with ID {request.RatingID}", StatusCodeEnum.NotFound_404, null);

            try
            {
                var uploadParams = new ImageUploadParams { File = new FileDescription(request.File.FileName, request.File.OpenReadStream()), Folder = "RatingImages" };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                    return new BaseResponse<UploadImageRatingRequest>($"Failed to upload image to Cloudinary: {uploadResult.Error.Message}", StatusCodeEnum.BadGateway_502, null);

                var imageRating = new ImageRating { Image = uploadResult.SecureUrl.ToString(), RatingID = request.RatingID };
                var savedImage = await _imageRatingRepository.AddImageAsync(imageRating);
                if (savedImage == null)
                    return new BaseResponse<UploadImageRatingRequest>("Failed to save image to database!", StatusCodeEnum.BadGateway_502, null);

                return new BaseResponse<UploadImageRatingRequest>("Upload image for Rating successfully", StatusCodeEnum.Created_201, request);
            }
            catch (Exception ex)
            {
                return new BaseResponse<UploadImageRatingRequest>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
        }

        public async Task<BaseResponse<ImageRatingResponse>> UpdateImageAsync(int imageId, UpdateImageRatingRequest request)
        {
            try
            {
                if (imageId <= 0)
                    return new BaseResponse<ImageRatingResponse>("ImageRatingID must be a positive value", StatusCodeEnum.BadRequest_400, null);

                if (request.File == null || request.File.Length == 0)
                    return new BaseResponse<ImageRatingResponse>("No file uploaded!", StatusCodeEnum.BadRequest_400, null);

                var existingImage = await _imageRatingRepository.GetImageByIdAsync(imageId);
                if (existingImage == null)
                    return new BaseResponse<ImageRatingResponse>($"ImageRating with ID {imageId} not found", StatusCodeEnum.NotFound_404, null);

                var publicId = ExtractPublicIdFromUrl(existingImage.Image);
                if (!string.IsNullOrEmpty(publicId))
                {
                    var deletionParams = new DeletionParams(publicId);
                    var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                    if (deletionResult.StatusCode != System.Net.HttpStatusCode.OK)
                        return new BaseResponse<ImageRatingResponse>($"Failed to delete old image from Cloudinary: {deletionResult.Error.Message}", StatusCodeEnum.InternalServerError_500, null);
                }

                var uploadParams = new ImageUploadParams { File = new FileDescription(request.File.FileName, request.File.OpenReadStream()), Folder = "RatingImages" };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                    return new BaseResponse<ImageRatingResponse>($"Failed to upload new image to Cloudinary: {uploadResult.Error.Message}", StatusCodeEnum.BadGateway_502, null);

                existingImage.Image = uploadResult.SecureUrl.ToString();
                var updatedImage = await _imageRatingRepository.UpdateImageAsync(existingImage);

                var response = _mapper.Map<ImageRatingResponse>(updatedImage);
                return new BaseResponse<ImageRatingResponse>("ImageRating updated successfully!", StatusCodeEnum.OK_200, response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<ImageRatingResponse>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
        }

        public async Task<BaseResponse<string>> DeleteImageAsync(int imageId)
        {
            try
            {
                var image = await _imageRatingRepository.GetImageByIdAsync(imageId);
                if (image == null)
                    return new BaseResponse<string>("ImageRating not found!", StatusCodeEnum.NotFound_404, null);

                var publicId = ExtractPublicIdFromUrl(image.Image);
                if (!string.IsNullOrEmpty(publicId))
                {
                    var deletionParams = new DeletionParams(publicId);
                    var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                    if (deletionResult.StatusCode != System.Net.HttpStatusCode.OK)
                        return new BaseResponse<string>($"Failed to delete image from Cloudinary: {deletionResult.Error.Message}", StatusCodeEnum.InternalServerError_500, null);
                }

                await _imageRatingRepository.DeleteAsync(image);
                return new BaseResponse<string>("ImageRating deleted successfully!", StatusCodeEnum.OK_200, "Deleted successfully");
            }
            catch (Exception ex)
            {
                return new BaseResponse<string>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
        }

        public async Task<BaseResponse<IEnumerable<ImageRatingResponse>>> GetImagesByRatingIdAsync(int ratingId)
        {
            try
            {
                var images = await _imageRatingRepository.GetImagesByRatingIdAsync(ratingId);
                var response = _mapper.Map<IEnumerable<ImageRatingResponse>>(images);
                return new BaseResponse<IEnumerable<ImageRatingResponse>>("Images retrieved successfully!", StatusCodeEnum.OK_200, response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<ImageRatingResponse>>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
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
    }
}