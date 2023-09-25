using HotelBooking.Services.BookingsService.Models;
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
		=> Ok(await hotelsService.GetHotels());

	// GET api/hotels/5
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetHotelWithOwnerInfoOutputModel))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Get(int id)
	{
		GetHotelWithOwnerInfoOutputModel? outputModel = await hotelsService.GetHotel(id);

		return outputModel != null
			? Ok(outputModel)
			: NotFound();
	}

	// POST api/hotels
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GetHotelInfoOutputModel))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Create(CreateHotelInputModel inputModel)
	{
		try
		{
			GetHotelInfoOutputModel outputModel = await hotelsService.CreateHotel(User.Id(), inputModel);
			return CreatedAtAction(nameof(Get), new { id = outputModel.Id }, outputModel);
		}
		catch (KeyNotFoundException e)
		{
			ModelState.AddModelError(nameof(inputModel.CityId), e.Message);
			return ValidationProblem(ModelState);
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
			ModelState.AddModelError(nameof(model.CityId), e.Message);
			return ValidationProblem(ModelState);
		}

		return Ok(model);
	}

	// DELETE api/hotels/5
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> Delete(int id)
	{
		try
		{
			await hotelsService.DeleteHotel(id, User.Id());
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
