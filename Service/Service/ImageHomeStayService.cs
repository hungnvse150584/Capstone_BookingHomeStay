using BusinessObject.Model;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Repository.IRepositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.ImageHomeStay;
using System;
using System.Threading.Tasks;

namespace Service.Service
{
    public class ImageHomeStayService : IImageHomeStayService
    {
        private readonly IImageHomeStayRepository _imageHomeStayRepository;
        private readonly Cloudinary _cloudinary;

        public ImageHomeStayService(IImageHomeStayRepository imageHomeStayRepository, Cloudinary cloudinary)
        {
            _imageHomeStayRepository = imageHomeStayRepository;
            _cloudinary = cloudinary;
        }

        public async Task<BaseResponse<UploadImageRequest>> UploadImageAsync(UploadImageRequest request)
        {
            // Kiểm tra request đầu vào
            if (request == null || request.File == null || request.File.Length == 0)
            {
                return new BaseResponse<UploadImageRequest>("No file uploaded!", StatusCodeEnum.BadRequest_400, null);
            }

            // Kiểm tra HomeStay có tồn tại không
            var homeStay = await _imageHomeStayRepository.GetByIdAsync((int)request.HomeStayID);
            if (homeStay == null)
            {
                return new BaseResponse<UploadImageRequest>($"Cannot find HomeStay with ID {(int)request.HomeStayID}", StatusCodeEnum.NotFound_404, null);
            }

            try
            {
                // Upload file lên Cloudinary
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(request.File.FileName, request.File.OpenReadStream()),
                    Folder = "HomeStayImages"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new BaseResponse<UploadImageRequest>($"Failed to upload image to Cloudinary: {uploadResult.Error.Message}", StatusCodeEnum.BadGateway_502, null);
                }

                // Tạo đối tượng ImageHomeStay
                var imageHomeStay = new ImageHomeStay
                {
                    Image = uploadResult.SecureUrl.ToString(),
                    HomeStayID = (int)request.HomeStayID
                };

                // Lưu vào database
                var savedImage = await _imageHomeStayRepository.AddImageAsync(imageHomeStay);
                if (savedImage == null)
                {
                    return new BaseResponse<UploadImageRequest>("Failed to save image to database!", StatusCodeEnum.BadGateway_502, null);
                }

                return new BaseResponse<UploadImageRequest>("Upload image for HomeStay successfully", StatusCodeEnum.Created_201, request);
            }
            catch (Exception ex)
            {
                return new BaseResponse<UploadImageRequest>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
        }
    }
}