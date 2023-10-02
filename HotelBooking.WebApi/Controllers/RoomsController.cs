using HotelBooking.Services.RoomsService;
using HotelBooking.Services.RoomsService.Models;
using HotelBooking.Services.SharedModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.WebApi.Controllers;

[Authorize]
[Route("api/rooms")]
[ApiController]
public class RoomsController : ControllerBase
{
	private readonly IRoomsService roomsService;

	public RoomsController(IRoomsService roomsService)
		=> this.roomsService = roomsService;

	// GET: api/rooms?checkInUtc=2023.09.21&checkOutUtc=2023.09.22
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetAvailableHotelRoomsOutputModel>))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Get([FromQuery] CreateBookingInputModel inputModel)
	{
		var rooms = await roomsService.GetAvailableRooms(
			inputModel.CheckInUtc ?? throw new ArgumentNullException(), 
			inputModel.CheckOutUtc ?? throw new ArgumentNullException());

		return Ok(rooms);
	}

	// GET api/rooms/5
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateGetUpdateRoomOutputModel))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Get(int id)
	{
		CreateGetUpdateRoomOutputModel? room = await roomsService.GetRoom(id);

		return room != null
			? Ok(room)
			: NotFound();
	}

	// POST api/hotels/5/rooms
	[HttpPost("~/api/hotels/{hotelId}/rooms")]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateGetUpdateRoomOutputModel))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Create(int hotelId, CreateUpdateRoomInputModel inputModel)
	{
		try
		{
			CreateGetUpdateRoomOutputModel outputModel = 
				await roomsService.CreateRoom(hotelId, User.Id(), inputModel);

			return CreatedAtAction(nameof(Get), new { id = outputModel.Id }, outputModel);
		}
		catch (UnauthorizedAccessException)
		{
			return Forbid();
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);
			return ValidationProblem();
		}
	}

	// PUT api/rooms/5
	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateGetUpdateRoomOutputModel))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Update(int id, CreateUpdateRoomInputModel inputModel)
	{
		try
		{
			return Ok(await roomsService.UpdateRoom(id, User.Id(), inputModel));
		}
		catch (UnauthorizedAccessException)
		{
			return Forbid();
		}
		catch (KeyNotFoundException)
		{
			return NotFound();
		}
	}

	// DELETE api/rooms/5
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> Delete(int id)
	{
		try
		{
			await roomsService.DeleteRoom(id, User.Id());
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
