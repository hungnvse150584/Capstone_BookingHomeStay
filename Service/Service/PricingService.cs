using AutoMapper;
using BusinessObject.Model;
using Repository.IRepositories;
using Repository.Repositories;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Enums;
using Service.RequestAndResponse.Request.Pricing;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.Pricing;
using Service.RequestAndResponse.Response.RoomType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Service
{
    public class PricingService : IPricingService
    {
        private readonly IMapper _mapper;
        private readonly IPricingRepository _priceRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IHomeStayRentalRepository _homeStayRentalRepository;

        public PricingService(IMapper mapper, IPricingRepository priceRepository, IRoomTypeRepository roomTypeRepository, IHomeStayRentalRepository homeStayRentalRepository)
        {
            _mapper = mapper;
            _priceRepository = priceRepository;
            _roomTypeRepository = roomTypeRepository;
            _homeStayRentalRepository = homeStayRentalRepository;
        }

        public async Task<BaseResponse<IEnumerable<GetAllPricing>>> GetAllPricingByHomeStayAsync(int homestayID)
        {
            IEnumerable<Pricing> pricing = await _priceRepository.GetAllPricingByHomeStayAsync(homestayID);
            if (pricing == null)
            {
                return new BaseResponse<IEnumerable<GetAllPricing>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var prices = _mapper.Map<IEnumerable<GetAllPricing>>(pricing);
            if (prices == null)
            {
                return new BaseResponse<IEnumerable<GetAllPricing>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllPricing>>("Get all RoomType as base success",
                StatusCodeEnum.OK_200, prices);
        }

        public async Task<BaseResponse<IEnumerable<GetAllPricing>>> GetPricingByHomeStayRentalAsync(int rentalID)
        {
            IEnumerable<Pricing> pricing = await _priceRepository.GetPricingByHomeStayRentalAsync(rentalID);
            if (pricing == null)
            {
                return new BaseResponse<IEnumerable<GetAllPricing>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var prices = _mapper.Map<IEnumerable<GetAllPricing>>(pricing);
            if (prices == null)
            {
                return new BaseResponse<IEnumerable<GetAllPricing>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllPricing>>("Get all RoomType as base success",
                StatusCodeEnum.OK_200, prices);
        }

        public async Task<BaseResponse<GetAllPricing>> GetPricingByIdAsync(int id)
        {
            Pricing pricing = await _priceRepository.GetPricingByIdAsync(id);
            var result = _mapper.Map<GetAllPricing>(pricing);
            return new BaseResponse<GetAllPricing>("Get HomeStay as base success", StatusCodeEnum.OK_200, result);
        }

        public async Task<BaseResponse<IEnumerable<GetAllPricing>>> GetPricingByRoomTypeAsync(int roomTypeID)
        {
            IEnumerable<Pricing> pricing = await _priceRepository.GetPricingByRoomTypeAsync(roomTypeID);
            if (pricing == null)
            {
                return new BaseResponse<IEnumerable<GetAllPricing>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            var prices = _mapper.Map<IEnumerable<GetAllPricing>>(pricing);
            if (prices == null)
            {
                return new BaseResponse<IEnumerable<GetAllPricing>>("Something went wrong!",
                StatusCodeEnum.BadGateway_502, null);
            }
            return new BaseResponse<IEnumerable<GetAllPricing>>("Get all RoomType as base success",
                StatusCodeEnum.OK_200, prices);
        }

        public async Task<BaseResponse<PricingResponse>> CreatePricing(CreatePricingRequest typeRequest)
        {
            // Kiểm tra các trường bắt buộc khác
            var homeStayRental = await _homeStayRentalRepository.GetHomeStayTypesByIdAsync(typeRequest.HomeStayRentalID);
            if (homeStayRental == null)
            {
                return new BaseResponse<PricingResponse>("Cannot Find HomeStayRental!",
                    StatusCodeEnum.BadGateway_502, null);
            }

            if (homeStayRental.RentWhole == true && typeRequest.RoomTypesID.HasValue)
            {
                return new BaseResponse<PricingResponse>("Cannot add RoomTypeID because HomeStayRental is RentWhole!", StatusCodeEnum.BadGateway_502, null);
            }

            if (homeStayRental.RentWhole == false)
            {
                if (!typeRequest.RoomTypesID.HasValue)
                {
                    return new BaseResponse<PricingResponse>("RoomTypesID is required when RentWhole is false!", StatusCodeEnum.BadGateway_502, null);
                }

                var roomType = await _roomTypeRepository.GetRoomTypesByIdAsync(typeRequest.RoomTypesID);
                if (roomType == null || roomType.HomeStayRentalID != typeRequest.HomeStayRentalID)
                {
                    return new BaseResponse<PricingResponse>("Invalid RoomTypeID for this HomeStayRental!", StatusCodeEnum.BadGateway_502, null);
                }
            }

            if (typeRequest.IsDefault && (typeRequest.StartDate.HasValue || typeRequest.EndDate.HasValue))
            {
                return new BaseResponse<PricingResponse>("StartDate and EndDate should not be provided when IsDefault is true!", StatusCodeEnum.BadGateway_502, null);
            }

            // Kiểm tra UnitPrice và RentPrice cho Weekday
            if (typeRequest.DayType == DayType.Weekday)
            {
                if (typeRequest.UnitPrice <= 0 || typeRequest.RentPrice <= 0)
                {
                    return new BaseResponse<PricingResponse>("UnitPrice and RentPrice are required and must be greater than 0 for Weekday pricing!", StatusCodeEnum.BadGateway_502, null);
                }
            }

            // Kiểm tra nếu DayType là Weekend hoặc Holiday
            if (typeRequest.DayType == DayType.Weekend || typeRequest.DayType == DayType.Holiday)
            {
                var existingWeekdayPricing = homeStayRental.RentWhole
                    ? await _priceRepository.GetPricingByHomeStayRentalAsync(typeRequest.HomeStayRentalID.Value)
                    : await _priceRepository.GetPricingByRoomTypeAsync(typeRequest.RoomTypesID.Value);

                // Lấy Pricing mới nhất có DayType = Weekday
                var weekdayPricing = existingWeekdayPricing
                    .Where(p => p.DayType == DayType.Weekday)
                    .OrderByDescending(p => p.StartDate ?? DateTime.MaxValue) // Sắp xếp theo StartDate (mới nhất trước), nếu null thì ưu tiên
                    .FirstOrDefault();

                if (weekdayPricing == null)
                {
                    return new BaseResponse<PricingResponse>("Cannot create Pricing for Weekend or Holiday because no Weekday Pricing exists!", StatusCodeEnum.BadGateway_502, null);
                }

                // Kiểm tra Percentage
                if (typeRequest.Percentage <= 0)
                {
                    return new BaseResponse<PricingResponse>("Percentage must be greater than 0 for Weekend or Holiday pricing!", StatusCodeEnum.BadGateway_502, null);
                }

                // Tính giá dựa trên phần trăm tăng so với giá của Weekday
                typeRequest.UnitPrice = weekdayPricing.UnitPrice * (1 + typeRequest.Percentage / 100);
                typeRequest.RentPrice = weekdayPricing.RentPrice * (1 + typeRequest.Percentage / 100);
            }

            var pricing = new Pricing
            {
                Description = typeRequest.Description,
                UnitPrice = typeRequest.UnitPrice,
                RentPrice = typeRequest.RentPrice,
                Percentage = typeRequest.Percentage,
                IsDefault = typeRequest.IsDefault,
                IsActive = typeRequest.IsActive,
                DayType = typeRequest.DayType,
                HomeStayRentalID = typeRequest.HomeStayRentalID,
                RoomTypesID = typeRequest.RoomTypesID,
                StartDate = typeRequest.IsDefault ? null : typeRequest.StartDate,
                EndDate = typeRequest.IsDefault ? null : typeRequest.EndDate
            };

            await _priceRepository.AddAsync(pricing);

            var pricingResponse = _mapper.Map<PricingResponse>(pricing);

            return new BaseResponse<PricingResponse>("Pricing created successfully!", StatusCodeEnum.OK_200, pricingResponse);
        }

        public async Task<BaseResponse<Pricing>> UpdatePricing(int pricingID, UpdatePricingRequest request)
        {
            var pricingExist = await _priceRepository.GetPricingByIdAsync(pricingID);
            if (pricingExist == null)
            {
                return new BaseResponse<Pricing>("Cannot Find ExistPricing!",
                StatusCodeEnum.BadGateway_502, null);
            }

            var homeStayRental = await _homeStayRentalRepository.GetHomeStayTypesByIdAsync(request.HomeStayRentalID);
            if (homeStayRental == null)
            {
                return new BaseResponse<Pricing>("Cannot find HomeStayRental!", StatusCodeEnum.BadGateway_502, null);
            }

            if (homeStayRental.RentWhole == true && request.RoomTypesID.HasValue)
            {
                return new BaseResponse<Pricing>("Cannot add RoomTypeID because HomeStayRental is RentWhole!", StatusCodeEnum.BadGateway_502, null);
            }

            if (homeStayRental.RentWhole == false)
            {
                if (!request.RoomTypesID.HasValue)
                {
                    return new BaseResponse<Pricing>("RoomTypesID is required when RentWhole is false!", StatusCodeEnum.BadGateway_502, null);
                }

                var roomType = await _roomTypeRepository.GetRoomTypesByIdAsync(request.RoomTypesID);
                if (roomType == null || roomType.HomeStayRentalID != request.HomeStayRentalID)
                {
                    return new BaseResponse<Pricing>("Invalid RoomTypeID for this HomeStayRental!", StatusCodeEnum.BadGateway_502, null);
                }
            }

            if (request.IsDefault && (request.StartDate.HasValue || request.EndDate.HasValue))
            {
                return new BaseResponse<Pricing>("StartDate and EndDate should not be provided when IsDefault is true!", StatusCodeEnum.BadGateway_502, null);
            }

            // Kiểm tra nếu DayType là Weekend hoặc Holiday, phải có Pricing của Weekday trước
            if (request.DayType == DayType.Weekend || request.DayType == DayType.Holiday)
            {
                var existingWeekdayPricing = homeStayRental.RentWhole
                    ? await _priceRepository.GetPricingByHomeStayRentalAsync(request.HomeStayRentalID.Value)
                    : await _priceRepository.GetPricingByRoomTypeAsync(request.RoomTypesID.Value);

                var weekdayPricing = existingWeekdayPricing.FirstOrDefault(p => p.DayType == DayType.Weekday);
                if (weekdayPricing == null)
                {
                    return new BaseResponse<Pricing>("Cannot update Pricing to Weekend or Holiday because no Weekday Pricing exists!", StatusCodeEnum.BadGateway_502, null);
                }

                // Tính giá dựa trên phần trăm tăng so với giá của Weekday
                request.UnitPrice = weekdayPricing.UnitPrice * (1 + request.Percentage / 100);
                request.RentPrice = weekdayPricing.RentPrice * (1 + request.Percentage / 100);
            }

            pricingExist.Description = request.Description;
            pricingExist.UnitPrice = request.UnitPrice;
            pricingExist.RentPrice = request.RentPrice;
            pricingExist.Percentage = request.Percentage; // Lưu phần trăm tăng
            pricingExist.IsDefault = request.IsDefault;
            pricingExist.IsActive = request.IsActive;
            pricingExist.DayType = request.DayType;
            pricingExist.HomeStayRentalID = request.HomeStayRentalID;
            pricingExist.RoomTypesID = request.RoomTypesID;
            pricingExist.StartDate = request.IsDefault ? null : request.StartDate;
            pricingExist.EndDate = request.IsDefault ? null : request.EndDate;

            await _priceRepository.UpdateAsync(pricingExist);

            return new BaseResponse<Pricing>("Pricing updated successfully!", StatusCodeEnum.OK_200, pricingExist);
        }

        public async Task<BaseResponse<GetTotalPrice>> GetTotalPrice(DateTime checkInDate, DateTime checkOutDate, int? homeStayRentalId, int? roomTypeId)
        {
            var prices = await _priceRepository.GetTotalPrice(checkInDate, checkOutDate, homeStayRentalId, roomTypeId);
            var response = new GetTotalPrice
            {
                totalUnitPrice = prices.totalUnitPrice,
                totalRentPrice = prices.totalRentPrice
            };
            if (response == null)
            {
                return new BaseResponse<GetTotalPrice>("Get All Fail", StatusCodeEnum.BadGateway_502, response);
            }
            return new BaseResponse<GetTotalPrice>("Get All Success", StatusCodeEnum.OK_200, response);
        }

        public async Task<DayType> GetDayType(DateTime date)
        {
            return await _priceRepository.GetDayType(date);
        }
    }
}
