﻿using BusinessObject.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.IService;
using Service.RequestAndResponse.BaseResponse;
using Service.RequestAndResponse.Request.ImageHomeStayTypes;
using Service.RequestAndResponse.Request.ImageService;
using Service.RequestAndResponse.Response.ImageHomeStayTypes;
using Service.RequestAndResponse.Response.ImageService;
using Service.Service;

namespace GreenRoam.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageHomeStayRentalsController : ControllerBase
    {
        private readonly IImageHomeStayTypesService _imageHomeStayTypesService;
        public ImageHomeStayRentalsController(IImageHomeStayTypesService imageHomeStayTypesService)
        {
            _imageHomeStayTypesService = imageHomeStayTypesService;
        }

        [Authorize(Roles = "Admin, Owner, Staff")]
        [HttpGet]
        [Route("GetAllImageHomeStayType")]
        public async Task<ActionResult<BaseResponse<IEnumerable<GetAllImageHomeStayType>>>> GetAllImageHomeStayTypes()
        {
            var imageType = await _imageHomeStayTypesService.GetAllImageHomeStayTypes();
            return Ok(imageType);
        }

        [HttpGet]
        [Route("GetAllImageHomeStayType/{id}")]
        public async Task<ActionResult<BaseResponse<GetAllImageHomeStayType>>> GetImageHomeStayTypesById(int id)
        {
            if (id == 0 || id == null)
            {
                return BadRequest("Please provide a valid Id.");
            }
            return await _imageHomeStayTypesService.GetImageHomeStayTypesById(id);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpPost]
        [Route("CreateImageHomeStayType")]
        public async Task<ActionResult<BaseResponse<AddImageHomeStayTypesRequest>>> CreateImageHomeStayType([FromBody] AddImageHomeStayTypesRequest imageTypeRequest)
        {
            if (imageTypeRequest == null)
            {
                return BadRequest("Please provide all required information.");
            }
            var imageType = await _imageHomeStayTypesService.CreateImageHomeStayTypes(imageTypeRequest);
            return CreatedAtAction(nameof(GetImageHomeStayTypesById), new { id = imageType.Data.ImageHomeStayRentalsID }, imageType);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpPut]
        [Route("UpdateImageService/{id}")]
        public async Task<ActionResult<BaseResponse<UpdateImageHomeStayTypesRequest>>> UpdateImageService(int id, [FromBody] UpdateImageHomeStayTypesRequest imageTypeRequest)
        {
            if (id <= 0)
            {
                return BadRequest("Please provide a valid Id.");
            }
            return await _imageHomeStayTypesService.UpdateImageHomeStayTypes(id, imageTypeRequest);
        }

        [Authorize(Roles = "Owner, Staff")]
        [HttpDelete]
        [Route("DeleteImageService/{id}")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteImageHomeStayTypes(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Please provide a valid Id.");
            }
            return await _imageHomeStayTypesService.DeleteImageHomeStayTypes(id);
        }

    }
}
