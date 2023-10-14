using HotelBooking.Services.HotelsService;
using HotelBooking.Services.HotelsService.Models;
using HotelBooking.Services.SharedModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.WebApi.Controllers;

[Authorize]
[Route("api/hotels")]
[ApiController]
public class HotelsController : ControllerBase
{
	private readonly IHotelsService hotelsService;

	public HotelsController(IHotelsService hotelsService)
		=> this.hotelsService = hotelsService;

	// GET: api/hotels
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BaseHotelInfoOutputModel>))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Get()
		=> Ok(await hotelsService.GetHotels(User.Id()));

	// GET api/hotels/5
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetHotelWithOwnerInfoOutputModel))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Get(int id)
	{
		GetHotelWithOwnerInfoOutputModel? outputModel = await hotelsService.GetHotels(id, User.Id());

		return outputModel != null
			? Ok(outputModel)
			: NotFound();
	}

	// POST api/hotels
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateHotelOutputModel))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Create(CreateHotelInputModel inputModel)
	{
		try
		{
			CreateHotelOutputModel outputModel = await hotelsService.CreateHotel(User.Id(), inputModel);
			return CreatedAtAction(nameof(Get), new { id = outputModel.Id }, outputModel);
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);
			return ValidationProblem();
		}
	}

	// PUT api/hotels/5
	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateHotelModel))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Update(int id, UpdateHotelModel model)
	{
		try
		{
			await hotelsService.UpdateHotel(id, User.Id(), model);
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

		return Ok(model);
	}

	// PUT api/hotels/{hotelId}/favorites
	[HttpPut("{hotelId}/favorites")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FavoriteHotelOutputModel))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Favorite(int hotelId)
	{
		try
		{
			return Ok(await hotelsService.FavoriteHotel(hotelId, User.Id()));
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);
			return ValidationProblem();
		}
	}

	// DELETE api/hotels/5
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
			await hotelsService.DeleteHotels(id, User.Id());
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
