using AutoMapper;
using Azure;
using BusinessObject.Model;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Identity.Client;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Response.HomeStays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Service.Service
{
    public class HomeStayService : IHomeStayService
    {
        private readonly IMapper _mapper;
        private readonly IHomeStayRepository _homeStayRepository;
        private readonly IImageHomeStayRepository _imageHomeStayRepository; 
        private readonly Cloudinary _cloudinary;



        public HomeStayService(
            IMapper mapper,
            IHomeStayRepository homeStayRepository,
            IImageHomeStayRepository imageHomeStayRepository,
            Cloudinary cloudinary)
        {
            _mapper = mapper;
            _homeStayRepository = homeStayRepository;
            _imageHomeStayRepository = imageHomeStayRepository;
            _cloudinary = cloudinary;
        }

        public async Task<BaseResponse<HomeStayResponse>> ChangeHomeStayStatus(int homestayId, HomeStayStatus status)
        {
            var homestay = await _homeStayRepository.ChangeHomeStayStatus(homestayId, status);
            var homestayResponse = _mapper.Map<HomeStayResponse>(homestay);
            return new BaseResponse<HomeStayResponse>("Change status ok", StatusCodeEnum.OK_200, homestayResponse);
        }

        public async Task<BaseResponse<string>> DeleteHomeStay(int id)
        {
            var homestay = await _homeStayRepository.GetByIdAsync(id);
            await _homeStayRepository.DeleteAsync(homestay);
            return new BaseResponse<string>("Delete homestay success", StatusCodeEnum.OK_200, "Deleted successfully");
        }

        public async Task<BaseResponse<IEnumerable<HomeStayResponse>>> GetAllHomeStayRegisterFromBase()
        {
            IEnumerable<HomeStay> homeStay = await _homeStayRepository.GetAllRegisterHomeStayAsync();
            if (homeStay == null)
            {
                return new BaseResponse<IEnumerable<HomeStayResponse>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var homeStays = _mapper.Map<IEnumerable<HomeStayResponse>>(homeStay);
            if (homeStays == null)
            {
                return new BaseResponse<IEnumerable<HomeStayResponse>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<HomeStayResponse>>("Get all HomeStay as base success",
                StatusCodeEnum.OK_200, homeStays);
        }

        public async Task<BaseResponse<IEnumerable<GetAllHomeStayWithOwnerName>>> GetAllHomeStayWithOwnerName()
        {
            IEnumerable<HomeStay> homeStay = await _homeStayRepository.GetAllRegisterHomeStayAsync();
            if (homeStay == null)
            {
                return new BaseResponse<IEnumerable<GetAllHomeStayWithOwnerName>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var homeStays = _mapper.Map<IEnumerable<GetAllHomeStayWithOwnerName>>(homeStay);
            if (homeStays == null)
            {
                return new BaseResponse<IEnumerable<GetAllHomeStayWithOwnerName>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllHomeStayWithOwnerName>>("Get all HomeStay as base success",
                StatusCodeEnum.OK_200, homeStays);
        }

        public async Task<BaseResponse<HomeStayResponse>> GetHomeStayDetailByIdFromBase(int id)
        {
            HomeStay homeStay = await _homeStayRepository.GetHomeStayDetailByIdAsync(id);
            var result = _mapper.Map<HomeStayResponse>(homeStay);
            return new BaseResponse<HomeStayResponse>("Get HomeStay as base success", StatusCodeEnum.OK_200, result);
        }

        public async Task<BaseResponse<HomeStayResponse>> GetOwnerHomeStayByIdFromBase(string accountId)
        {
            HomeStay homeStay = await _homeStayRepository.GetOwnerHomeStayByIdAsync(accountId);
            var result = _mapper.Map<HomeStayResponse>(homeStay);
            return new BaseResponse<HomeStayResponse>("Get HomeStay as base success", StatusCodeEnum.OK_200, result);
        }
        public async Task<BaseResponse<IEnumerable<SimpleHomeStayResponse>>> GetSimpleHomeStaysByAccount(string accountId)
        {
            try
            {

                var allHomeStays = await _homeStayRepository.GetAllRegisterHomeStayAsync();
                var filteredHomeStays = allHomeStays.Where(h => h.AccountID == accountId).ToList();

                if (!filteredHomeStays.Any())
                {
                    return new BaseResponse<IEnumerable<SimpleHomeStayResponse>>("No HomeStay found for the account", StatusCodeEnum.NotFound_404, null);
                }

                // Ánh xạ sang SimpleHomeStayResponse
                var response = _mapper.Map<IEnumerable<SimpleHomeStayResponse>>(filteredHomeStays);
                return new BaseResponse<IEnumerable<SimpleHomeStayResponse>>("Get HomeStays by account success", StatusCodeEnum.OK_200, response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<SimpleHomeStayResponse>>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
        }

        private async Task<List<string>> UploadImagesToCloudinary(List<IFormFile> files)
        {
            if (files == null || !files.Any())
            {
                return new List<string>(); // Trả về danh sách rỗng nếu không có file
            }

            var urls = new List<string>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    continue; // Bỏ qua file không hợp lệ
                }

                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "HomeStayImages"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    urls.Add(uploadResult.SecureUrl.ToString());
                }
                else
                {
                    throw new Exception($"Failed to upload image to Cloudinary: {uploadResult.Error.Message}");
                }
            }

            return urls;
        }
        public async Task<BaseResponse<List<HomeStay>>> RegisterHomeStay(CreateHomeStayRequest request)
        {
            try
            {
                // Tạo HomeStay
                var homestayRegister = new List<HomeStay>
        {
            new HomeStay
            {
                Name = request.Name,
                Description = request.Description,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Status = HomeStayStatus.PendingApproval,
                TypeOfRental = request.RentalType,
                Address = request.Address,
                Area = request.Area,
                Longitude = request.Longtitude,
                Latitude = request.Latitude,
                AccountID = request.AccountID,
            }
        };

                // Lưu HomeStay vào database và gọi SaveChanges để lấy ID
                await _homeStayRepository.AddRange(homestayRegister);
                await _homeStayRepository.SaveChangesAsync(); // Thêm dòng này để gán HomeStayID

                // Upload ảnh nếu có
                if (request.Images != null && request.Images.Any())
                {
                    // Gọi phương thức upload hình
                    var imageUrls = await UploadImagesToCloudinary(request.Images);

                    // Lưu từng URL vào ImageHomeStay
                    foreach (var url in imageUrls)
                    {
                        var imageHomeStay = new ImageHomeStay
                        {
                            Image = url,
                            HomeStayID = homestayRegister[0].HomeStayID // Bây giờ HomeStayID đã được gán
                        };
                        await _imageHomeStayRepository.AddImageAsync(imageHomeStay);
                    }
                }

                return new BaseResponse<List<HomeStay>>("Register HomeStay and upload images successfully, Please Wait for Accepting", StatusCodeEnum.Created_201, homestayRegister);
            }
            catch (Exception ex)
            {
                // Ghi log chi tiết lỗi
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return new BaseResponse<List<HomeStay>>($"Something went wrong! Error: {ex.Message}. Inner Exception: {ex.InnerException?.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
        }
        public async Task<BaseResponse<HomeStay>> UpdateHomeStay(int homestayId, CreateHomeStayRequest request)
        {
            try
            {
                // 1. Tìm homestay
                var homeStayExist = await _homeStayRepository.GetHomeStayDetailByIdAsync(homestayId);
                if (homeStayExist == null)
                {
                    return new BaseResponse<HomeStay>("Cannot find HomeStay", StatusCodeEnum.BadGateway_502, null);
                }

                // 2. Cập nhật các trường cần thiết
                //    - Map từ request sang entity sẵn có (tránh ghi đè CreateAt, Status, v.v.)
                var updatedHomeStay = _mapper.Map(request, homeStayExist);
                updatedHomeStay.CreateAt = homeStayExist.CreateAt;  // Giữ nguyên thời gian tạo
                updatedHomeStay.Status = homeStayExist.Status;      // Giữ nguyên status cũ (nếu muốn)
                updatedHomeStay.UpdateAt = DateTime.Now;            // Thời gian cập nhật
                updatedHomeStay.Address = request.Address;          // Gán lại Address (nếu muốn tường minh)
                updatedHomeStay.TypeOfRental = request.RentalType;  // Gán lại TypeOfRental

                // 3. Lưu thay đổi phần text (nếu có) trước khi thêm ảnh
                await _homeStayRepository.UpdateAsync(updatedHomeStay);
                await _homeStayRepository.SaveChangesAsync();
                // Hoặc tuỳ ý, có thể SaveChanges một lần cuối ở dưới cùng.

                // 4. Nếu request có ảnh mới => Upload lên Cloudinary
                if (request.Images != null && request.Images.Any())
                {
                    var imageUrls = await UploadImagesToCloudinary(request.Images);

                    // 5. Tạo đối tượng ImageHomeStay mới và lưu vào DB
                    foreach (var url in imageUrls)
                    {
                        var imageHomeStay = new ImageHomeStay
                        {
                            Image = url,
                            HomeStayID = updatedHomeStay.HomeStayID
                        };
                        await _imageHomeStayRepository.AddImageAsync(imageHomeStay);
                    }
                }

                // 6. (Tuỳ chọn) SaveChanges cuối cùng
                await _homeStayRepository.SaveChangesAsync();

                // 7. Trả về
                return new BaseResponse<HomeStay>("Update HomeStay successfully (with images)",
                                                  StatusCodeEnum.OK_200, updatedHomeStay);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return new BaseResponse<HomeStay>($"Something went wrong! Error: {ex.Message}",
                                                  StatusCodeEnum.InternalServerError_500, null);
            }
        }



    }
}
