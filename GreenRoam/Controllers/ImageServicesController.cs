using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.ImageService;
using Service.RequestAndResponse.Response.ImageService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreenRoam.Controllers
{
    [Route("api/imageservice")]
    [ApiController]
    public class ImageServicesController : ControllerBase
    {
        private readonly IImageServicesService _imageServicesService;

        public ImageServicesController(IImageServicesService imageServicesService)
        {
            _imageServicesService = imageServicesService;
        }

        [HttpGet]
        [Route("GetAllImageServices")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllImageService>>>> GetAllImageServices()
        {
            var imageServices = await _imageServicesService.GetAllImageServices();
            return Ok(imageServices);
        }

        [HttpGet]
        [Route("GetImageService/{id}")]
        public async Task<ActionResult<BaseResponse<GetAllImageService>>> GetImageServiceById(int id)
        {
            if (id == 0 || id == null)
            {
                return BadRequest("Please provide a valid Id.");
            }
            return await _imageServicesService.GetImageServiceById(id);
        }

        [HttpPost]
        [Route("CreateImageService")]
        public async Task<ActionResult<BaseResponse<AddImageServicesRequest>>> CreateImageService([FromBody] AddImageServicesRequest imageServiceRequest)
        {
            if (imageServiceRequest == null)
            {
                return BadRequest("Please provide all required information.");
            }
            var imageService = await _imageServicesService.CreateImageService(imageServiceRequest);
            return CreatedAtAction(nameof(GetImageServiceById), new { id = imageService.Data.ImageServicesID }, imageService);
        }

        [HttpPut]
        [Route("UpdateImageService/{id}")]
        public async Task<ActionResult<BaseResponse<UpdateImageServicesRequest>>> UpdateImageService(int id, [FromBody] UpdateImageServicesRequest imageServiceRequest)
        {
            if (id <= 0)
            {
                return BadRequest("Please provide a valid Id.");
            }
            return await _imageServicesService.UpdateImageService(id, imageServiceRequest);
        }

        [HttpDelete]
        [Route("DeleteImageService/{id}")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteImageService(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Please provide a valid Id.");
            }
            return await _imageServicesService.DeleteImageService(id);
        }
    }
}
