using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Street;
using Service.RequestAndResponse.Response.Streets;
using Service.Service;

namespace GreenRoam.Controllers
{
    [Route("api/street")]
    [ApiController]
    public class StreetController : ControllerBase
    {
        private readonly IStreetService _streetService;
        public StreetController(IStreetService streetService)
        {
            _streetService = streetService;
        }

        [HttpGet]
        [Route("GetAllStreet")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllStreet>>>> GetAllStreet()
        {
            var streets = await _streetService.GetAllStreetFromBase();
            return Ok(streets);
        }


        [HttpGet]
        [Route("GetStreet/{id}")]
        public async Task<ActionResult<BaseResponse<GetAllStreet>>> GetStreetById(int id)
        {
            if (id == 0 || id == null)
            {
                return BadRequest("Please Input Id!");
            }
            return await _streetService.GetStreetByIdFromBase(id);
        }


        [HttpPost]
        [Route("CreateStreet")]
        public async Task<ActionResult<BaseResponse<AddStreetRequest>>> CreateStreet(AddStreetRequest streetRequest)
        {
            if (streetRequest == null)
            {
                return BadRequest("Please Implement all Information");
            }
            var street = await _streetService.CreateStreetFromBase(streetRequest);
            return street;
        }


        [HttpPut]
        [Route("UpdateStreet")]
        public async Task<ActionResult<BaseResponse<UpdateStreetRequest>>> UpdateStreet(int id, UpdateStreetRequest streetRequest)
        {
            if (id == 0 || id == null)
            {
                return BadRequest("Please Input Id!");
            }
            return await _streetService.UpdateStreetFromBase(id, streetRequest);
        }
    }
}
