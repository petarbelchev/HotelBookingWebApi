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

	// GET: api/rooms
	[HttpGet]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<IEnumerable<GetAvailableHotelRoomsOutputModel>>> Get(DateTime checkIn, DateTime checkOut)
		=> Ok(await roomsService.GetAvailableRooms(checkIn, checkOut));

	// GET api/rooms/5
	[HttpGet("{id}")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<CreateGetUpdateRoomOutputModel>> Get(int id)
	{
		CreateGetUpdateRoomOutputModel? room = await roomsService.GetRoom(id);

		return room != null
			? Ok(room)
			: NotFound();
	}

	// POST api/hotels/5/rooms
	[HttpPost("~/api/hotels/{hotelId}/rooms")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<CreateGetUpdateRoomOutputModel>> Create(int hotelId, CreateUpdateRoomInputModel inputModel)
	{
		try
		{
			CreateGetUpdateRoomOutputModel outputModel = await roomsService.CreateRoom(hotelId, User.Id(), inputModel);
			return CreatedAtAction(nameof(Get), new { id = outputModel.Id }, outputModel);
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

	// PUT api/rooms/5
	[HttpPut("{id}")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<CreateGetUpdateRoomOutputModel>> Update(int id, CreateUpdateRoomInputModel inputModel)
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
	[Produces("application/json")]
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
