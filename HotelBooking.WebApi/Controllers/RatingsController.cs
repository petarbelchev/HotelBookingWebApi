using HotelBooking.Services.RatingsService;
using HotelBooking.Services.RatingsService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.WebApi.Controllers;

[Authorize]
[Route("api/ratings")]
[ApiController]
public class RatingsController : ControllerBase
{
	private readonly IRatingsService ratingsService;

	public RatingsController(IRatingsService ratingsService)
		=> this.ratingsService = ratingsService;

	// PUT api/comments/5/ratings
	[HttpPut("~/api/comments/{commentId}/ratings")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<CreateRatingOutputModel>> RateComment(int commentId, CreateRatingInputModel inputModel)
	{
		try
		{
			return Ok(await ratingsService.RateComment(commentId, User.Id(), inputModel));
		}
		catch (KeyNotFoundException e)
		{
			ModelState.AddModelError(nameof(commentId), e.Message);
			return ValidationProblem(ModelState);
		}
	}

	// PUT api/hotels/5/ratings
	[HttpPut("~/api/hotels/{hotelId}/ratings")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<CreateRatingOutputModel>> RateHotel(int hotelId, CreateRatingInputModel inputModel)
	{
		try
		{
			return Ok(await ratingsService.RateHotel(hotelId, User.Id(), inputModel));
		}
		catch (KeyNotFoundException e)
		{
			ModelState.AddModelError(nameof(hotelId), e.Message);
			return ValidationProblem(ModelState);
		}
	}

	// PUT api/replies/5/ratings
	[HttpPut("~/api/replies/{replyId}/ratings")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<CreateRatingOutputModel>> RateReply(int replyId, CreateRatingInputModel inputModel)
	{
		try
		{
			return Ok(await ratingsService.RateReply(replyId, User.Id(), inputModel));
		}
		catch (KeyNotFoundException e)
		{
			ModelState.AddModelError(nameof(replyId), e.Message);
			return ValidationProblem(ModelState);
		}
	}
}
