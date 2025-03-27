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
               
                if (!string.IsNullOrEmpty(request.PricingJson))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    // Kiểm tra xem PricingJson có phải là danh sách không
                    if (request.PricingJson.TrimStart().StartsWith("["))
                    {
                        // Nếu là danh sách, deserialize trực tiếp thành List<PricingForHomeStayRental>
                        request.Pricing = JsonSerializer.Deserialize<List<PricingForHomeStayRental>>(request.PricingJson, options);
                    }
                    else
                    {
                        // Nếu là đối tượng đơn lẻ, deserialize thành PricingForHomeStayRental rồi đưa vào danh sách
                        var singlePricing = JsonSerializer.Deserialize<PricingForHomeStayRental>(request.PricingJson, options);
                        request.Pricing = new List<PricingForHomeStayRental> { singlePricing };
                    }

                    if (request.Pricing != null)
                    {
                        Console.WriteLine("Using Pricing from PricingJson.");
                        foreach (var pricing in request.Pricing)
                        {
                            Console.WriteLine($"Pricing from PricingJson: UnitPrice={pricing.UnitPrice}, RentPrice={pricing.RentPrice}, StartDate={pricing.StartDate?.ToString() ?? "null"}, EndDate={pricing.EndDate?.ToString() ?? "null"}, IsDefault={pricing.IsDefault}, IsActive={pricing.IsActive}, DayType={pricing.DayType}, Description={pricing.Description}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("request.Pricing is null after deserialize from PricingJson.");
                    }
                }
                else
                {
                    // Nếu PricingJson rỗng, sử dụng Pricing từ request (nếu có)
                    if (request.Pricing != null && request.Pricing.Any())
                    {
                        Console.WriteLine("Using Pricing directly from request.");
                        foreach (var pricing in request.Pricing)
                        {
                            Console.WriteLine($"Pricing from request: UnitPrice={pricing.UnitPrice}, RentPrice={pricing.RentPrice}, StartDate={pricing.StartDate?.ToString() ?? "null"}, EndDate={pricing.EndDate?.ToString() ?? "null"}, IsDefault={pricing.IsDefault}, IsActive={pricing.IsActive}, DayType={pricing.DayType}, Description={pricing.Description}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Both PricingJson and Pricing are null or empty.");
                    }
                }

                // Gán giá trị mặc định cho Status và RentWhole nếu là null
                request.Status ??= true;
                request.RentWhole ??= true;

                // Log để kiểm tra
                Console.WriteLine($"RentWhole: {request.RentWhole.Value}");
                Console.WriteLine($"Status: {request.Status.Value}");
                Console.WriteLine($"PricingJson: {request.PricingJson ?? "null"}");
                Console.WriteLine($"Pricing: {(request.Pricing != null ? $"Count = {request.Pricing.Count}" : "null")}");

                // Kiểm tra RentWhole và Pricing
                if (request.RentWhole.Value && (request.Pricing == null || !request.Pricing.Any()))
                {
                    return new BaseResponse<HomeStayRentals>(
                        "Pricing must be provided when RentWhole is true!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }
                if (!request.RentWhole.Value && request.Pricing != null && request.Pricing.Any())
                {
                    return new BaseResponse<HomeStayRentals>(
                        "Pricing cannot be provided when RentWhole is false!",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                var homeStayRental = _mapper.Map<HomeStayRentals>(request);
                homeStayRental.CreateAt = DateTime.Now;

                await _homeStayTypeRepository.AddAsync(homeStayRental);
                Console.WriteLine("Saving HomeStayRentals...");
                await _homeStayTypeRepository.SaveChangesAsync();
                Console.WriteLine("HomeStayRentals saved successfully.");

                if (request.Images != null && request.Images.Any())
                {
                    Console.WriteLine("Uploading images to Cloudinary...");
                    var imageUrls = await UploadImagesToCloudinary(request.Images);
                    Console.WriteLine($"Uploaded {imageUrls.Count} images: {string.Join(", ", imageUrls)}");
                    foreach (var url in imageUrls)
                    {
                        var imageRental = new ImageHomeStayRentals
                        {
                            Image = url,
                            HomeStayRentalID = homeStayRental.HomeStayRentalID,
                        };
                        await _imageHomeStayTypesRepository.AddImageAsync(imageRental);
                    }
                    Console.WriteLine("Saving ImageHomeStayRentals...");
                    await _imageHomeStayTypesRepository.SaveChangesAsync();
                    Console.WriteLine("ImageHomeStayRentals saved successfully.");
                }

                // Lưu Pricing sau khi đã lưu HomeStayRental
                if (request.RentWhole.Value && request.Pricing != null && request.Pricing.Any())
                {
                    Console.WriteLine("Saving Prices...");
                    var prices = _mapper.Map<ICollection<Pricing>>(request.Pricing);
                    foreach (var price in prices)
                    {
                        Console.WriteLine($"Before saving - Pricing: UnitPrice={price.UnitPrice}, RentPrice={price.RentPrice}, StartDate={price.StartDate?.ToString() ?? "null"}, EndDate={price.EndDate?.ToString() ?? "null"}, IsDefault={price.IsDefault}, IsActive={price.IsActive}, DayType={price.DayType}, Description={price.Description}");

                        price.PricingID = 0;
                        price.HomeStayRentalID = homeStayRental.HomeStayRentalID;
                        await _pricingRepository.AddAsync(price);
                    }
                    await _pricingRepository.SaveChangesAsync();
                    Console.WriteLine("Prices saved successfully.");

                    // Gán lại Prices vào homeStayRental để trả về trong response
                    homeStayRental.Prices = prices;
                }

                return new BaseResponse<HomeStayRentals>(
                    "HomeStayType created successfully!",
                    StatusCodeEnum.Created_201,
                    homeStayRental);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"InnerException: {ex.InnerException?.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
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

        public async Task<BaseResponse<IEnumerable<GetHomeStayRentalDetailResponse>>> FilterHomeStayRentalsAsync(FilterHomeStayRentalRequest request)
        {
            try
            {
                // Kiểm tra CheckInDate và CheckOutDate hợp lệ
                if (request.CheckInDate > request.CheckOutDate)
                {
                    return new BaseResponse<IEnumerable<GetHomeStayRentalDetailResponse>>(
                        "Check-out date must be after check-in date.",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                if (request.CheckInDate < DateTime.UtcNow.Date)
                {
                    return new BaseResponse<IEnumerable<GetHomeStayRentalDetailResponse>>(
                        "Check-in date cannot be in the past.",
                        StatusCodeEnum.BadRequest_400,
                        null);
                }

                var checkInDate = request.CheckInDate.Date;
                var checkOutDate = request.CheckOutDate.Date;

                // Bước 1: Lấy tất cả HomeStayRentals (đã lọc Status, RentWhole và HomeStayID tại database)
                var filteredHomeStayRentals = await _homeStayTypeRepository.GetAllAsyncFilter(request.RentWhole);

                // Lọc thêm theo HomeStayID
                filteredHomeStayRentals = filteredHomeStayRentals
                    .Where(h => h.HomeStayID == request.HomeStayID)
                    .ToList();

                if (!filteredHomeStayRentals.Any())
                {
                    return new BaseResponse<IEnumerable<GetHomeStayRentalDetailResponse>>(
                        "No HomeStayRentals available with the specified conditions.",
                        StatusCodeEnum.OK_200,
                        new List<GetHomeStayRentalDetailResponse>());
                }

                // Bước 2: Lọc theo CheckIn/CheckOut, MaxAdults, MaxChildren
                var finalFilteredRentals = new List<HomeStayRentals>();
                foreach (var rental in filteredHomeStayRentals)
                {
                    // Kiểm tra sức chứa (MaxAdults, MaxChildren)
                    if (rental.MaxAdults < request.NumberOfAdults ||
                        rental.MaxChildren < request.NumberOfChildren ||
                        rental.MaxPeople < (request.NumberOfAdults + request.NumberOfChildren))
                    {
                        Console.WriteLine($"HomeStayRentalID: {rental.HomeStayRentalID} is excluded due to insufficient capacity. " +
                                          $"MaxAdults: {rental.MaxAdults}, Requested: {request.NumberOfAdults}, " +
                                          $"MaxChildren: {rental.MaxChildren}, Requested: {request.NumberOfChildren}, " +
                                          $"MaxPeople: {rental.MaxPeople}, Requested: {request.NumberOfAdults + request.NumberOfChildren}");
                        continue;
                    }

                    // Kiểm tra CheckIn/CheckOut
                    bool isAvailable = true;
                    var activeBookings = rental.BookingDetails?
                        .Where(bd => bd.Booking != null &&
                                     (bd.Booking.Status == BookingStatus.Pending ||
                                      bd.Booking.Status == BookingStatus.Confirmed ||
                                      bd.Booking.Status == BookingStatus.InProgress))
                        .ToList() ?? new List<BookingDetail>();

                    foreach (var bookingDetail in activeBookings)
                    {
                        var detailCheckInDate = bookingDetail.CheckInDate.Date;
                        var detailCheckOutDate = bookingDetail.CheckOutDate.Date;

                        Console.WriteLine($"HomeStayRentalID: {rental.HomeStayRentalID}, BookingDetailID: {bookingDetail.BookingDetailID}, " +
                                          $"CheckIn: {detailCheckInDate:yyyy-MM-dd}, CheckOut: {detailCheckOutDate:yyyy-MM-dd}, " +
                                          $"Request CheckIn: {checkInDate:yyyy-MM-dd}, Request CheckOut: {checkOutDate:yyyy-MM-dd}");

                        if (detailCheckInDate <= checkOutDate && detailCheckOutDate >= checkInDate)
                        {
                            isAvailable = false;
                            Console.WriteLine($"HomeStayRentalID: {rental.HomeStayRentalID} is not available due to overlapping booking.");
                            break;
                        }
                    }

                    if (isAvailable)
                    {
                        finalFilteredRentals.Add(rental);
                        Console.WriteLine($"HomeStayRentalID: {rental.HomeStayRentalID} is available and added to filtered list.");
                    }
                }

                // Ánh xạ sang GetHomeStayRentalDetailResponse
                var response = _mapper.Map<IEnumerable<GetHomeStayRentalDetailResponse>>(finalFilteredRentals);

                // Gán thêm HomeStayName cho từng item trong response
                foreach (var item in response)
                {
                    var rental = finalFilteredRentals.FirstOrDefault(r => r.HomeStayRentalID == item.HomeStayRentalID);
                    if (rental != null && rental.HomeStay != null)
                    {
                        item.HomeStayName = rental.HomeStay.Name;
                    }
                }

                return new BaseResponse<IEnumerable<GetHomeStayRentalDetailResponse>>(
                    finalFilteredRentals.Any() ? "HomeStayRentals filtered successfully!" : "No HomeStayRentals available for the given criteria.",
                    StatusCodeEnum.OK_200,
                    response);
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<GetHomeStayRentalDetailResponse>>(
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
            if (request.Pricing != null && request.Pricing?.Any() == true)
            {
                var existingPricing = existingRental.Prices.ToList();

                var updatedPricingIds = request.Pricing
                                        .Select(d => d.PricingID)
                                        .Where(id => id.HasValue)
                                        .Select(id => id.Value)
                                        .ToList();
                var detailsToRemove = await _pricingRepository.GetPricingDetailsToRemoveAsync(homeStayRentalID, updatedPricingIds);

                if (detailsToRemove.Any())
                {
                    await _pricingRepository.DeleteRange(detailsToRemove);
                }

                foreach (var updatePricicing in request.Pricing)
                {
                    if (updatePricicing.PricingID.HasValue)
                    {
                        var existedPricing = existingRental.Prices
                        .FirstOrDefault(d => d.PricingID == updatePricicing.PricingID.Value);

                        if (existedPricing == null)
                        {
                            return new BaseResponse<HomeStayRentals>($"Cannot find existing pricing detail with ID {updatePricicing.PricingID}", StatusCodeEnum.BadRequest_400, null);
                        }

                        var existingDetail = existingPricing
                            .FirstOrDefault(d => d.PricingID == updatePricicing.PricingID.Value);
                        if (existingDetail != null)
                        {
                            if (existingRental.RentWhole == true)
                            {
                                if (existingRental.Prices == null || !existingRental.Prices.Any())
                                {
                                    return new BaseResponse<HomeStayRentals>(
                                        "No pricing found for this HomeStayRental!",
                                        StatusCodeEnum.BadRequest_400,
                                        null);
                                }

                                if (updatePricicing.RoomTypesID > 0)
                                {
                                    return new BaseResponse<HomeStayRentals>("You cannot select RoomTypeID when update pricing for the whole homestay.",
                                        StatusCodeEnum.Conflict_409, null);
                                }

                                if (updatePricicing.StartDate >= updatePricicing.EndDate)
                                {
                                    return new BaseResponse<HomeStayRentals>("StartDate must < EndDate",
                                        StatusCodeEnum.Conflict_409, null);
                                }

                                existingDetail.Description = updatePricicing.Description;
                                existingDetail.UnitPrice = updatePricicing.UnitPrice;
                                existingDetail.RentPrice = updatePricicing.RentPrice;
                                existingDetail.IsActive = updatePricicing.IsActive;
                                existingDetail.IsDefault = updatePricicing.IsDefault;
                                existingDetail.DayType = updatePricicing.DayType;
                                existingDetail.HomeStayRentalID = homeStayRentalID;
                                existingDetail.RoomTypesID = null;
                                existingDetail.StartDate = updatePricicing.IsDefault ? null : updatePricicing.StartDate;
                                existingDetail.EndDate = updatePricicing.IsDefault ? null : updatePricicing.EndDate;
                            }
                        }
                    }
                    else
                    {
                        //Tạo 1 pricing mới
                        if (existingRental.RentWhole == true)
                        {
                            if (updatePricicing.RoomTypesID > 0)
                            {
                                return new BaseResponse<HomeStayRentals>("You cannot select RoomTypeID when update pricing for the whole homestay.",
                                    StatusCodeEnum.Conflict_409, null);
                            }

                            if (updatePricicing.StartDate >= updatePricicing.EndDate)
                            {
                                return new BaseResponse<HomeStayRentals>("StartDate must < EndDate",
                                    StatusCodeEnum.Conflict_409, null);
                            }

                            existingRental.Prices.Add(new Pricing
                            {
                                Description = updatePricicing.Description,
                                UnitPrice = updatePricicing.UnitPrice,
                                RentPrice = updatePricicing.RentPrice,
                                IsActive = updatePricicing.IsActive,
                                IsDefault = updatePricicing.IsDefault,
                                DayType = updatePricicing.DayType,
                                HomeStayRentalID = homeStayRentalID,
                                RoomTypesID = null,
                                StartDate = updatePricicing.IsDefault ? null : updatePricicing.StartDate,
                                EndDate = updatePricicing.IsDefault ? null : updatePricicing.EndDate
                            });
                        }
                    }
                }
            }
            
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
