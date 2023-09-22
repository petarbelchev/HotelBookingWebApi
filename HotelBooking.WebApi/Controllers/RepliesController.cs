using HotelBooking.Services.RepliesService;
using HotelBooking.Services.RepliesService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.WebApi.Controllers;

[Authorize]
[Route("api/replies")]
[ApiController]
public class RepliesController : ControllerBase
{
	private readonly IRepliesService repliesService;

	public RepliesController(IRepliesService repliesService)
		=> this.repliesService = repliesService;

	// GET: api/comments/5/replies
	[HttpGet("~/api/comments/{commentId}/replies")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<IEnumerable<GetReplyOutputModel>>> GetCommentReplies(int commentId)
		=> Ok(await repliesService.GetCommentReplies(commentId));

	// POST api/comments/5/replies
	[HttpPost("~/api/comments/{commentId}/replies")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<GetReplyOutputModel>> Create(int commentId, CreateReplyInputModel inputModel)
	{
		try
		{
			GetReplyOutputModel outputModel = await repliesService.AddReply(commentId, User.Id(), inputModel);
			return CreatedAtAction(nameof(GetCommentReplies), new { commentId }, outputModel);
		}
		catch (KeyNotFoundException e)
		{
			ModelState.AddModelError(nameof(commentId), e.Message);
			return ValidationProblem(ModelState);
		}
	}

	// DELETE api/replies/5
	[HttpDelete("{id}")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> Delete(int id)
	{
		try
		{
			await repliesService.DeleteReply(id, User.Id());
		}
		catch (UnauthorizedAccessException)
		{
			return Forbid();
		}
		catch (KeyNotFoundException)
		{
			return NotFound();
		}

		return NoContent();
	}
}
