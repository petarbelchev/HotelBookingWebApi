using HotelBooking.Services.HotelsService;
using HotelBooking.Services.HotelsService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.WebApi.Controllers;

[Authorize]
[Route("api/hotels")]
[ApiController]
public class HotelController : ControllerBase
{
	private readonly IHotelsService hotelsService;

	public HotelController(IHotelsService hotelsService)
		=> this.hotelsService = hotelsService;

	// GET: api/hotels
	//[HttpGet]
	//public IEnumerable<string> Get()
	//{
	//	return new string[] { "value1", "value2" };
	//}

	// GET api/hotels/5
	//[HttpGet("{id}")]
	//public string Get(int id)
	//{
	//	return "value";
	//}

	// POST api/hotels
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> Create(CreateHotelInputModel inputModel)
	{
		try
		{
			await hotelsService.CreateHotel(User.Id(), inputModel);
		}
		catch (KeyNotFoundException e)
		{
			return NotFound(e.Message);
		}

		return Ok();
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
	[ProducesResponseType(StatusCodes.Status200OK)]
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

		return Ok();
	}
}
