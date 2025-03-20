using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.CommissionRates;
using Service.RequestAndResponse.Response.CommissionRate;

namespace GreenRoam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommissionRateController : ControllerBase
    {
        private readonly ICommissionRateService _commissionService;
        public CommissionRateController(ICommissionRateService commissionService)
        {
            _commissionService = commissionService;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllCommissionRate>>>> GetAllCommissionRates()
        {
            var result = await _commissionService.GetAllCommissionRates();
            return Ok(result);
        }

        //[HttpGet]
        //[Route("GetByHomeStay/{homeStayID}")]
        //public async Task<ActionResult<BaseResponse<GetAllCommissionRate>>> GetCommissionRateByHomeStay(int homeStayID)
        //{
        //    var result = await _commissionService.GetCommissionRateByHomeStay(homeStayID);
        //    return Ok(result);
        //}

        [HttpPost]
        [Route("Create")]
        public async Task<ActionResult<BaseResponse<CreateCommissionRateRequest>>> CreateCommissionRate([FromBody] CreateCommissionRateRequest request)
        {
            var result = await _commissionService.CreateCommmisionRate(request);
            return StatusCode((int)result.StatusCode, result);
        }

        //[HttpPut]
        //[Route("Update")]
        //public async Task<ActionResult<BaseResponse<UpdateCommissionRateRequest>>> UpdateCommissionRate([FromBody] UpdateCommissionRateRequest request)
        //{
        //    var result = await _commissionService.UpdateCommmisionRate(request);
        //    return StatusCode((int)result.StatusCode, result);
        //}
    }
}
