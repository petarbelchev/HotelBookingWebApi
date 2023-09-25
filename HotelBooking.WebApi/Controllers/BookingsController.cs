using HotelBooking.Services.BookingsService;
using HotelBooking.Services.BookingsService.Models;
using HotelBooking.Services.SharedModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.WebApi.Controllers;

[Authorize]
[Route("api/bookings")]
[ApiController]
public class BookingsController : ControllerBase
{
	private readonly IBookingsService bookingsService;

	public BookingsController(IBookingsService bookingsService)
		=> this.bookingsService = bookingsService;

	// TODO: Make it accessible for admins only.
	// GET: api/bookings
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CreateGetBookingOutputModel>))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Get()
		=> Ok(await bookingsService.GetBookings());

	// GET api/bookings/5
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateGetBookingOutputModel))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Get(int id)
	{
		CreateGetBookingOutputModel? outputModel;

		try
		{
			outputModel = await bookingsService.GetBookings(id, User.Id());
		}
		catch (UnauthorizedAccessException)
		{
			return Forbid();
		}

		return outputModel != null 
			? Ok(outputModel)
			: NotFound();
	}

	// POST api/rooms/5/bookings
	[HttpPost("~/api/rooms/{roomId}/bookings")]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateGetBookingOutputModel))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Create(int roomId, CreateBookingInputModel inputModel)
	{
		try
		{
			CreateGetBookingOutputModel outputModel =
				await bookingsService.CreateBooking(roomId, User.Id(), inputModel);

			return CreatedAtAction(nameof(Get), new { outputModel.Id }, outputModel);
		}
		catch (KeyNotFoundException e)
		{
			ModelState.AddModelError(nameof(roomId), e.Message);
			return ValidationProblem(ModelState);
		}
	}

	// DELETE api/bookings/5
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Cancel(int id)
	{
		try
		{
			await bookingsService.CancelBooking(id, User.Id());
		}
		catch (UnauthorizedAccessException)
		{
			return Forbid();
		}
		catch (KeyNotFoundException)
		{
			return NotFound();
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError("checkIn", e.Message);
			return ValidationProblem(ModelState);
		}

		return NoContent();
	}
}
