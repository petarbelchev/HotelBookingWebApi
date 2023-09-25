using HotelBooking.Services.BookingsService.Models;
using HotelBooking.Services.CitiesService;
using HotelBooking.Services.CitiesService.Models;
using HotelBooking.Services.SharedModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.WebApi.Controllers;

[Authorize]
[Route("api/cities")]
[ApiController]
public class CitiesController : ControllerBase
{
	private readonly ICitiesService citiesService;

	public CitiesController(ICitiesService citiesService)
		=> this.citiesService = citiesService;

	// GET: api/cities
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetCityOutputModel>))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<IActionResult> Get()
		=> Ok(await citiesService.GetCities());

	// GET api/cities/5
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCityOutputModel))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Get(int id)
	{
		GetCityOutputModel? city = await citiesService.GetCity(id);

		return city != null
			? Ok(city)
			: NotFound();
	}

	// POST api/cities
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GetCityOutputModel))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> Create(CreateUpdateCityInputModel inputModel)
	{
		try
		{
			GetCityOutputModel outputModel = await citiesService.CreateCity(inputModel);
			return CreatedAtAction(nameof(Get), new { id = outputModel.Id }, outputModel);
		}
		catch (UnauthorizedAccessException)
		{
			return Forbid();
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);			
			return ValidationProblem(ModelState);
		}
	}

	// PUT api/cities/5
	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCityOutputModel))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Update(int id, CreateUpdateCityInputModel inputModel)
	{
		try
		{
			return Ok(await citiesService.UpdateCity(id, inputModel));
		}
		catch (UnauthorizedAccessException)
		{
			return this.Forbid();
		}
		catch (KeyNotFoundException)
		{
			return NotFound();
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);
			return ValidationProblem(ModelState);
		}
	}

	// DELETE api/cities/5
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> Delete(int id)
	{
		try
		{
			await citiesService.DeleteCity(id);
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
