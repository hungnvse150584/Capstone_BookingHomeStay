using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Owner, Staff")]
        [HttpGet]
        [Route("GetAllImageServices")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllImageService>>>> GetAllImageServices()
        {
            var imageServices = await _imageServicesService.GetAllImageServices();
            return Ok(imageServices);
        }

        [Authorize(Roles = "Owner, Staff, Customer")]
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

        [Authorize(Roles = "Owner, Staff")]
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

        [Authorize(Roles = "Owner, Staff")]
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

        [Authorize(Roles = "Owner, Staff")]
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
