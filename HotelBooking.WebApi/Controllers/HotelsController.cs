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
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<IEnumerable<BaseHotelInfoOutputModel>>> Get()
		=> Ok(await hotelsService.GetHotels());

	// GET api/hotels/5
	[HttpGet("{id}")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<GetHotelWithOwnerInfoOutputModel>> Get(int id)
	{
		GetHotelWithOwnerInfoOutputModel? outputModel = await hotelsService.GetHotel(id);

		return outputModel != null
			? Ok(outputModel)
			: NotFound();
	}

	// POST api/hotels
	[HttpPost]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<GetHotelInfoOutputModel>> Create(CreateHotelInputModel inputModel)
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
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<UpdateHotelModel>> Update(int id, UpdateHotelModel model)
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

		return model;
	}

	// DELETE api/hotels/5
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
