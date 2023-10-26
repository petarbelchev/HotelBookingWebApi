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

	// GET: api/rooms?cityId=1&checkInLocal=2023.09.21&checkOutLocal=2023.09.22
	[AllowAnonymous]
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetAvailableHotelRoomsOutputModel>))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetAvailableRooms([FromQuery] GetAvailableRoomsInputModel inputModel)
		=> Ok(await roomsService.GetAvailableRooms(inputModel, User.IdOrNull()));

	// GET api/rooms/5
	[AllowAnonymous]
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateGetUpdateRoomOutputModel))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetRoom(int id)
	{
		CreateGetUpdateRoomOutputModel? room = await roomsService.GetRoom(id);

		return room != null
			? Ok(room)
			: NotFound();
	}

	// GET api/hotels/5/rooms
	[HttpGet("~/api/hotels/{hotelId}/rooms")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CreateGetUpdateRoomOutputModel>))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetHotelRooms(int hotelId)
	{
		try
		{
			return Ok(await roomsService.GetHotelRooms(hotelId, User.Id()));
		}
		catch (KeyNotFoundException)
		{
			return NotFound();
		}
	}

	// POST api/hotels/5/rooms
	[HttpPost("~/api/hotels/{hotelId}/rooms")]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateGetUpdateRoomOutputModel))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Create(int hotelId, CreateUpdateRoomInputModel inputModel)
	{
		try
		{
			CreateGetUpdateRoomOutputModel outputModel =
				await roomsService.CreateRoom(hotelId, User.Id(), inputModel);

			return CreatedAtAction(nameof(GetRoom), new { id = outputModel.Id }, outputModel);
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
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
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
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
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
