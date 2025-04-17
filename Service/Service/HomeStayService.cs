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
using Service.RequestAndResponse.Request.Pricing;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.ImageHomeStay;
using Service.RequestAndResponse.Response.Pricing;
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
        private readonly IPricingRepository _priceRepository; // Add this
        private readonly Cloudinary _cloudinary;

        public HomeStayService(
            IMapper mapper,
            IHomeStayRepository homeStayRepository,
            IImageHomeStayRepository imageHomeStayRepository,
            IHomeStayRentalRepository homeRentalRepository,
            IPricingRepository priceRepository, // Add this
            Cloudinary cloudinary)
        {
            _mapper = mapper;
            _homeStayRepository = homeStayRepository;
            _imageHomeStayRepository = imageHomeStayRepository;
            _cloudinary = cloudinary;
            _homeStayRentalRepository = homeRentalRepository;
            _priceRepository = priceRepository; // Add this
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
            HomeStay homeStay = await _homeStayRepository.GetHomeStayDetailByIdAsync(id);
            var result = _mapper.Map<SimpleHomeStayResponse>(homeStay);
            return new BaseResponse<SimpleHomeStayResponse>("Get HomeStay as base success", StatusCodeEnum.OK_200, result);
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
                            Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is excluded due to insufficient capacity.");
                            continue;
                        }

                        // Lấy danh sách phòng trong rental
                        var roomTypes = rental.RoomTypes ?? new List<RoomTypes>();
                        var allRooms = roomTypes.SelectMany(rt => rt.Rooms ?? new List<Room>()).ToList();

                        if (!allRooms.Any())
                        {
                            Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is excluded due to no rooms.");
                            continue;
                        }

                        // Lấy danh sách phòng đã được đặt trong khoảng thời gian yêu cầu
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
                                    bookedRoomIds.Add((int)detail.RoomID);
                                }
                            }
                        }

                        // Kiểm tra xem có phòng nào còn trống không
                        bool isRentalAvailable = allRooms.Any(room => !bookedRoomIds.Contains(room.RoomID) && room.isActive);

                        if (isRentalAvailable)
                        {
                            hasAvailableRental = true;
                            Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is available (has available rooms).");
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"HomeStayID: {homeStay.HomeStayID}, RentalID: {rental.HomeStayRentalID} is not available (all rooms are booked).");
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

                return new BaseResponse<IEnumerable<HomeStayResponse>>(
                    finalHomeStays.Any() ? "HomeStays filtered successfully!" : "No HomeStays available for the given criteria.",
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

                // Step 2: Xử lý Pricing (tương tự CreateHomeStayType)
                List<PricingForHomeStayRental> pricingList = null;
                if (!string.IsNullOrEmpty(request.PricingJson))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

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

                // Gán giá trị mặc định cho Status và RentWhole nếu là null
                request.Status ??= true;
                request.RentWhole ??= true;

                // Kiểm tra RentWhole và Pricing
                if (request.RentWhole.Value && (pricingList == null || !pricingList.Any()))
                {
                    return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                        "Pricing must be provided when RentWhole is true!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }
                if (!request.RentWhole.Value && pricingList != null && pricingList.Any())
                {
                    return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                        "Pricing cannot be provided when RentWhole is false!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                // Step 3: Tạo HomeStay (tương tự RegisterHomeStay)
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
                        var imageHomeStay = new ImageHomeStay
                        {
                            Image = url,
                            HomeStayID = homestay.HomeStayID
                        };
                        await _imageHomeStayRepository.AddImageAsync(imageHomeStay);
                    }
                    await _imageHomeStayRepository.SaveChangesAsync();
                }

                // Step 4: Tạo HomeStayRental và tự động gán HomeStayID
                var rental = new HomeStayRentals
                {
                    Name = request.RentalName ?? "Whole HomeStay",
                    Description = request.RentalDescription ?? "Whole HomeStay Rental",
                    MaxAdults = request.MaxAdults,
                    MaxChildren = request.MaxChildren,
                    MaxPeople = request.MaxPeople,
                    HomeStayID = homestay.HomeStayID, // Tự động gán HomeStayID từ HomeStay vừa tạo
                    numberBedRoom = request.numberBedRoom,
                    numberBathRoom = request.numberBathRoom,
                    numberKitchen = request.numberKitchen,
                    numberWifi = request.numberWifi,
                    Status = request.Status ?? true,
                    RentWhole = request.RentWhole ?? true,
                    CreateAt = DateTime.UtcNow
                };

                // Upload ảnh cho HomeStayRental nếu có
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

                // Gán HomeStayRentalID cho ImageHomeStayRentals sau khi lưu
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

                // Step 5: Kiểm tra RentWhole và Pricing (tương tự CreateHomeStayType)
                if (rental.RentWhole && (pricingList == null || !pricingList.Any()))
                {
                    return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                        "Pricing must be provided when RentWhole is true!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }
                if (!rental.RentWhole && pricingList != null && pricingList.Any())
                {
                    return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                        "Pricing cannot be provided when RentWhole is false!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                // Step 6: Tạo Pricing nếu có
                var pricings = new List<Pricing>();
                if (rental.RentWhole && pricingList != null && pricingList.Any())
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

                            pricingItem.UnitPrice = (int)(weekdayPricing.UnitPrice * (1 + pricingItem.Percentage / 100));
                            pricingItem.RentPrice = (int)(weekdayPricing.RentPrice * (1 + pricingItem.Percentage / 100));
                        }
                        else
                        {
                            if (pricingItem.UnitPrice <= 0 || pricingItem.RentPrice <= 0)
                            {
                                return new BaseResponse<CreateHomeStayWithRentalsAndPricingResponse>(
                                    "UnitPrice and RentPrice are required and must be greater than 0 for Weekday pricing!",
                                    StatusCodeEnum.BadRequest_400,
                                    null);
                            }
                        }

                        var pricing = new Pricing
                        {
                            Description = pricingItem.Description,
                            UnitPrice = pricingItem.UnitPrice,
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

                var response = new CreateHomeStayWithRentalsAndPricingResponse
                {
                    HomeStay = _mapper.Map<HomeStayResponse>(homestay),
                    Rentals = _mapper.Map<List<GetAllHomeStayType>>(new List<HomeStayRentals> { rental }),
                    Pricings = _mapper.Map<List<PricingResponse>>(pricings)
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
    }
}
