using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Province;
using Service.RequestAndResponse.Response.Provinces;

namespace GreenRoam.Controllers
{
    [Route("api/province")]
    [ApiController]
    public class ProvinceController : ControllerBase
    {
        private readonly IProvinceService _provinceService;
        public ProvinceController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }

        [HttpGet]
        [Route("GetAllProvince")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllProvince>>>> GetAllProvinces()
        {
            var provinces = await _provinceService.GetAllProvinceFromBase();
            return Ok(provinces);
        }

        [HttpGet]
        [Route("GetProvince/{id}")]
        public async Task<ActionResult<BaseResponse<GetAllProvince>>> GetProvinceById(int id)
        {
            if (id == 0 || id == null)
            {
                return BadRequest("Please Input Id!");
            }
            return await _provinceService.GetProvinceByIdFromBase(id);
        }

        [HttpPost]
        [Route("CreateProvince")]
        public async Task<ActionResult<BaseResponse<AddProvinceRequest>>> CreateProvince([FromBody] AddProvinceRequest provinceRequest)
        {
            if (provinceRequest == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var province = await _provinceService.CreateProvinceFromBase(provinceRequest);
            return province;
        }

        [HttpPut]
        [Route("UpdateProvince")]
        public async Task<ActionResult<BaseResponse<UpdateProvinceRequest>>> UpdateProvince(int id, [FromBody] UpdateProvinceRequest provinceRequest)
        {
            if (id == 0 || id == null)
            {
                return BadRequest("Please Input Id!");
            }
            return await _provinceService.UpdateProvinceFromBase(id, provinceRequest);
        }
    }
}
