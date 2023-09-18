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
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<BaseHotelInfoOutputModel>>> Get()
		=> Ok(await hotelsService.GetHotels());

	// GET api/hotels/5
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
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
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<GetHotelInfoOutputModel>> Create(CreateHotelInputModel inputModel)
	{
		try
		{
			return Ok(await hotelsService.CreateHotel(User.Id(), inputModel));
		}
		catch (KeyNotFoundException e)
		{
			return NotFound(e.Message);
		}		
	}

	// PUT api/hotels/5
	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<UpdateHotelModel>> Update(int id, UpdateHotelModel model)
	{
		try
		{
			await hotelsService.UpdateHotel(id, User.Id(), model);
		}
		catch (UnauthorizedAccessException)
		{
			return Unauthorized();
		}
		catch (KeyNotFoundException e)
		{
			return NotFound(e.Message);
		}

		return model;
	}

	// DELETE api/hotels/5
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> Delete(int id)
	{
		try
		{
			await hotelsService.DeleteHotel(id, User.Id());
		}
		catch (UnauthorizedAccessException)
		{
			return Unauthorized();
		}
		catch (KeyNotFoundException)
		{
			return NotFound();
		}

		return NoContent();
	}
}
