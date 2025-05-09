using AutoMapper;
using BusinessObject.Model;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.HomeStayType;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.HomeStayType;
using Service.RequestAndResponse.Response.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Service.RequestAndResponse.Request.Pricing;
using System.Text.Json;
using Service.RequestAndResponse.Request.RoomType;
using Service.RequestAndResponse.Request.Booking;

namespace Service.Service
{
    public class HomeStayRentalService : IHomeStayTypeService
    {
        private readonly IMapper _mapper;
        private readonly IHomeStayRentalRepository _homeStayTypeRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly Cloudinary _cloudinary;
        private readonly IImageHomeStayTypesRepository _imageHomeStayTypesRepository;
        private readonly IPricingRepository _pricingRepository;
        private readonly IRoomTypeService _roomTypeService;
        public HomeStayRentalService(IMapper mapper, IHomeStayRentalRepository homeStayTypeRepository,
             IServiceRepository serviceRepository, Cloudinary cloudinary, IImageHomeStayTypesRepository imageHomeStayTypes, IPricingRepository pricingRepository, IRoomTypeService roomTypeService)
        {
            _mapper = mapper;
            _homeStayTypeRepository = homeStayTypeRepository;
            _serviceRepository = serviceRepository;
            _cloudinary = cloudinary;
            _imageHomeStayTypesRepository = imageHomeStayTypes;
            _pricingRepository = pricingRepository;
            _roomTypeService = roomTypeService;
        }

