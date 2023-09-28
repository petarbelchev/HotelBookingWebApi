using HotelBooking.Services.CitiesService;
using HotelBooking.Services.CitiesService.Models;
using HotelBooking.Services.SharedModels;
using HotelBooking.WebApi.Infrastructure;
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
	[Authorize(Roles = AppRoles.Admin)]
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
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);			
			return ValidationProblem();
		}
	}

	// PUT api/cities/5
	[Authorize(Roles = AppRoles.Admin)]
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
		catch (KeyNotFoundException)
		{
			return NotFound();
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);
			return ValidationProblem();
		}
	}

	// DELETE api/cities/5
	[Authorize(Roles = AppRoles.Admin)]
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
		catch (KeyNotFoundException)
		{
			return NotFound();
		}

		return NoContent();
	}
}
