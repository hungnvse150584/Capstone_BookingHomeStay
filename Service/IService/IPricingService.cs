using BusinessObject.Model;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.HomeStay;
using Service.RequestAndResponse.Request.Pricing;
using Service.RequestAndResponse.Request.RoomType;
using Service.RequestAndResponse.Response.HomeStays;
using Service.RequestAndResponse.Response.Pricing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IPricingService
    {
        Task<BaseResponse<IEnumerable<GetAllPricing>>> GetAllPricingByHomeStayAsync(int homestayID);
        Task<BaseResponse<IEnumerable<GetAllPricing>>> GetPricingByHomeStayRentalAsync(int rentalID);
        Task<BaseResponse<IEnumerable<GetAllPricing>>> GetPricingByRoomTypeAsync(int roomTypeID);
        Task<BaseResponse<GetAllPricing>> GetPricingByIdAsync(int id);
        Task<BaseResponse<PricingResponse>> CreatePricing(CreatePricingRequest typeRequest);
        Task<BaseResponse<Pricing>> UpdatePricing(int pricingID, UpdatePricingRequest request);
        Task<BaseResponse<GetTotalPrice>> GetTotalPrice(DateTime checkInDate, DateTime checkOutDate, int? homeStayRentalId, int? roomTypeId);
        Task<DayType> GetDayType(DateTime date, int? homeStayRentalId, int? roomtypeId);


    }
}