        public async Task<BaseResponse<IEnumerable<GetAllHomeStayType>>> GetAllHomeStayTypesByHomeStayID(int homestayId)
        {
            IEnumerable<HomeStayRentals> homeStayType = await _homeStayTypeRepository.GetAllHomeStayTypesAsync(homestayId);
            if (homeStayType == null)
            {
                return new BaseResponse<IEnumerable<GetAllHomeStayType>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var homeStayTypes = _mapper.Map<IEnumerable<GetAllHomeStayType>>(homeStayType);
            if (homeStayTypes == null)
            {
                return new BaseResponse<IEnumerable<GetAllHomeStayType>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllHomeStayType>>("Get all HomeStayType as base success",
                StatusCodeEnum.OK_200, homeStayTypes);
        }
        public async Task<BaseResponse<GetHomeStayRentalDetailResponse>> GetHomeStayRentalDetail(int homeStayRentalId)
        {
            try
            {
                // Lấy HomeStayRental theo ID, bao gồm các thông tin liên quan
                var homeStayRental = await _homeStayTypeRepository.GetHomeStayTypesByIdAsync(homeStayRentalId);

                if (homeStayRental == null)
                {
                    return new BaseResponse<GetHomeStayRentalDetailResponse>(
                        "HomeStayRental not found!",
                        StatusCodeEnum.NotFound_404,
                        null);
                }

                // Log để kiểm tra dữ liệu
                Console.WriteLine($"HomeStayRental: ID={homeStayRental.HomeStayRentalID}, Name={homeStayRental.Name}");
                Console.WriteLine($"HomeStay: {homeStayRental.HomeStay?.Name ?? "null"}");
                Console.WriteLine($"ImageHomeStayRentals: {(homeStayRental.ImageHomeStayRentals != null ? homeStayRental.ImageHomeStayRentals.Count() : 0)} items");
                Console.WriteLine($"Prices: {(homeStayRental.Prices != null ? homeStayRental.Prices.Count() : 0)} items");
                Console.WriteLine($"RoomTypes: {(homeStayRental.RoomTypes != null ? homeStayRental.RoomTypes.Count() : 0)} items");
                Console.WriteLine($"BookingDetails: {(homeStayRental.BookingDetails != null ? homeStayRental.BookingDetails.Count() : 0)} items");

                // Ánh xạ sang GetHomeStayRentalDetailResponse
                var response = _mapper.Map<GetHomeStayRentalDetailResponse>(homeStayRental);

                // Gán thêm HomeStayName từ HomeStay
                response.HomeStayName = homeStayRental.HomeStay?.Name;

                return new BaseResponse<GetHomeStayRentalDetailResponse>(
                    "Get HomeStayRental detail successfully!",
                    StatusCodeEnum.OK_200,
                    response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return new BaseResponse<GetHomeStayRentalDetailResponse>(
                    $"An error occurred while getting HomeStayRental detail: {ex.Message}. InnerException: {ex.InnerException?.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }
        public async Task<BaseResponse<HomeStayRentals>> CreateHomeStayType(CreateHomeStayTypeRequest request)
        {
            try
            {
                // Xử lý Pricing: Ưu tiên PricingJson
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
                //if (request.RentWhole.Value && (pricingList == null || !pricingList.Any()))
                //{
                //    return new BaseResponse<HomeStayRentals>(
                //        "Pricing must be provided when RentWhole is true!",
                //        StatusCodeEnum.BadRequest_400,
                //        null);
                //}
                //if (!request.RentWhole.Value && pricingList != null && pricingList.Any())
                //{
                //    return new BaseResponse<HomeStayRentals>(
                //        "Pricing cannot be provided when RentWhole is false!",
                //        StatusCodeEnum.BadRequest_400,
                //        null);
                //}

                // Ánh xạ request sang HomeStayRentals
                var homeStayRental = _mapper.Map<HomeStayRentals>(request);
                homeStayRental.CreateAt = DateTime.Now;

                await _homeStayTypeRepository.AddAsync(homeStayRental);
                await _homeStayTypeRepository.SaveChangesAsync();

                // Xử lý Images nếu có
                if (request.Images != null && request.Images.Any())
                {
                    var imageUrls = await UploadImagesToCloudinary(request.Images);
                    foreach (var url in imageUrls)
                    {
                        var imageRental = new ImageHomeStayRentals
                        {
                            Image = url,
                            HomeStayRentalID = homeStayRental.HomeStayRentalID,
                        };
                        await _imageHomeStayTypesRepository.AddImageAsync(imageRental);
                    }
                    await _imageHomeStayTypesRepository.SaveChangesAsync();
                }

                // Xử lý Pricing nếu có
                if (request.RentWhole.Value && pricingList != null && pricingList.Any())
                {
                    // Kiểm tra nếu có Pricing cho Weekend hoặc Holiday thì phải có Weekday trước
                    foreach (var pricingItem in pricingList)
                    {
                        // Sửa lỗi CS0019: So sánh DayType đúng cách
                        if (pricingItem.DayType == DayType.Weekend || pricingItem.DayType == DayType.Holiday)
                        {
                            var existingPricing = await _pricingRepository.GetPricingByHomeStayRentalAsync(homeStayRental.HomeStayRentalID);
                            var weekdayPricing = existingPricing.FirstOrDefault(p => p.DayType == DayType.Weekday);
                            if (weekdayPricing == null)
                            {
                                return new BaseResponse<HomeStayRentals>(
                                    "Cannot create Pricing for Weekend or Holiday because no Weekday Pricing exists!",
                                    StatusCodeEnum.BadRequest_400,
                                    null);
                            }

                            // Sửa lỗi CS0266: So sánh với 0.0 hoặc ép kiểu
                            if (pricingItem.Percentage <= 0.0) // So sánh với 0.0 thay vì 0 để tránh lỗi chuyển đổi kiểu
                            {
                                return new BaseResponse<HomeStayRentals>(
                                    "Percentage must be greater than 0 for Weekend or Holiday pricing!",
                                    StatusCodeEnum.BadRequest_400,
                                    null);
                            }

                            // Tính giá dựa trên phần trăm tăng so với Weekday
                            pricingItem.UnitPrice = (int)(weekdayPricing.UnitPrice * (1 + pricingItem.Percentage / 100));
                            pricingItem.RentPrice = (int)(weekdayPricing.RentPrice * (1 + pricingItem.Percentage / 100));
                        }
                    }

                    // Tiếp tục lưu Pricing
                    var prices = _mapper.Map<ICollection<Pricing>>(pricingList);
                    foreach (var price in prices)
                    {
                        price.PricingID = 0;
                        price.HomeStayRentalID = homeStayRental.HomeStayRentalID;
                        price.RoomTypesID = null;
                        price.StartDate = price.IsDefault ? null : price.StartDate;
                        price.EndDate = price.IsDefault ? null : price.EndDate;
                        await _pricingRepository.AddAsync(price);
                    }
                    await _pricingRepository.SaveChangesAsync();

                    homeStayRental.Prices = prices;
                }

                return new BaseResponse<HomeStayRentals>(
                    "HomeStayType created successfully!",
                    StatusCodeEnum.Created_201,
                    homeStayRental);
            }
            catch (Exception ex)
            {
                return new BaseResponse<HomeStayRentals>(
                    $"An error occurred while creating HomeStayType: {ex.Message}. InnerException: {ex.InnerException?.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }
        private async Task<List<string>> UploadImagesToCloudinary(List<IFormFile> files)
        {
            if (files == null || !files.Any())
            {
                return new List<string>(); 
            }

            var urls = new List<string>();

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    continue; 
                }

                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "HomeStayRentalImages"
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






        public async Task<BaseResponse<string>> DeleteHomeStayRental(int id)
        {
            var rental = await _homeStayTypeRepository.GetByIdAsync(id);
            await _homeStayTypeRepository.DeleteAsync(rental);
            return new BaseResponse<string>("Delete homestay rental success", StatusCodeEnum.OK_200, "Deleted successfully");
        }

        public async Task<BaseResponse<IEnumerable<GetAllHomeStayType>>> GetAllHomeStayTypes()
        {
            IEnumerable<HomeStayRentals> homeStayType = await _homeStayTypeRepository.GetAllAsync();
            if (homeStayType == null)
            {
                return new BaseResponse<IEnumerable<GetAllHomeStayType>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var homeStayTypes = _mapper.Map<IEnumerable<GetAllHomeStayType>>(homeStayType);
            if (homeStayTypes == null)
            {
                return new BaseResponse<IEnumerable<GetAllHomeStayType>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllHomeStayType>>("Get all HomeStayType as base success",
                StatusCodeEnum.OK_200, homeStayTypes);
        }

        public async Task<BaseResponse<IEnumerable<GetAllHomeStayTypeFilter>>> FilterHomeStayRentalsAsync(FilterHomeStayRentalRequest request)
        {
            try
            {
                Console.WriteLine($"Service: RentWhole value received: {request.RentWhole?.ToString() ?? "null"}");

                var checkInDate = request.CheckInDate.Value.Date;
                var checkOutDate = request.CheckOutDate.Value.Date;
                int numberOfAdults = request.NumberOfAdults.Value;
                int numberOfChildren = request.NumberOfChildren ?? 0;

                // Log giá trị RentWhole ngay trước khi gọi DAO
                bool? rentWholeToPass = request.RentWhole;
                Console.WriteLine($"Service: RentWhole value to pass to DAO: {rentWholeToPass?.ToString() ?? "null"}");

                var finalFilteredRentals = await _homeStayTypeRepository.FilterHomeStayRentalsAsync(
                    request.HomeStayID,
                    rentWholeToPass, // Truyền trực tiếp giá trị gốc, không thay đổi
                    checkInDate,
                    checkOutDate,
                    numberOfAdults,
                    numberOfChildren);

                Console.WriteLine($"Service: Number of HomeStayRentals from repository: {finalFilteredRentals.Count()}");
                foreach (var rental in finalFilteredRentals)
                {
                    Console.WriteLine($"Service: HomeStayRentalID: {rental.HomeStayRentalID}, RentWhole: {rental.RentWhole}");
                }

                var response = _mapper.Map<IEnumerable<GetAllHomeStayTypeFilter>>(finalFilteredRentals);

                foreach (var rental in response)
                {
                    var hasBooking = rental.BookingDetails
                        .Any(bd => bd.Booking != null &&
                                  bd.HomeStayRentalID == rental.HomeStayRentalID &&
                                   (bd.Booking.Status == BookingStatus.Pending ||
                                    bd.Booking.Status == BookingStatus.Confirmed ||
                                    bd.Booking.Status == BookingStatus.InProgress) &&
                                   bd.CheckInDate.Date <= checkOutDate &&
                                   bd.CheckOutDate.Date >= checkInDate);

                    var roomIds = rental.RoomTypes
                        .SelectMany(rt => rt.Rooms)
                        .Where(r => r.IsActive)
                        .Select(r => r.RoomID)
                        .ToList();

                    if (rental.RentWhole)
                    {
                        if (hasBooking)
                        {
                            rental.TotalAvailableRooms = 0;
                        }
                        else
                        {
                            rental.TotalAvailableRooms = 1;
                        }
                    }
                    else
                    {
                        if (!roomIds.Any())
                        {
                            rental.TotalAvailableRooms = 0;
                            continue;
                        }

                        if (hasBooking)
                        {
                            rental.TotalAvailableRooms = 0;
                        }
                        else
                        {
                            rental.TotalAvailableRooms = roomIds.Count;
                            foreach (var roomType in rental.RoomTypes)
                            {
                                var roomTypeRoomIds = roomType.Rooms
                                    .Where(r => r.IsActive)
                                    .Select(r => r.RoomID)
                                    .ToList();
                                roomType.AvailableRoomsCount = roomTypeRoomIds.Count;
                            }
                        }
                    }

                    if (request.RentWhole.HasValue)
                    {
                        Console.WriteLine($"Service: Applying RentWhole filter: {request.RentWhole.Value} for HomeStayRentalID: {rental.HomeStayRentalID}");
                        if (request.RentWhole.Value != rental.RentWhole)
                        {
                            rental.TotalAvailableRooms = 0;
                            Console.WriteLine($"Service: HomeStayRentalID: {rental.HomeStayRentalID} excluded due to RentWhole mismatch.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Service: No RentWhole filter applied for HomeStayRentalID: {rental.HomeStayRentalID}.");
                    }
                }

                var filteredResponse = response
                    .Where(r => r.TotalAvailableRooms > 0)
                    .ToList();

                return new BaseResponse<IEnumerable<GetAllHomeStayTypeFilter>>(
                    filteredResponse.Any() ? "HomeStayRentals filtered successfully!" : "No HomeStayRentals available for the given criteria.",
                    StatusCodeEnum.OK_200,
                    filteredResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service: Error occurred: {ex.Message}");
                return new BaseResponse<IEnumerable<GetAllHomeStayTypeFilter>>(
                    $"An error occurred while filtering HomeStayRentals: {ex.Message}",
                    StatusCodeEnum.InternalServerError_500,
                    null);
            }
        }
        public async Task<BaseResponse<HomeStayRentals>> UpdateHomeStayType(int homeStayRentalID, UpdateHomeStayTypeRequest request)
        {
            var existingRental = await _homeStayTypeRepository.GetHomeStayTypesByIdAsync(homeStayRentalID);
            if (existingRental == null)
            {
                return new BaseResponse<HomeStayRentals>(
                        "Cannot find HomeStayRental, check again your ID!",
                        StatusCodeEnum.BadRequest_400,
                null);
            }
            //if (request.Pricing != null && request.Pricing?.Any() == true)
            //{
            //    var existingPricing = existingRental.Prices.ToList();

            //    //var updatedPricingIds = request.Pricing
            //    //                        .Select(d => d.PricingID)
            //    //                        .Where(id => id.HasValue)
            //    //                        .Select(id => id.Value)
            //    //                        .ToList();
            //    var detailsToRemove = await _pricingRepository.GetPricingDetailsToRemoveAsync(homeStayRentalID, updatedPricingIds);

            //    if (detailsToRemove.Any())
            //    {
            //        await _pricingRepository.DeleteRange(detailsToRemove);
            //    }

            //    foreach (var updatePricicing in request.Pricing)
            //    {
            //        if (updatePricicing.PricingID.HasValue)
            //        {
            //            var existedPricing = existingRental.Prices
            //            .FirstOrDefault(d => d.PricingID == updatePricicing.PricingID.Value);

            //            if (existedPricing == null)
            //            {
            //                return new BaseResponse<HomeStayRentals>($"Cannot find existing pricing detail with ID {updatePricicing.PricingID}", StatusCodeEnum.BadRequest_400, null);
            //            }

            //            var existingDetail = existingPricing
            //                .FirstOrDefault(d => d.PricingID == updatePricicing.PricingID.Value);
            //            if (existingDetail != null)
            //            {
            //                if (existingRental.RentWhole == true)
            //                {
            //                    if (existingRental.Prices == null || !existingRental.Prices.Any())
            //                    {
            //                        return new BaseResponse<HomeStayRentals>(
            //                            "No pricing found for this HomeStayRental!",
            //                            StatusCodeEnum.BadRequest_400,
            //                            null);
            //                    }

            //                    if (updatePricicing.RoomTypesID > 0)
            //                    {
            //                        return new BaseResponse<HomeStayRentals>("You cannot select RoomTypeID when update pricing for the whole homestay.",
            //                            StatusCodeEnum.Conflict_409, null);
            //                    }

            //                    if (updatePricicing.StartDate >= updatePricicing.EndDate)
            //                    {
            //                        return new BaseResponse<HomeStayRentals>("StartDate must < EndDate",
            //                            StatusCodeEnum.Conflict_409, null);
            //                    }

            //                    existingDetail.Description = updatePricicing.Description;
            //                    existingDetail.UnitPrice = updatePricicing.UnitPrice;
            //                    existingDetail.RentPrice = updatePricicing.RentPrice;
            //                    existingDetail.IsActive = updatePricicing.IsActive;
            //                    existingDetail.IsDefault = updatePricicing.IsDefault;
            //                    existingDetail.DayType = updatePricicing.DayType;
            //                    existingDetail.HomeStayRentalID = homeStayRentalID;
            //                    existingDetail.RoomTypesID = null;
            //                    existingDetail.StartDate = updatePricicing.IsDefault ? null : updatePricicing.StartDate;
            //                    existingDetail.EndDate = updatePricicing.IsDefault ? null : updatePricicing.EndDate;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            //Tạo 1 pricing mới
            //            if (existingRental.RentWhole == true)
            //            {
            //                if (updatePricicing.RoomTypesID > 0)
            //                {
            //                    return new BaseResponse<HomeStayRentals>("You cannot select RoomTypeID when update pricing for the whole homestay.",
            //                        StatusCodeEnum.Conflict_409, null);
            //                }

            //                if (updatePricicing.StartDate >= updatePricicing.EndDate)
            //                {
            //                    return new BaseResponse<HomeStayRentals>("StartDate must < EndDate",
            //                        StatusCodeEnum.Conflict_409, null);
            //                }

            //                existingRental.Prices.Add(new Pricing
            //                {
            //                    Description = updatePricicing.Description,
            //                    UnitPrice = updatePricicing.UnitPrice,
            //                    RentPrice = updatePricicing.RentPrice,
            //                    IsActive = updatePricicing.IsActive,
            //                    IsDefault = updatePricicing.IsDefault,
            //                    DayType = updatePricicing.DayType,
            //                    HomeStayRentalID = homeStayRentalID,
            //                    RoomTypesID = null,
            //                    StartDate = updatePricicing.IsDefault ? null : updatePricicing.StartDate,
            //                    EndDate = updatePricicing.IsDefault ? null : updatePricicing.EndDate
            //                });
            //            }
            //        }
            //    }
            //}
            
            existingRental.Name = request.Name;
            existingRental.Description = request.Description;
            existingRental.HomeStayID = request.HomeStayID;
            existingRental.numberBathRoom = request.numberBathRoom;
            existingRental.numberBedRoom = request.numberBedRoom;
            existingRental.numberWifi = request.numberWifi;
            existingRental.Status = request.Status;
            existingRental.RentWhole = request.RentWhole;
            existingRental.MaxAdults = request.MaxAdults;
            existingRental.MaxChildren = request.MaxChildren;
            existingRental.MaxPeople = request.MaxAdults + request.MaxChildren;

            await _homeStayTypeRepository.UpdateAsync(existingRental);
            return new BaseResponse<HomeStayRentals>("HomeStayType updated successfully!", StatusCodeEnum.OK_200, existingRental);
        }
    } 
}
