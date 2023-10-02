using HotelBooking.Services.BookingsService;
using HotelBooking.Services.BookingsService.Models;
using HotelBooking.Services.SharedModels;
using HotelBooking.WebApi.Infrastructure;
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

	// GET: api/bookings
	[Authorize(Roles = AppRoles.Admin)]
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CreateGetBookingOutputModel>))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> GetBookings()
		=> Ok(await bookingsService.GetBookings());

	// GET: api/users/5/bookings
	[HttpGet("~/api/users/{customerId}/bookings")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CreateGetBookingOutputModel>))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> GetCustomerBookings(int customerId)
	{
		return User.Id() != customerId && !User.IsInRole(AppRoles.Admin) 
			? Forbid() 
			: Ok(await bookingsService.GetBookings(customerId));
	}

	// GET api/bookings/5
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateGetBookingOutputModel))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetBooking(int id)
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

			return CreatedAtAction(nameof(GetBooking), new { outputModel.Id }, outputModel);
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);
			return ValidationProblem();
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
			ModelState.AddModelError(e.ParamName!, e.Message);
			return ValidationProblem();
		}

		return NoContent();
	}
}
