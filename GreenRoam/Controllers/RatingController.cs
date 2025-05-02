    using Microsoft.AspNetCore.Mvc;
    using Service.IService;
    using Service.RequestAndResponse.BaseResponse;
    using Service.RequestAndResponse.Enums;
    using Service.RequestAndResponse.Request.Rating;
    using Service.RequestAndResponse.Response.Ratings;
    using System.Threading.Tasks;

    namespace GreenRoam.Controllers
    {
        [Route("api/rating")]
        [ApiController]
        public class RatingController : ControllerBase
        {
            private readonly IRatingService _ratingService;

            public RatingController(IRatingService ratingService)
            {
                _ratingService = ratingService;
            }

            [HttpPost]
            [Route("CreateRating")]
            [ProducesResponseType(StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<BaseResponse<CreateRatingResponse>>> CreateRating([FromBody] CreateRatingRequest request)
            {
                try
                {
                    if (request == null)
                    {
                        return BadRequest(new BaseResponse<CreateRatingResponse>(
                            "Request body cannot be null!",
                            StatusCodeEnum.BadRequest_400,
                            null));
                    }

                    if (!ModelState.IsValid)
                    {
                        return BadRequest(new BaseResponse<CreateRatingResponse>(
                            "Invalid request data!",
                            StatusCodeEnum.BadRequest_400,
                            null));
                    }

                    var result = await _ratingService.CreateRatingAsync(request);
                    return StatusCode((int)result.StatusCode, result);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new BaseResponse<CreateRatingResponse>(
                        $"Something went wrong! Error: {ex.Message}",
                        StatusCodeEnum.InternalServerError_500,
                        null));
                }
            }

            [HttpPut]
            [Route("UpdateRating/{ratingId}")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<BaseResponse<CreateRatingResponse>>> UpdateRating(int ratingId, [FromBody] UpdateRatingRequest request)
            {
                if (ratingId <= 0)
                {
                    return BadRequest(new BaseResponse<CreateRatingResponse>(
                        "Invalid Rating ID.",
                        StatusCodeEnum.BadRequest_400,
                        null));
                }

                if (request == null)
                {
                    return BadRequest(new BaseResponse<CreateRatingResponse>(
                        "Request body cannot be null.",
                        StatusCodeEnum.BadRequest_400,
                        null));
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(new BaseResponse<CreateRatingResponse>(
                        "Invalid request data.",
                        StatusCodeEnum.BadRequest_400,
                        null));
                }

                var result = await _ratingService.UpdateRatingAsync(ratingId, request);
                return StatusCode((int)result.StatusCode, result);
            }

            [HttpDelete]
            [Route("DeleteRating/{ratingId}")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<BaseResponse<string>>> DeleteRating(int ratingId)
            {
                if (ratingId <= 0)
                {
                    return BadRequest(new BaseResponse<string>(
                        "Please provide a valid Rating ID.",
                        StatusCodeEnum.BadRequest_400,
                        null));
                }

                var result = await _ratingService.DeleteRatingAsync(ratingId);
                return StatusCode((int)result.StatusCode, result);
            }

            [HttpGet]
            [Route("GetByHomeStay/{homeStayId}")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<BaseResponse<IEnumerable<CreateRatingResponse>>>> GetRatingByHomeStayId(int homeStayId)
            {
                if (homeStayId <= 0)
                {
                    return BadRequest(new BaseResponse<IEnumerable<CreateRatingResponse>>(
                        "Please provide a valid HomeStay ID.",
                        StatusCodeEnum.BadRequest_400,
                        null));
                }

                var result = await _ratingService.GetRatingByHomeStayIdAsync(homeStayId);
                return StatusCode((int)result.StatusCode, result);
            }

            [HttpGet]
            [Route("GetByAccount/{accountId}")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<BaseResponse<IEnumerable<CreateRatingResponse>>>> GetRatingByAccountId(string accountId)
            {
                if (string.IsNullOrEmpty(accountId))
                {
                    return BadRequest(new BaseResponse<IEnumerable<CreateRatingResponse>>(
                        "Please provide a valid Account ID.",
                        StatusCodeEnum.BadRequest_400,
                        null));
                }

                var result = await _ratingService.GetRatingByAccountIdAsync(accountId);
                return StatusCode((int)result.StatusCode, result);
            }

            [HttpGet]
            [Route("GetByUserAndHomeStay")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<BaseResponse<CreateRatingResponse>>> GetRatingByUserIdAndHomeStay(string accountId, int homeStayId)
            {
                if (string.IsNullOrEmpty(accountId) || homeStayId <= 0)
                {
                    return BadRequest(new BaseResponse<CreateRatingResponse>(
                        "Please provide a valid Account ID and HomeStay ID.",
                        StatusCodeEnum.BadRequest_400,
                        null));
                }

                var result = await _ratingService.GetRatingByUserIdAndHomeStayAsync(accountId, homeStayId);
                return StatusCode((int)result.StatusCode, result);
            }

            [HttpGet]
            [Route("GetAverageRating/{homeStayId}")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<BaseResponse<double>>> GetAverageRating(int homeStayId)
            {
                if (homeStayId <= 0)
                {
                    return BadRequest(new BaseResponse<double>(
                        "Please provide a valid HomeStay ID.",
                        StatusCodeEnum.BadRequest_400,
                        0));
                }

                var result = await _ratingService.GetAverageRatingAsync(homeStayId);
                return StatusCode((int)result.StatusCode, result);
            }
        }
    }