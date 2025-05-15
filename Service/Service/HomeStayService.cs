using AutoMapper;
using Azure;
using BusinessObject.Model;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Request.Pricing;
using Service.RequestAndResponse.Request.RoomType;
using Service.RequestAndResponse.Response.Accounts;
using Service.RequestAndResponse.Response.Bookings;
using Service.RequestAndResponse.Response.CancellationPolicyRequest;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.ImageHomeStay;
using Service.RequestAndResponse.Response.ImageRating;
using Service.RequestAndResponse.Response.Pricing;
using Service.RequestAndResponse.Response.Ratings;
using Service.RequestAndResponse.Response.RoomType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Service.Service
{
    public class HomeStayService : IHomeStayService
    {
        private readonly IMapper _mapper;
        private readonly IHomeStayRepository _homeStayRepository;
        private readonly IImageHomeStayRepository _imageHomeStayRepository;
        private readonly IHomeStayRentalRepository _homeStayRentalRepository;
        private readonly IPricingRepository _priceRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly Cloudinary _cloudinary;

        public HomeStayService(
            IMapper mapper,
            IHomeStayRepository homeStayRepository,
            IImageHomeStayRepository imageHomeStayRepository,
            IHomeStayRentalRepository homeRentalRepository,
            IPricingRepository priceRepository,
             IStaffRepository staffRepository,
              IAccountRepository accountRepository,
              IRoomTypeRepository roomTypeRepository,
            Cloudinary cloudinary)
        {
            _mapper = mapper;
            _homeStayRepository = homeStayRepository;
            _imageHomeStayRepository = imageHomeStayRepository;
            _cloudinary = cloudinary;
            _homeStayRentalRepository = homeRentalRepository;
            _priceRepository = priceRepository;
            _staffRepository = staffRepository;
            _accountRepository = accountRepository;
            _roomTypeRepository = roomTypeRepository;

        }

        public async Task<BaseResponse<HomeStayResponse>> ChangeHomeStayStatus(int homestayId, HomeStayStatus status, int? commissionRateId = null)
        {
            try
            {
                // Gọi repository để thay đổi Status và CommissionRateID
                var homestay = await _homeStayRepository.ChangeHomeStayStatus(homestayId, status, commissionRateId);

                if (homestay == null)
                {
                    return new BaseResponse<HomeStayResponse>(
                        "HomeStay not found!",
                        StatusCodeEnum.NotFound_404,
                        null);
                }

                var homestayResponse = _mapper.Map<HomeStayResponse>(homestay);
                return new BaseResponse<HomeStayResponse>(
                    "Change status and commission rate successfully!",
                    StatusCodeEnum.OK_200,
                    homestayResponse);
            }
            catch (Exception ex)
            {
                return new BaseResponse<HomeStayResponse>(
                    $"An error occurred while changing HomeStay status: {ex.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
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

        public async Task<BaseResponse<SimpleHomeStayResponse>> GetHomeStayDetailByIdFromBase(int id)
        {
            try
            {
                HomeStay homeStay = await _homeStayRepository.GetHomeStayDetailByIdAsync(id);
                if (homeStay == null)
                {
                    return new BaseResponse<SimpleHomeStayResponse>("HomeStay not found!", StatusCodeEnum.NotFound_404, null);
                }

                // Sử dụng AutoMapper để ánh xạ các trường cơ bản
                var result = _mapper.Map<SimpleHomeStayResponse>(homeStay);

                // Xử lý thủ công cho SumRate
                if (homeStay.Ratings == null || !homeStay.Ratings.Any())
                {
                    Console.WriteLine($"No Ratings found for HomeStayID: {id}");
                    result.SumRate = null;
                }
                else
                {
                    var validRatings = homeStay.Ratings
                        .Where(r => r.HomeStayID.HasValue && r.HomeStayID == id)
                        .ToList();
                    if (!validRatings.Any())
                    {
                        Console.WriteLine($"No valid Ratings with HomeStayID matching {id}. Ratings count: {homeStay.Ratings.Count}");
                        foreach (var rating in homeStay.Ratings)
                        {
                            Console.WriteLine($"RatingID: {rating.RatingID}, HomeStayID: {rating.HomeStayID}");
                        }
                        result.SumRate = null;
                    }
                    else
                    {
                        result.SumRate = validRatings.Average(r => r.SumRate);
                    }
                }

                // Xử lý thủ công cho TotalRatings
                if (homeStay.Ratings == null)
                {
                    Console.WriteLine($"Ratings is null for HomeStayID: {id}");
                    result.TotalRatings = 0;
                }
                else
                {
                    result.TotalRatings = homeStay.Ratings
                        .Count(r => r.HomeStayID.HasValue && r.HomeStayID == id);
                }

                // Xử lý thủ công cho LatestRatings
                if (homeStay.Ratings == null || !homeStay.Ratings.Any())
                {
                    Console.WriteLine($"No Ratings found for LatestRatings, HomeStayID: {id}");
                    result.LatestRatings = new List<CreateRatingResponse>();
                }
                else
                {
                    result.LatestRatings = homeStay.Ratings
                        .Where(r => r.HomeStayID.HasValue && r.HomeStayID == id)
                        .OrderByDescending(r => r.CreatedAt)
                        .Take(5)
                        .Select(r => new CreateRatingResponse
                        {
                            RatingID = r.RatingID,
                            SumRate = r.SumRate,
                            CleaningRate = r.CleaningRate,
                            ServiceRate = r.ServiceRate,
                            FacilityRate = r.FacilityRate,
                            Content = r.Content,
                            AccountID = r.AccountID,
                            Username = r.Account?.UserName,
                            HomeStayID = r.HomeStayID ?? 0,
                            BookingID = r.BookingID ?? 0,
                            CreatedAt = r.CreatedAt,
                            UpdatedAt = r.UpdatedAt,
                            ImageRatings = r.ImageRatings?.Select(ir => new ImageRatingResponse
                            {
                                ImageRatingID = ir.ImageRatingID,
                                Image = ir.Image,
                                RatingID = ir.RatingID ?? 0
                            }).ToList() ?? new List<ImageRatingResponse>()
                        }).ToList();
                }

                // Xử lý thủ công cho CheapestPrice
                if (homeStay.HomeStayRentals == null || !homeStay.HomeStayRentals.Any())
                {
                    Console.WriteLine($"No HomeStayRentals found for HomeStayID: {id}");
                    result.CheapestPrice = null;
                }
                else
                {
                    var prices = homeStay.HomeStayRentals
                        .SelectMany(r => r.RoomTypes ?? new List<RoomTypes>())
                        .SelectMany(rt => rt.Prices ?? new List<Pricing>())
                        .Where(p => p.IsActive && p.RoomTypesID.HasValue)
                        .OrderBy(p => p.RentPrice)
                        .Select(p => (decimal?)p.RentPrice)
                        .FirstOrDefault();
                    if (prices == null)
                    {
                        Console.WriteLine($"No valid Prices found for HomeStayID: {id}");
                        var allPrices = homeStay.HomeStayRentals
                            .SelectMany(r => r.RoomTypes ?? new List<RoomTypes>())
                            .SelectMany(rt => rt.Prices ?? new List<Pricing>())
                            .ToList();
                        Console.WriteLine($"Total Prices count: {allPrices.Count}");
                        foreach (var price in allPrices)
                        {
                            Console.WriteLine($"PriceID: {price.PricingID}, RoomTypesID: {price.RoomTypesID}, IsActive: {price.IsActive}");
                        }
                        result.CheapestPrice = null;
                    }
                    else
                    {
                        result.CheapestPrice = prices;
                    }
                }

                // Xử lý thủ công cho CancellationPolicy
                if (homeStay.CancelPolicy == null)
                {
                    Console.WriteLine($"No CancellationPolicy found for HomeStayID: {id}");
                    result.CancelPolicy = null;
                }
                else
                {
                    result.CancelPolicy = new GetAllCancellationPolicy
                    {
                        CancellationID = homeStay.CancelPolicy.CancellationID,
                        DayBeforeCancel = homeStay.CancelPolicy.DayBeforeCancel,
                        RefundPercentage = homeStay.CancelPolicy.RefundPercentage,
                        CreateAt = homeStay.CancelPolicy.CreateAt,
                        UpdateAt = homeStay.CancelPolicy.UpdateAt,
                        HomeStayID = homeStay.CancelPolicy.HomeStayID 
                    };
                }

                // Xử lý thủ công cho StaffID và StaffName
                var staff = await _staffRepository.GetAllStaffByHomeStay(id);
                var firstStaff = staff?.FirstOrDefault();
                if (firstStaff == null)
                {
                    Console.WriteLine($"No Staff found for HomeStayID: {id}");
                    result.StaffID = null;
                    result.StaffName = null;
                }
                else
                {
                    result.StaffID = firstStaff.StaffIdAccount;
                    result.StaffName = firstStaff.StaffName;
                }

                // Xử lý thủ công cho OwnerID và OwnerName
                result.OwnerID = homeStay.AccountID;
                var ownerAccount = await _accountRepository.GetByAccountIdAsync(homeStay.AccountID);
             

                // Log kết quả
                Console.WriteLine($"Mapped SumRate for HomeStayID {id}: {result.SumRate}");
                Console.WriteLine($"Mapped TotalRatings for HomeStayID {id}: {result.TotalRatings}");
                Console.WriteLine($"Mapped LatestRatings count for HomeStayID {id}: {(result.LatestRatings != null ? result.LatestRatings.Count() : 0)}");
                Console.WriteLine($"Mapped CheapestPrice for HomeStayID {id}: {result.CheapestPrice}");
                Console.WriteLine($"Mapped CancellationPolicy for HomeStayID {id}: {(result.CancelPolicy != null ? result.CancelPolicy.CancellationID : "null")}");
                Console.WriteLine($"Mapped StaffID for HomeStayID {id}: {result.StaffID}");
                Console.WriteLine($"Mapped StaffName for HomeStayID {id}: {result.StaffName}");
                Console.WriteLine($"Mapped OwnerID for HomeStayID {id}: {result.OwnerID}");
           

                return new BaseResponse<SimpleHomeStayResponse>("Get HomeStay as base success", StatusCodeEnum.OK_200, result);
            }
            catch (Exception ex)
            {
                return new BaseResponse<SimpleHomeStayResponse>($"An error occurred: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
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

                    var emptyList = new List<SimpleHomeStayResponse>();
                    return new BaseResponse<IEnumerable<SimpleHomeStayResponse>>(
                        "Account has not registered any homestay",
                        StatusCodeEnum.OK_200,
                        emptyList
                    );
                }


                var response = _mapper.Map<IEnumerable<SimpleHomeStayResponse>>(filteredHomeStays);
                return new BaseResponse<IEnumerable<SimpleHomeStayResponse>>("Get HomeStays by account success", StatusCodeEnum.OK_200, response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<SimpleHomeStayResponse>>($"Something went wrong! Error: {ex.Message}", StatusCodeEnum.InternalServerError_500, null);
            }
        }

        public async Task<BaseResponse<ImageHomeStayResponse>> UpdateHomeStayImages(int homeStayId, UpdateHomeStayImagesBodyRequest request)
        {
            try
            {
                // Kiểm tra HomeStay tồn tại
                var homeStay = await _homeStayRepository.GetByIdAsync(homeStayId);
                if (homeStay == null)
                {
                    return new BaseResponse<ImageHomeStayResponse>(
                        "HomeStay not found!",
                        StatusCodeEnum.NotFound_404,
                        null);
                }

                // Lấy danh sách hình ảnh hiện tại
                var existingImages = await _imageHomeStayRepository.GetImagesByHomeStayIdAsync(homeStayId);

                // Nếu có ImageHomeStayID, thay thế hình ảnh cụ thể
                if (request.ImageHomeStayID.HasValue)
                {
                    // Kiểm tra ImageHomeStayID hợp lệ
                    var imageToReplace = existingImages?.FirstOrDefault(img => img.ImageHomeStayID == request.ImageHomeStayID.Value);
                    if (imageToReplace == null)
                    {
                        return new BaseResponse<ImageHomeStayResponse>(
                            "ImageHomeStay not found!",
                            StatusCodeEnum.NotFound_404,
                            null);
                    }

                    // Kiểm tra có hình ảnh mới để thay thế không
                    if (request.Images == null || !request.Images.Any())
                    {
                        return new BaseResponse<ImageHomeStayResponse>(
                            "No new image provided to replace the existing one.",
                            StatusCodeEnum.BadRequest_400,
                            null);
                    }

                    // Chỉ cho phép thay thế bằng 1 hình ảnh
                    if (request.Images.Count > 1)
                    {
                        return new BaseResponse<ImageHomeStayResponse>(
                            "Only one image can be provided to replace an existing image.",
                            StatusCodeEnum.BadRequest_400,
                            null);
                    }

                    // Upload hình ảnh mới lên Cloudinary
                    var imageUrls = await UploadImagesToCloudinary(request.Images);
                    if (imageUrls == null || !imageUrls.Any())
                    {
                        return new BaseResponse<ImageHomeStayResponse>(
                            "Failed to upload new image to Cloudinary.",
                            StatusCodeEnum.InternalServerError_500,
                            null);
                    }

                    // Xóa hình ảnh cũ trên Cloudinary
                    var publicId = ExtractPublicIdFromUrl(imageToReplace.Image);
                    if (!string.IsNullOrEmpty(publicId))
                    {
                        var deletionParams = new DeletionParams(publicId);
                        var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
                        if (deletionResult.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            return new BaseResponse<ImageHomeStayResponse>(
                                $"Failed to delete old image from Cloudinary: {deletionResult.Error.Message}",
                                StatusCodeEnum.InternalServerError_500,
                                null);
                        }
                    }

                    // Cập nhật URL mới cho hình ảnh
                    imageToReplace.Image = imageUrls.First();
                    var updatedImage = await _imageHomeStayRepository.UpdateImageAsync(imageToReplace);

                    // Cập nhật HomeStay
                    homeStay.UpdateAt = DateTime.UtcNow;
                    await _homeStayRepository.UpdateAsync(homeStay);
                    await _homeStayRepository.SaveChangesAsync();

                    // Ánh xạ sang ImageHomeStayResponse để trả về
                    var response = _mapper.Map<ImageHomeStayResponse>(updatedImage);

                    return new BaseResponse<ImageHomeStayResponse>(
                        "HomeStay image replaced successfully!",
                        StatusCodeEnum.OK_200,
                        response);
                }


                var newImages = new List<ImageHomeStay>();
                if (request.Images != null && request.Images.Any())
                {
                    var imageUrls = await UploadImagesToCloudinary(request.Images);
                    foreach (var url in imageUrls)
                    {
                        var imageHomeStay = new ImageHomeStay
                        {
                            Image = url,
                            HomeStayID = homeStayId
                        };
                        newImages.Add(imageHomeStay);
                        await _imageHomeStayRepository.AddImageAsync(imageHomeStay);
                    }
                    await _imageHomeStayRepository.SaveChangesAsync();
                }

                // Cập nhật HomeStay
                homeStay.UpdateAt = DateTime.UtcNow;
                await _homeStayRepository.UpdateAsync(homeStay);
                await _homeStayRepository.SaveChangesAsync();

                // Trả về hình ảnh mới nhất (nếu có) hoặc null
                var latestImage = newImages.LastOrDefault();
                var responseData = latestImage != null ? _mapper.Map<ImageHomeStayResponse>(latestImage) : null;

                return new BaseResponse<ImageHomeStayResponse>(
                    "HomeStay images updated successfully!",
                    StatusCodeEnum.OK_200,
                    responseData);
            }
            catch (Exception ex)
            {
                return new BaseResponse<ImageHomeStayResponse>(
                    $"An error occurred while updating HomeStay images: {ex.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
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
        private string ExtractPublicIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            // URL Cloudinary có dạng: https://res.cloudinary.com/{cloud_name}/image/upload/v{version}/{folder}/{public_id}.{format}
            var parts = url.Split('/');
            var fileNameWithExtension = parts.Last();
            var publicId = fileNameWithExtension.Split('.').First(); // Lấy phần trước đuôi file
            var folder = "HomeStayImages/"; // Phải khớp với folder khi upload
            return $"{folder}{publicId}";
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
        //public async Task<BaseResponse<HomeStay>> UpdateHomeStay(int homestayId, UpdateHomeStayRequest request)
        //{
        //    try
        //    {

        //        var homeStayExist = await _homeStayRepository.GetHomeStayDetailByIdAsync(homestayId);
        //        if (homeStayExist == null)
        //        {
        //            return new BaseResponse<HomeStay>("Cannot find HomeStay", StatusCodeEnum.BadGateway_502, null);
        //        }


        //        var updatedHomeStay = _mapper.Map(request, homeStayExist);
        //        updatedHomeStay.CreateAt = homeStayExist.CreateAt; 
        //        updatedHomeStay.Status = homeStayExist.Status;      
        //        updatedHomeStay.UpdateAt = DateTime.Now;          
        //        updatedHomeStay.Address = request.Address;          
        //        updatedHomeStay.TypeOfRental = request.RentalType;
        //        updatedHomeStay.Longitude = request.Longitude;
        //        updatedHomeStay.Latitude = request.Latitude;


        //        await _homeStayRepository.UpdateAsync(updatedHomeStay);
        //        await _homeStayRepository.SaveChangesAsync();

        //        return new BaseResponse<HomeStay>("Update HomeStay successfully",
        //                                          StatusCodeEnum.OK_200, updatedHomeStay);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: {ex.Message}");
        //        Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
        //        return new BaseResponse<HomeStay>($"Something went wrong! Error: {ex.Message}",
        //                                          StatusCodeEnum.InternalServerError_500, null);
        //    }
        //}

        public async Task<BaseResponse<HomeStay>> UpdateHomeStay(int homestayId, UpdateHomeStayRequest request)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (request == null)
                {
                    return new BaseResponse<HomeStay>("Request cannot be null", StatusCodeEnum.BadRequest_400, null);
                }

                // Kiểm tra HomeStayID hợp lệ
                if (homestayId <= 0)
                {
                    return new BaseResponse<HomeStay>("HomeStayID must be greater than 0", StatusCodeEnum.BadRequest_400, null);
                }

                // Kiểm tra homestay tồn tại
                var homeStayExist = await _homeStayRepository.GetHomeStayDetailByIdAsync(homestayId);
                if (homeStayExist == null)
                {
                    return new BaseResponse<HomeStay>("Cannot find HomeStay with the provided HomeStayID", StatusCodeEnum.NotFound_404, null);
                }

                // Ánh xạ dữ liệu từ request sang homestay hiện tại
                var updatedHomeStay = _mapper.Map(request, homeStayExist);
                updatedHomeStay.CreateAt = homeStayExist.CreateAt; // Giữ nguyên CreateAt
                updatedHomeStay.Status = homeStayExist.Status;     // Giữ nguyên Status
                updatedHomeStay.UpdateAt = DateTime.UtcNow;        // Cập nhật thời gian

                // Cập nhật homestay
                await _homeStayRepository.UpdateAsync(updatedHomeStay);
                await _homeStayRepository.SaveChangesAsync();

                return new BaseResponse<HomeStay>("Update HomeStay successfully",
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
        //public async Task<BaseResponse<IEnumerable<HomeStayResponse>>> FilterHomeStaysAsync(FilterHomeStayRequest request)
        //{
        //    try
        //    {
        //        // Kiểm tra ngày hợp lệ
        //        if (request.CheckInDate > request.CheckOutDate)
        //        {
        //            return new BaseResponse<IEnumerable<HomeStayResponse>>(
        //                "Check-out date must be after check-in date.",
        //                StatusCodeEnum.BadRequest_400,
        //                null);
        //        }

        //        if (request.CheckInDate < DateTime.UtcNow.Date)
        //        {
        //            return new BaseResponse<IEnumerable<HomeStayResponse>>(
        //                "Check-in date cannot be in the past.",
        //                StatusCodeEnum.BadRequest_400,
        //                null);
        //        }

        //        var checkInDate = request.CheckInDate.Date;
        //        var checkOutDate = request.CheckOutDate.Date;

        //        // Bước 1: Lấy tất cả HomeStay và lọc theo Status trước
        //        var allHomeStays = await _homeStayRepository.GetAllWithDetailsAsync();
        //        var acceptedHomeStays = allHomeStays
        //            .Where(h => (int)h.Status == (int)HomeStayStatus.Accepted)
        //            .ToList();

        //        if (!acceptedHomeStays.Any())
        //        {
        //            return new BaseResponse<IEnumerable<HomeStayResponse>>(
        //                "No HomeStays available with Accepted status.",
        //                StatusCodeEnum.OK_200,
        //                new List<HomeStayResponse>());
        //        }

        //        // Log dữ liệu Rentals, RoomTypes, và Prices
        //        Console.WriteLine("Data check for HomeStays:");
        //        foreach (var homeStay in acceptedHomeStays)
        //        {
        //            Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}");
        //            var rentals = homeStay.HomeStayRentals ?? new List<HomeStayRentals>();
        //            Console.WriteLine($"  Number of Rentals: {rentals.Count}");
        //            foreach (var rental in rentals)
        //            {
        //                var roomTypes = rental.RoomTypes ?? new List<RoomTypes>();
        //                Console.WriteLine($"    RentalID: {rental.HomeStayRentalID}, Number of RoomTypes: {roomTypes.Count}");
        //                foreach (var roomType in roomTypes)
        //                {
        //                    var prices = roomType.Prices ?? new List<Pricing>();
        //                    Console.WriteLine($"      RoomTypeID: {roomType.RoomTypesID}, Number of Prices: {prices.Count}");
        //                    foreach (var price in prices)
        //                    {
        //                        Console.WriteLine($"        PriceID: {price.PricingID}, RentPrice: {price.RentPrice}, IsDefault: {price.IsDefault}, IsActive: {price.IsActive}");
        //                    }
        //                }
        //            }
        //        }

        //        // Bước 2: Lọc theo Location (khoảng cách)
        //        var homeStaysWithDistance = new List<(HomeStay HomeStay, double Distance)>();
        //        foreach (var homeStay in acceptedHomeStays)
        //        {
        //            var distance = await _homeStayRepository.CalculateHaversineDistance(
        //                request.Latitude, request.Longitude, homeStay.Latitude, homeStay.Longitude);
        //            if (distance > request.MaxDistance)
        //            {
        //                Console.WriteLine($"HomeStayID: {homeStay.HomeStayID} is excluded due to distance: {distance} km (max: {request.MaxDistance} km).");
        //                continue;
        //            }
        //            homeStaysWithDistance.Add((homeStay, distance));
        //        }

        //        if (!homeStaysWithDistance.Any())
        //        {
        //            return new BaseResponse<IEnumerable<HomeStayResponse>>(
        //                "No HomeStays available within the specified distance.",
        //                StatusCodeEnum.OK_200,
        //                new List<HomeStayResponse>());
        //        }

        //        // Bước 3: Lọc theo sức chứa và kiểm tra phòng trống
        //        var filteredHomeStays = new List<(HomeStay HomeStay, double Distance)>();
        //        foreach (var (homeStay, distance) in homeStaysWithDistance)
        //        {
        //            bool hasAvailableRental = false;
        //            var rentals = homeStay.HomeStayRentals ?? new List<HomeStayRentals>();

        //            if (!rentals.Any())
        //            {
        //                Console.WriteLine($"HomeStayID: {homeStay.HomeStayID} is excluded due to no rentals.");
        //                continue;
        //            }

        //            var activeBookings = homeStay.Bookings?
        //                .Where(b => b.Status == BookingStatus.Pending ||
        //                            b.Status == BookingStatus.Confirmed ||
        //                            b.Status == BookingStatus.InProgress)
        //                .ToList() ?? new List<Booking>();

        //            Console.WriteLine($"HomeStayID: {homeStay.HomeStayID} has {rentals.Count} rentals and {activeBookings.Count} active bookings.");

        //            foreach (var rental in rentals)
        //            {
        //                // Kiểm tra sức chứa của rental
        //                if (rental.MaxAdults < request.NumberOfAdults ||
        //                    rental.MaxChildren < request.NumberOfChildren ||
        //                    rental.MaxPeople < (request.NumberOfAdults + request.NumberOfChildren))
        //                {
        //                    Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is excluded due to insufficient capacity.");
        //                    continue;
        //                }

        //                // Kiểm tra tính khả dụng của rental dựa trên RentWhole
        //                bool isRentalAvailable = false;
        //                if (rental.RentWhole)
        //                {
        //                    var hasBookingForRental = activeBookings
        //                        .SelectMany(b => b.BookingDetails ?? new List<BookingDetail>())
        //                        .Any(bd => bd.HomeStayRentalID == rental.HomeStayRentalID &&
        //                                   bd.CheckInDate.Date <= checkOutDate &&
        //                                   bd.CheckOutDate.Date >= checkInDate);

        //                    if (hasBookingForRental)
        //                    {
        //                        Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is not available (already booked for the requested period).");
        //                    }
        //                    else
        //                    {
        //                        isRentalAvailable = true;
        //                        Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is available (RentWhole = true, no bookings).");
        //                    }
        //                }
        //                else
        //                {
        //                    var roomTypes = rental.RoomTypes ?? new List<RoomTypes>();
        //                    var allRooms = roomTypes.SelectMany(rt => rt.Rooms ?? new List<Room>()).ToList();

        //                    if (!allRooms.Any())
        //                    {
        //                        Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is excluded due to no rooms.");
        //                        continue;
        //                    }

        //                    var bookedRoomIds = new HashSet<int>();
        //                    foreach (var booking in activeBookings)
        //                    {
        //                        var bookingDetails = booking.BookingDetails ?? new List<BookingDetail>();
        //                        foreach (var detail in bookingDetails)
        //                        {
        //                            if (detail.HomeStayRentalID != rental.HomeStayRentalID)
        //                                continue;

        //                            var detailCheckInDate = detail.CheckInDate.Date;
        //                            var detailCheckOutDate = detail.CheckOutDate.Date;

        //                            if (detailCheckInDate <= checkOutDate && detailCheckOutDate >= checkInDate)
        //                            {
        //                                if (detail.RoomID.HasValue)
        //                                {
        //                                    bookedRoomIds.Add(detail.RoomID.Value);
        //                                }
        //                            }
        //                        }
        //                    }

        //                    isRentalAvailable = allRooms.Any(room => !bookedRoomIds.Contains(room.RoomID) && room.isActive);

        //                    if (isRentalAvailable)
        //                    {
        //                        Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is available (has available rooms).");
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is not available (all rooms are booked).");
        //                    }
        //                }

        //                if (isRentalAvailable)
        //                {
        //                    hasAvailableRental = true;
        //                    break;
        //                }
        //            }

        //            if (hasAvailableRental)
        //            {
        //                filteredHomeStays.Add((homeStay, distance));
        //                Console.WriteLine($"HomeStayID: {homeStay.HomeStayID} is available and added to filtered list (distance: {distance} km).");
        //            }
        //            else
        //            {
        //                Console.WriteLine($"HomeStayID: {homeStay.HomeStayID} is excluded due to no available rentals.");
        //            }
        //        }

        //        // Sắp xếp theo khoảng cách (tăng dần)
        //        var finalHomeStays = filteredHomeStays
        //            .OrderBy(hs => hs.Distance)
        //            .Select(hs => hs.HomeStay)
        //            .ToList();

        //        var response = _mapper.Map<IEnumerable<HomeStayResponse>>(finalHomeStays);

        //        // Bước 4: Lọc theo rating (cho phép khoảng chênh lệch ±0.5)
        //        response = response.Where(r =>
        //            (!request.Rating.HasValue || (r.SumRate.HasValue && Math.Abs(r.SumRate.Value - request.Rating.Value) <= 0.5)) &&
        //            (r.SumRate.HasValue && r.SumRate >= 1 && r.SumRate <= 5)
        //        ).ToList();

        //        // Bước 5: Lọc theo price
        //        response = response.Where(r =>
        //            (!request.MinPrice.HasValue || r.DefaultRentPrice >= request.MinPrice) &&
        //            (!request.MaxPrice.HasValue || r.DefaultRentPrice <= request.MaxPrice) &&
        //            r.DefaultRentPrice.HasValue
        //        ).ToList();

        //        // Log kết quả sau ánh xạ và lọc
        //        Console.WriteLine("Final HomeStays after mapping and filtering:");
        //        foreach (var res in response)
        //        {
        //            Console.WriteLine($"HomeStayID: {res.HomeStayID}, DefaultRentPrice: {res.DefaultRentPrice}, SumRate: {res.SumRate}");
        //        }

        //        return new BaseResponse<IEnumerable<HomeStayResponse>>(
        //            response.Any() ? "HomeStays filtered successfully!" : "No HomeStays available for the given criteria.",
        //            StatusCodeEnum.OK_200,
        //            response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new BaseResponse<IEnumerable<HomeStayResponse>>(
        //            $"An error occurred while filtering HomeStays: {ex.Message}",
        //            StatusCodeEnum.InternalServerError_500,
        //            null);
        //    }
        //}

        public async Task<BaseResponse<IEnumerable<HomeStayResponse>>> FilterHomeStaysAsync(FilterHomeStayRequest request)
        {
            try
            {
                // Kiểm tra ngày hợp lệ
                if (request.CheckInDate > request.CheckOutDate)
                {
                    return new BaseResponse<IEnumerable<HomeStayResponse>>(
                        "Check-out date must be after check-in date.",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                if (request.CheckInDate < DateTime.UtcNow.Date)
                {
                    return new BaseResponse<IEnumerable<HomeStayResponse>>(
                        "Check-in date cannot be in the past.",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                var checkInDate = request.CheckInDate.Date;
                var checkOutDate = request.CheckOutDate.Date;

                // Bước 1: Lấy tất cả HomeStay và lọc theo Status trước
                var allHomeStays = await _homeStayRepository.GetAllWithDetailsAsync();
                var acceptedHomeStays = allHomeStays
                    .Where(h => (int)h.Status == (int)HomeStayStatus.Accepted)
                    .ToList();

                if (!acceptedHomeStays.Any())
                {
                    return new BaseResponse<IEnumerable<HomeStayResponse>>(
                        "No HomeStays available with Accepted status.",
                        StatusCodeEnum.OK_200,
                        new List<HomeStayResponse>());
                }

                // Log dữ liệu Rentals, RoomTypes, và Prices
                Console.WriteLine("Data check for HomeStays:");
                foreach (var homeStay in acceptedHomeStays)
                {
                    Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}");
                    var rentals = homeStay.HomeStayRentals ?? new List<HomeStayRentals>();
                    Console.WriteLine($"  Number of Rentals: {rentals.Count}");
                    foreach (var rental in rentals)
                    {
                        var roomTypes = rental.RoomTypes ?? new List<RoomTypes>();
                        Console.WriteLine($"    RentalID: {rental.HomeStayRentalID}, Number of RoomTypes: {roomTypes.Count}, RentWhole: {rental.RentWhole}");
                        foreach (var roomType in roomTypes)
                        {
                            var rooms = roomType.Rooms ?? new List<Room>();
                            var prices = roomType.Prices ?? new List<Pricing>();
                            Console.WriteLine($"      RoomTypeID: {roomType.RoomTypesID}, Number of Rooms: {rooms.Count}, Number of Prices: {prices.Count}");
                            foreach (var room in rooms)
                            {
                                Console.WriteLine($"        RoomID: {room.RoomID}, isActive: {room.isActive}");
                            }
                            foreach (var price in prices)
                            {
                                Console.WriteLine($"        PriceID: {price.PricingID}, RentPrice: {price.RentPrice}, IsDefault: {price.IsDefault}, IsActive: {price.IsActive}");
                            }
                        }
                    }
                }

                // Bước 2: Lọc theo Location (khoảng cách)
                var homeStaysWithDistance = new List<(HomeStay HomeStay, double Distance)>();
                foreach (var homeStay in acceptedHomeStays)
                {
                    var distance = await _homeStayRepository.CalculateHaversineDistance(
                        request.Latitude, request.Longitude, homeStay.Latitude, homeStay.Longitude);
                    if (distance > request.MaxDistance)
                    {
                        Console.WriteLine($"HomeStayID: {homeStay.HomeStayID} is excluded due to distance: {distance} km (max: {request.MaxDistance} km).");
                        continue;
                    }
                    homeStaysWithDistance.Add((homeStay, distance));
                }

                if (!homeStaysWithDistance.Any())
                {
                    return new BaseResponse<IEnumerable<HomeStayResponse>>(
                        "No HomeStays available within the specified distance.",
                        StatusCodeEnum.OK_200,
                        new List<HomeStayResponse>());
                }

                // Bước 3: Lọc theo sức chứa và kiểm tra phòng trống
                var filteredHomeStays = new List<(HomeStay HomeStay, double Distance)>();
                foreach (var (homeStay, distance) in homeStaysWithDistance)
                {
                    bool hasAvailableRental = false;
                    var rentals = homeStay.HomeStayRentals ?? new List<HomeStayRentals>();

                    if (!rentals.Any())
                    {
                        Console.WriteLine($"HomeStayID: {homeStay.HomeStayID} is excluded due to no rentals.");
                        continue;
                    }

                    var activeBookings = homeStay.Bookings?
                        .Where(b => b.Status == BookingStatus.Pending ||
                                    b.Status == BookingStatus.Confirmed ||
                                    b.Status == BookingStatus.InProgress)
                        .ToList() ?? new List<Booking>();

                    Console.WriteLine($"HomeStayID: {homeStay.HomeStayID} has {rentals.Count} rentals and {activeBookings.Count} active bookings.");

                    foreach (var rental in rentals)
                    {
                        // Kiểm tra sức chứa của rental
                        if (rental.MaxAdults < request.NumberOfAdults ||
                            rental.MaxChildren < request.NumberOfChildren ||
                            rental.MaxPeople < (request.NumberOfAdults + request.NumberOfChildren))
                        {
                            Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is excluded due to insufficient capacity (MaxAdults: {rental.MaxAdults}, MaxChildren: {rental.MaxChildren}, MaxPeople: {rental.MaxPeople}).");
                            continue;
                        }

                        // Kiểm tra tính khả dụng của rental dựa trên RentWhole
                        bool isRentalAvailable = false;
                        if (rental.RentWhole)
                        {
                            var hasBookingForRental = activeBookings
                                .SelectMany(b => b.BookingDetails ?? new List<BookingDetail>())
                                .Any(bd => bd.HomeStayRentalID == rental.HomeStayRentalID &&
                                           bd.CheckInDate.Date <= checkOutDate &&
                                           bd.CheckOutDate.Date >= checkInDate);

                            if (hasBookingForRental)
                            {
                                Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is not available (already booked for the requested period).");
                            }
                            else
                            {
                                isRentalAvailable = true;
                                Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is available (RentWhole = true, no bookings).");
                            }
                        }
                        else
                        {
                            var roomTypes = rental.RoomTypes ?? new List<RoomTypes>();
                            var allRooms = roomTypes.SelectMany(rt => rt.Rooms ?? new List<Room>()).ToList();

                            if (!allRooms.Any())
                            {
                                Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is excluded due to no rooms.");
                                continue;
                            }

                            var bookedRoomIds = new HashSet<int>();
                            foreach (var booking in activeBookings)
                            {
                                var bookingDetails = booking.BookingDetails ?? new List<BookingDetail>();
                                foreach (var detail in bookingDetails)
                                {
                                    if (detail.HomeStayRentalID != rental.HomeStayRentalID)
                                        continue;

                                    var detailCheckInDate = detail.CheckInDate.Date;
                                    var detailCheckOutDate = detail.CheckOutDate.Date;

                                    if (detailCheckInDate <= checkOutDate && detailCheckOutDate >= checkInDate)
                                    {
                                        if (detail.RoomID.HasValue)
                                        {
                                            bookedRoomIds.Add(detail.RoomID.Value);
                                            Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID}, RoomID: {detail.RoomID.Value} is booked (CheckIn: {detailCheckInDate}, CheckOut: {detailCheckOutDate}).");
                                        }
                                    }
                                }
                            }

                            var availableRooms = allRooms.Where(room => !bookedRoomIds.Contains(room.RoomID) && room.isActive).ToList();
                            isRentalAvailable = availableRooms.Any();

                            if (isRentalAvailable)
                            {
                                Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is available (has {availableRooms.Count} available rooms).");
                            }
                            else
                            {
                                Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is not available (all rooms are booked or inactive).");
                            }
                        }

                        if (isRentalAvailable)
                        {
                            hasAvailableRental = true;
                            break;
                        }
                    }

                    if (hasAvailableRental)
                    {
                        filteredHomeStays.Add((homeStay, distance));
                        Console.WriteLine($"HomeStayID: {homeStay.HomeStayID} is available and added to filtered list (distance: {distance} km).");
                    }
                    else
                    {
                        Console.WriteLine($"HomeStayID: {homeStay.HomeStayID} is excluded due to no available rentals.");
                    }
                }

                // Sắp xếp theo khoảng cách (tăng dần)
                var finalHomeStays = filteredHomeStays
                    .OrderBy(hs => hs.Distance)
                    .Select(hs => hs.HomeStay)
                    .ToList();

                var response = _mapper.Map<IEnumerable<HomeStayResponse>>(finalHomeStays);

                // Bước 4: Lọc theo Rating (cho phép khoảng chênh lệch ±0.5)
                response = response.Where(r =>
                    (!request.Rating.HasValue || (r.SumRate.HasValue && Math.Abs(r.SumRate.Value - request.Rating.Value) <= 0.5))
                ).ToList();

                // Bước 5: Lọc theo Price
                response = response.Where(r =>
                    (!request.MinPrice.HasValue || (r.DefaultRentPrice.HasValue && r.DefaultRentPrice >= request.MinPrice)) &&
                    (!request.MaxPrice.HasValue || (r.DefaultRentPrice.HasValue && r.DefaultRentPrice <= request.MaxPrice))
                ).ToList();

                // Log kết quả sau ánh xạ và lọc
                Console.WriteLine("Final HomeStays after mapping and filtering:");
                foreach (var res in response)
                {
                    Console.WriteLine($"HomeStayID: {res.HomeStayID}, DefaultRentPrice: {res.DefaultRentPrice}, SumRate: {res.SumRate}");
                }

                return new BaseResponse<IEnumerable<HomeStayResponse>>(
                    response.Any() ? "HomeStays filtered successfully!" : "No HomeStays available for the given criteria.",
                    StatusCodeEnum.OK_200,
                    response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<HomeStayResponse>>(
                    $"An error occurred while filtering HomeStays: {ex.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }

        /*public async Task<BaseResponse<IEnumerable<SimpleHomeStayResponse>>> GetNearestHomeStays(double userLat, double userLon, int topN = 5)
        {
            IEnumerable<HomeStay> homeStay = await _homeStayRepository.GetNearestHomeStaysAsync(userLat,userLon,topN);
            if (homeStay == null)
            {
                return new BaseResponse<IEnumerable<SimpleHomeStayResponse>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var homeStays = _mapper.Map<IEnumerable<SimpleHomeStayResponse>>(homeStay);
            if (homeStays == null)
            {
                return new BaseResponse<IEnumerable<SimpleHomeStayResponse>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<SimpleHomeStayResponse>>("Get all HomeStay as base success",
                StatusCodeEnum.OK_200, homeStays);
        }*/

        public async Task<BaseResponse<IEnumerable<SimpleHomeStayResponse>>> GetNearestHomeStays(double userLat, double userLon, int pageIndex = 1, int pageSize = 5)
        {
            IEnumerable<HomeStay> homeStay = await _homeStayRepository.GetNearestHomeStaysAsync(userLat, userLon, pageIndex, pageSize);
            if (homeStay == null)
            {
                return new BaseResponse<IEnumerable<SimpleHomeStayResponse>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var homeStays = _mapper.Map<IEnumerable<SimpleHomeStayResponse>>(homeStay);
            if (homeStays == null)
            {
                return new BaseResponse<IEnumerable<SimpleHomeStayResponse>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<SimpleHomeStayResponse>>("Get all HomeStay as base success",
                StatusCodeEnum.OK_200, homeStays);
        }
        public async Task<BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>> CreateHomeStayWithRentalsAndPricingAsync(CreateHomeStayWithRentalsAndPricingRequest request)
        {
            try
            {
                // Step 1: Kiểm tra dữ liệu bắt buộc
                if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Description) ||
                    string.IsNullOrEmpty(request.Address) || string.IsNullOrEmpty(request.AccountID))
                {
                    return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                        "Required fields are missing (Name, Description, Address, AccountID)!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                if (request.Longtitude == 0 || request.Latitude == 0)
                {
                    return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                        "Longtitude and Latitude must be valid numbers!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                // Step 2: Xử lý Pricing
                List<PricingForHomeStayRental> pricingList = null;
                if (!string.IsNullOrEmpty(request.PricingJson))
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    if (request.PricingJson.TrimStart().StartsWith("["))
                    {
                        pricingList = JsonSerializer.Deserialize<List<PricingForHomeStayRental>>(request.PricingJson, options);
                    }
                    else
                    {
                        var singlePricing = JsonSerializer.Deserialize<PricingForHomeStayRental>(request.PricingJson, options);
                        pricingList = new List<PricingForHomeStayRental> { singlePricing };
                    }
                }
                else if (request.Pricing != null && request.Pricing.Any())
                {
                    pricingList = request.Pricing;
                }

                // Step 2.1: Xử lý RoomTypes
                List<CreateRoomTypeRequest> roomTypeList = null;
                if (!string.IsNullOrEmpty(request.RoomTypesJson))
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    if (request.RoomTypesJson.TrimStart().StartsWith("["))
                    {
                        roomTypeList = JsonSerializer.Deserialize<List<CreateRoomTypeRequest>>(request.RoomTypesJson, options);
                    }
                    else
                    {
                        var singleRoomType = JsonSerializer.Deserialize<CreateRoomTypeRequest>(request.RoomTypesJson, options);
                        roomTypeList = new List<CreateRoomTypeRequest> { singleRoomType };
                    }
                }
                else if (request.RoomTypes != null && request.RoomTypes.Any())
                {
                    roomTypeList = request.RoomTypes;
                }

                // Gán giá trị mặc định
                request.Status ??= true;
                request.RentWhole ??= true;

                // Kiểm tra Pricing
                if (pricingList != null && pricingList.Any())
                {
                    foreach (var pricingItem in pricingList)
                    {
                        if (pricingItem.DayType == DayType.Weekday && pricingItem.RentPrice <= 0)
                        {
                            return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                                "UnitPrice and RentPrice are required and must be greater than 0 for Weekday pricing!",
                                StatusCodeEnum.BadRequest_400,
                                null);
                        }
                    }
                }

                // Step 3: Tạo HomeStay
                var homestay = new HomeStay
                {
                    Name = request.Name,
                    Description = request.Description,
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow,
                    Status = HomeStayStatus.PendingApproval,
                    TypeOfRental = request.RentalType,
                    Address = request.Address,
                    Area = request.Area,
                    Longitude = request.Longtitude,
                    Latitude = request.Latitude,
                    AccountID = request.AccountID
                };

                await _homeStayRepository.AddAsync(homestay);
                await _homeStayRepository.SaveChangesAsync();

                if (request.Images != null && request.Images.Any())
                {
                    var imageUrls = await UploadImagesToCloudinary(request.Images);
                    foreach (var url in imageUrls)
                    {
                        var imageHomeStay = new ImageHomeStay { Image = url, HomeStayID = homestay.HomeStayID };
                        await _imageHomeStayRepository.AddImageAsync(imageHomeStay);
                    }
                    await _imageHomeStayRepository.SaveChangesAsync();
                }

                // Step 4: Tạo HomeStayRental
                var rental = new HomeStayRentals
                {
                    Name = request.RentalName ?? "Whole HomeStay",
                    Description = request.RentalDescription ?? "Whole HomeStay Rental",
                    MaxAdults = request.MaxAdults,
                    MaxChildren = request.MaxChildren,
                    MaxPeople = request.MaxPeople,
                    HomeStayID = homestay.HomeStayID,
                    numberBedRoom = request.numberBedRoom,
                    numberBathRoom = request.numberBathRoom,
                    numberKitchen = request.numberKitchen,
                    numberWifi = request.numberWifi,
                    Status = request.Status ?? true,
                    RentWhole = request.RentWhole ?? true,
                    CreateAt = DateTime.UtcNow
                };

                if (request.RentalImages != null && request.RentalImages.Any())
                {
                    var imageUrls = await UploadImagesToCloudinary(request.RentalImages);
                    rental.ImageHomeStayRentals = imageUrls.Select(url => new ImageHomeStayRentals
                    {
                        Image = url,
                        HomeStayRentalID = rental.HomeStayRentalID
                    }).ToList();
                }

                await _homeStayRentalRepository.AddAsync(rental);
                await _homeStayRentalRepository.SaveChangesAsync();

               

                if (rental.ImageHomeStayRentals != null && rental.ImageHomeStayRentals.Any())
                {
                    foreach (var image in rental.ImageHomeStayRentals)
                    {
                        image.HomeStayRentalID = rental.HomeStayRentalID;
                        await _imageHomeStayRepository.AddImageAsync(new ImageHomeStay
                        {
                            Image = image.Image,
                            HomeStayID = rental.HomeStayID
                        });
                    }
                    await _imageHomeStayRepository.SaveChangesAsync();
                }

                // Step 5: Xử lý Pricing
                var pricings = new List<Pricing>();
                if (pricingList != null && pricingList.Any())
                {
                    foreach (var pricingItem in pricingList)
                    {
                        if (pricingItem.DayType == DayType.Weekend || pricingItem.DayType == DayType.Holiday)
                        {
                            var existingPricing = await _priceRepository.GetPricingByHomeStayRentalAsync(rental.HomeStayRentalID);
                            var weekdayPricing = existingPricing.FirstOrDefault(p => p.DayType == DayType.Weekday);
                            if (weekdayPricing == null)
                            {
                                return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                                    "Cannot create Pricing for Weekend or Holiday because no Weekday Pricing exists!",
                                    StatusCodeEnum.BadRequest_400,
                                    null);
                            }
                            if (pricingItem.Percentage <= 0.0)
                            {
                                return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                                    "Percentage must be greater than 0 for Weekend or Holiday pricing!",
                                    StatusCodeEnum.BadRequest_400,
                                    null);
                            }
                            pricingItem.RentPrice = (int)(weekdayPricing.RentPrice * (1 + pricingItem.Percentage / 100));
                        }

                        var pricing = new Pricing
                        {
                            Description = pricingItem.Description,
                            RentPrice = pricingItem.RentPrice,
                            Percentage = pricingItem.Percentage ?? 0,
                            StartDate = pricingItem.IsDefault ? null : pricingItem.StartDate,
                            EndDate = pricingItem.IsDefault ? null : pricingItem.EndDate,
                            IsDefault = pricingItem.IsDefault,
                            IsActive = pricingItem.IsActive,
                            DayType = pricingItem.DayType,
                            HomeStayRentalID = rental.HomeStayRentalID
                        };
                        pricings.Add(pricing);
                    }

                    await _priceRepository.AddRange(pricings);
                    await _priceRepository.SaveChangesAsync();
                }

                // Step 6: Xử lý RoomTypes
                var roomTypes = new List<RoomTypes>();
                var roomTypeResponses = new List<CreateRoomTypeResponse>();

                if (roomTypeList != null && roomTypeList.Any())
                {
                    if (rental.RentWhole)
                    {
                        return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                            "Cannot create RoomType because RentWhole is true!",
                            StatusCodeEnum.BadRequest_400,
                            null);
                    }

                    foreach (var roomTypeRequest in roomTypeList)
                    {
                        if (roomTypeRequest == null)
                        {
                            return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                                "RoomType request cannot be null!",
                                StatusCodeEnum.BadRequest_400,
                                null);
                        }

                        var roomType = _mapper.Map<RoomTypes>(roomTypeRequest);
                        roomType.CreateAt = DateTime.UtcNow;
                        roomType.UpdateAt = DateTime.UtcNow;
                        roomType.HomeStayRentalID = rental.HomeStayRentalID;
                        roomType.Status = true;

                        await _roomTypeRepository.AddAsync(roomType);
                        await _roomTypeRepository.SaveChangesAsync();

                        List<PricingForHomeStayRental> roomTypePricingList = null;
                        if (!string.IsNullOrEmpty(roomTypeRequest.PricingJson))
                        {
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            if (roomTypeRequest.PricingJson.TrimStart().StartsWith("["))
                            {
                                roomTypePricingList = JsonSerializer.Deserialize<List<PricingForHomeStayRental>>(roomTypeRequest.PricingJson, options);
                            }
                            else
                            {
                                var singlePricing = JsonSerializer.Deserialize<PricingForHomeStayRental>(roomTypeRequest.PricingJson, options);
                                roomTypePricingList = new List<PricingForHomeStayRental> { singlePricing };
                            }
                        }
                        else if (roomTypeRequest.Pricing != null && roomTypeRequest.Pricing.Any())
                        {
                            roomTypePricingList = roomTypeRequest.Pricing;
                        }

                        var roomTypePricings = new List<Pricing>();
                        if (roomTypePricingList != null && roomTypePricingList.Any())
                        {
                            foreach (var pricingItem in roomTypePricingList)
                            {
                                if (pricingItem.DayType == DayType.Weekend || pricingItem.DayType == DayType.Holiday)
                                {
                                    var existingPricing = await _priceRepository.GetPricingByRoomTypeAsync(roomType.RoomTypesID);
                                    var weekdayPricing = existingPricing.FirstOrDefault(p => p.DayType == DayType.Weekday);
                                    if (weekdayPricing == null)
                                    {
                                        return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                                            "Cannot create Pricing for Weekend or Holiday because no Weekday Pricing exists for RoomType!",
                                            StatusCodeEnum.BadRequest_400,
                                            null);
                                    }
                                    if (pricingItem.Percentage <= 0.0)
                                    {
                                        return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                                            "Percentage must be greater than 0 for Weekend or Holiday pricing in RoomType!",
                                            StatusCodeEnum.BadRequest_400,
                                            null);
                                    }
                                    pricingItem.RentPrice = (int)(weekdayPricing.RentPrice * (1 + pricingItem.Percentage / 100));
                                }

                                var pricing = new Pricing
                                {
                                    Description = pricingItem.Description,
                                    RentPrice = pricingItem.RentPrice,
                                    Percentage = pricingItem.Percentage ?? 0,
                                    StartDate = pricingItem.IsDefault ? null : pricingItem.StartDate,
                                    EndDate = pricingItem.IsDefault ? null : pricingItem.EndDate,
                                    IsDefault = pricingItem.IsDefault,
                                    IsActive = pricingItem.IsActive,
                                    DayType = pricingItem.DayType,
                                    RoomTypesID = roomType.RoomTypesID,
                                    HomeStayRentalID = null
                                };
                                roomTypePricings.Add(pricing);
                            }

                            await _priceRepository.AddRange(roomTypePricings);
                            await _priceRepository.SaveChangesAsync();

                            // Tải lại roomType với dữ liệu đầy đủ
                            roomType = await _roomTypeRepository.GetRoomTypeByID(roomType.RoomTypesID);
                            roomType.Prices = roomTypePricings;
                        }

                        roomTypes.Add(roomType);
                        var roomTypeResponse = _mapper.Map<CreateRoomTypeResponse>(roomType);
                        roomTypeResponses.Add(roomTypeResponse);
                    }
                }

                var response = new CreateHomeStayWithRentalsAndPricingResponse
                {
                    HomeStay = _mapper.Map<HomeStayResponse>(homestay),
                    Rentals = _mapper.Map<List<GetAllHomeStayType>>(new List<HomeStayRentals> { rental }),
                    Pricings = _mapper.Map<List<PricingResponse>>(pricings),
                    RoomTypes = roomTypeResponses
                };

                return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                    "HomeStay, Rental, and Pricings created successfully, Please Wait for Accepting",
                    StatusCodeEnum.Created_201,
                    response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                    $"Something went wrong! Error: {ex.Message}. Inner Exception: {ex.InnerException?.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }

        public async Task<BaseResponse<List<GetOwnerUser>>> GetOwnersWithHomeStayStats()
        {
            var owners = await _homeStayRepository.GetOwnersWithHomeStayStats();
            var result = owners.Select(a => new GetOwnerUser
            {
                AccountID = a.Account.Id,
                Email = a.Account.Email,
                Name = a.Account.Name,
                Phone = a.Account.Phone,
                Address = a.Account.Address,
                TotalHomeStay = a.TotalHomeStays
            }).ToList();
            return new BaseResponse<List<GetOwnerUser>>("Get All Success", StatusCodeEnum.OK_200, result);
        }

        public async Task<BaseResponse<List<GetTrendingHomeStay>>> GetTrendingHomeStays(int top = 10)
        {
            try
            {
                var trendingData = await _homeStayRepository.GetTrendingHomeStaysAsync(top);

                var result = trendingData
                    .Select(item => new GetTrendingHomeStay
                    {
                        HomeStays = _mapper.Map<SingleHomeStayResponse>(item.HomeStay),
                        AvgRating = Math.Round(item.AverageRating, 2),
                        RatingCount = item.RatingCount
                    })
                    .ToList();

                return new BaseResponse<List<GetTrendingHomeStay>>("Get All Success", StatusCodeEnum.OK_200, result);
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<GetTrendingHomeStay>>($"Đã xảy ra lỗi: {ex.Message}", StatusCodeEnum.BadGateway_502, null);
            }
        }

        public async Task<BaseResponse<List<GetTopLoyalOwners>>> GetTopLoyalOwnersAsync(int top = 5)
        {
            var total = await _homeStayRepository.GetTopLoyalOwnersAsync(top);
            var response = total.Select(p => new GetTopLoyalOwners
            {
                accountID = p.accountID,
                ownerName = p.ownerName,
                totalHomeStays = p.totalHomeStays
            }).ToList();
            if (response == null || !response.Any())
            {
                return new BaseResponse<List<GetTopLoyalOwners>>("Get Total Fail", StatusCodeEnum.BadRequest_400, null);
            }
            return new BaseResponse<List<GetTopLoyalOwners>>("Get All Success", StatusCodeEnum.OK_200, response);
        }
    }
}
