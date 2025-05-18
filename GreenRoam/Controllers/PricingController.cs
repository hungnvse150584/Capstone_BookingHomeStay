using BusinessObject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Pricing;
using Service.RequestAndResponse.Response.Pricing;
using Service.Service;

namespace GreenRoam.Controllers
{
    [Route("api/homestay")]
    [ApiController]
    public class PricingController : ControllerBase
    {
        private readonly IPricingService _pricingService;
        public PricingController(IPricingService pricingService)
        {
            _pricingService = pricingService;
        }

        [Authorize(Roles = "Admin, Owner, Staff, Customer")]
        [HttpGet]
        [Route("GetAllPricing/{homestayID}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllPricing>>>> GetAllPricingByHomeStay(int homestayID)
        {
            var pricing = await _pricingService.GetAllPricingByHomeStayAsync(homestayID);
            return Ok(pricing);
        }

        [Authorize(Roles = "Admin, Owner, Staff, Customer")]
        [HttpGet]
        [Route("GetAllPricingByHomeStayRental/{rentalID}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllPricing>>>> GetPricingByHomeStayRental(int rentalID)
        {
            var pricing = await _pricingService.GetPricingByHomeStayRentalAsync(rentalID);
            return Ok(pricing);
        }

        [Authorize(Roles = "Admin, Owner, Staff, Customer")]
        [HttpGet]
        [Route("GetAllPricingByRoomType/{roomTypeID}")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllPricing>>>> GetPricingByRoomType(int roomTypeID)
        {
            var pricing = await _pricingService.GetPricingByRoomTypeAsync(roomTypeID);
            return Ok(pricing);
        }

        [Authorize(Roles = "Admin, Owner, Staff, Customer")]
        [HttpGet]
        [Route("GetPricingByID/{id}")]
        public async Task<ActionResult<BaseResponse<GetAllPricing>>> GetPricingById(int id)
        {
            var pricing = await _pricingService.GetPricingByIdAsync(id);
            return Ok(pricing);
        }


        [HttpGet]
        [Route("GetTotalPrice")]
        public async Task<ActionResult<BaseResponse<GetTotalPrice>>> GetTotalPrice(DateTime checkInDate, DateTime checkOutDate, int? homeStayRentalId, int? roomTypeId)
        {
            var pricing = await _pricingService.GetTotalPrice(checkInDate, checkOutDate, homeStayRentalId, roomTypeId);
            return Ok(pricing);
        }

        [HttpGet]
        [Route("GetDateType")]
        public async Task<ActionResult<DayType>> GetDayType(DateTime dateTime, int? homeStayRentalId, int? roomtypeId)
        {
            var date = await _pricingService.GetDayType(dateTime, homeStayRentalId, roomtypeId);
            return Ok(date);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpPost]
        [Route("CreatePricing")]
        public async Task<ActionResult<BaseResponse<Pricing>>> CreatePricing(CreatePricingRequest typeRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pricing = await _pricingService.CreatePricing(typeRequest);

            if (pricing == null)
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }

            return Ok(pricing);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpPut]
        [Route("UpdatePricing")]
        public async Task<ActionResult<BaseResponse<Pricing>>> UpdatePricing(int pricingID, UpdatePricingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pricingID <= 0)
            {
                return BadRequest("Invalid pricingID.");
            }

            if (request == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            var result = await _pricingService.UpdatePricing(pricingID, request);

            if (result == null)
            {
                return StatusCode(500, "An error occurred while updating the HomeStay.");
            }

            return Ok(result);
        }
    }
}
