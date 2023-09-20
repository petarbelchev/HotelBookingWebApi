using HotelBooking.Services.CommentsService;
using HotelBooking.Services.CommentsService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.WebApi.Controllers;

[Authorize]
[Route("api/comments")]
[ApiController]
public class CommentsController : ControllerBase
{
	private readonly ICommentsService commentsService;

	public CommentsController(ICommentsService commentsService)
		=> this.commentsService = commentsService;

	// GET: api/comments
	[HttpGet("~/api/hotels/{hotelId}/comments")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<GetCommentOutputModel>>> Get(int hotelId)
		=> Ok(await commentsService.GetHotelComments(hotelId));

	// POST api/hotels/5/comments
	[HttpPost("~/api/hotels/{hotelId}/comments")]
	public async Task<ActionResult<GetCommentOutputModel>> Create(int hotelId, CreateCommentInputModel inputModel)
	{
		try
		{
			return Ok(await commentsService.AddComment(hotelId, User.Id(), inputModel));
		}
		catch (KeyNotFoundException)
		{
			return NotFound();
		}
	}

	// DELETE api/comments/5
	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(int id)
	{
		try
		{
			await commentsService.DeleteComment(id, User.Id());
		}
		catch (UnauthorizedAccessException)
		{
			return Unauthorized();
		}
		catch (KeyNotFoundException)
		{
			return NotFound();
		}

		return NoContent();
	}
}
