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
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<GetCityOutputModel>>> Get()
		=> Ok(await citiesService.GetCities());

	// GET api/cities/5
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<GetCityOutputModel>> Get(int id)
	{
		GetCityOutputModel? city = await citiesService.GetCity(id);

		return city != null
			? Ok(city)
			: NotFound();
	}

	// POST api/cities
	[HttpPost]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<GetCityOutputModel>> Create(CreateUpdateCityInputModel inputModel)
	{
		try
		{
			return Ok(await citiesService.CreateCity(inputModel));
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);
			return ValidationProblem(ModelState);
		}
	}

	// PUT api/cities/5
	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<GetCityOutputModel>> Update(int id, CreateUpdateCityInputModel inputModel)
	{
		try
		{
			return Ok(await citiesService.UpdateCity(id, inputModel));
		}
		catch (UnauthorizedAccessException)
		{
			return Unauthorized();
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
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult> Delete(int id)
	{
		try
		{
			await citiesService.DeleteCity(id);
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
