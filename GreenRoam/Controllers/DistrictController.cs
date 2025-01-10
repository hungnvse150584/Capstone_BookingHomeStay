using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.District;
using Service.RequestAndResponse.Response.Districts;
using Service.Service;

namespace GreenRoam.Controllers
{
    [Route("api/district")]
    [ApiController]
    public class DistrictController : ControllerBase
    {
        private readonly IDistrictService _districtService;
        public DistrictController(IDistrictService districtService)
        {
            _districtService = districtService;
        }

        [HttpGet]
        [Route("GetAllDistrict")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllDistrict>>>> GetAllDistrict()
        {
            var districts = await _districtService.GetAllDistrictFromBase();
            return Ok(districts);
        }

        [HttpGet]
        [Route("GetDistrict/{id}")]
        public async Task<ActionResult<BaseResponse<GetAllDistrict>>> GetDistrictById(int id)
        {
            if (id == 0 || id == null)
            {
                return BadRequest("Please Input Id!");
            }
            return await _districtService.GetDistrictByIdFromBase(id);
        }

        [HttpPost]
        [Route("CreateDistrict")]
        public async Task<ActionResult<BaseResponse<AddDistrictRequest>>> CreateDistrict([FromBody] AddDistrictRequest districtRequest)
        {
            if (districtRequest == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var district = await _districtService.CreateDistrictFromBase(districtRequest);
            return district;
        }

        [HttpPut]
        [Route("UpdateDistrict")]
        public async Task<ActionResult<BaseResponse<UpdateDistrictRequest>>> UpdateDistrict(int id, UpdateDistrictRequest districtRequest)
        {
            if (id == 0 || id == null)
            {
                return BadRequest("Please Input Id!");
            }
            return await _districtService.UpdateDistrictFromBase(id, districtRequest);
        }
    }
}
