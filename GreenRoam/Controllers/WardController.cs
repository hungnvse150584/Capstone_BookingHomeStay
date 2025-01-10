using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Ward;
using Service.RequestAndResponse.Response.Districts;
using Service.RequestAndResponse.Response.Wards;
using Service.Service;

namespace GreenRoam.Controllers
{
    [Route("api/ward")]
    [ApiController]
    public class WardController : ControllerBase
    {
        private readonly IWardService _wardService;
        public WardController(IWardService wardService)
        {
            _wardService = wardService;
        }

        [HttpGet]
        [Route("GetAllWard")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllWard>>>> GetAllWard()
        {
            var wards = await _wardService.GetAllWardFromBase();
            return Ok(wards);
        }

        [HttpGet]
        [Route("GetWard/{id}")]
        public async Task<ActionResult<BaseResponse<GetAllWard>>> GetWardById(int id)
        {
            if (id == 0 || id == null)
            {
                return BadRequest("Please Input Id!");
            }
            return await _wardService.GetWardByIdFromBase(id);
        }

        [HttpPost]
        [Route("CreateWard")]
        public async Task<ActionResult<BaseResponse<AddWardRequest>>> CreateWardFromBase(AddWardRequest wardRequest)
        {
            if (wardRequest == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var ward = await _wardService.CreateWardFromBase(wardRequest);
            return ward;
        }

        [HttpPut]
        [Route("UpdateWard")]
        public async Task<ActionResult<BaseResponse<UpdateWardRequest>>> UpdateWardFromBase(int id, UpdateWardRequest wardRequest)
        {
            if (id == 0 || id == null)
            {
                return BadRequest("Please Input Id!");
            }
            return await _wardService.UpdateWardFromBase(id, wardRequest);
        }
    }
}
