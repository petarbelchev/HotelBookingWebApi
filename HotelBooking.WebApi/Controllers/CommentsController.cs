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

    // GET: api/hotels/5/comments
    [AllowAnonymous]
    [HttpGet("~/api/hotels/{hotelId}/comments")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetCommentOutputModel>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(int hotelId)
        => Ok(await commentsService.GetHotelComments(hotelId, User.IdOrNull()));

    // POST api/hotels/5/comments
    [HttpPost("~/api/hotels/{hotelId}/comments")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GetCommentOutputModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create(int hotelId, CreateCommentInputModel inputModel)
    {
        try
        {
            GetCommentOutputModel outputModel = await commentsService.AddComment(hotelId, User.Id(), inputModel);
            return CreatedAtAction(nameof(Get), new { hotelId }, outputModel);
        }
        catch (ArgumentException e)
        {
            ModelState.AddModelError(e.ParamName!, e.Message);
            return ValidationProblem();
        }
    }

    // DELETE api/comments/5
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            await commentsService.DeleteComment(id, User.Id());
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
