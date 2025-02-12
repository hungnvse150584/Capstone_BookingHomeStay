using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.Report;
using Service.RequestAndResponse.Request.Services;
using Service.RequestAndResponse.Response.Reports;
using Service.RequestAndResponse.Response.Services;

namespace GreenRoam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [HttpGet]
        [Route("GetAllReport")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllReport>>>> GetAllReport()
        {
            var report = await _reportService.GetAllReport();
            return Ok(report);
        }
        [HttpGet]
        [Route("GetReport/{id}")]
        public async Task<ActionResult<BaseResponse<GetAllReport>>> GetAllReportById(int id)
        {
            if (id == 0 || id == null)
            {
                return BadRequest("Please provide a valid Id.");
            }
            return await _reportService.GetReportById(id);
        }

        [HttpPost]
        [Route("CreateReport")]
        public async Task<ActionResult<BaseResponse<CreateReport>>> CreateReport([FromBody] CreateReport createReport)
        {
            if (createReport == null)
            {
                return BadRequest("Please provide all required information.");
            }
            var report = await _reportService.CreateReport(createReport);
            return CreatedAtAction(nameof(GetAllReportById), new { id = report.Data.ReportID }, report);
        }
        [HttpPut]
        [Route("UpdateReport/{id}")]
        public async Task<ActionResult<BaseResponse<UpdateReport>>> UpdateReport(int id, [FromBody] UpdateReport updateReport)
        {
            if (id <= 0)
            {
                return BadRequest("Please provide a valid Id.");
            }
            return await _reportService.UpdateReport(id, updateReport);
        }
        [HttpDelete]
        [Route("DeleteImageService/{id}")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteReport(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Please provide a valid Id.");
            }
            return await _reportService.DeleteReport(id);
        }
        [HttpGet]
        [Route("SearchReport")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllReport>>>> SearchReport([FromQuery] string search, [FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                return BadRequest("PageIndex and PageSize must be greater than 0.");
            }
            var result = await _reportService.SearchReport(search, pageIndex, pageSize);
            return Ok(result);
        }
    }
}
