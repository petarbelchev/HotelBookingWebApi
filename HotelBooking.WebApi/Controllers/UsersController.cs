using HotelBooking.Services.UsersService;
using HotelBooking.Services.UsersService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.WebApi.Controllers;

[Authorize]
[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
	private readonly IUsersService usersService;

	public UsersController(IUsersService usersService)
		=> this.usersService = usersService;

	// GET: api/users
	[HttpGet]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public async Task<ActionResult<IEnumerable<GetUserOutputModel>>> Get()
		=> Ok(await usersService.GetUsers());

	// GET api/users/5
	[HttpGet("{id}")]
	[Produces("application/json")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<GetUserOutputModel>> Get(int id)
	{
		GetUserOutputModel? outputModel = await usersService.GetUser(id);

		return outputModel != null
			? Ok(outputModel)
			: NotFound();
	}

	// DELETE api/users/5
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<ActionResult> Delete(int id)
	{
		if (User.Id() != id)
			return Forbid();

		await usersService.DeleteUser(id);

		return NoContent();
	}

	// POST api/users/login
	[AllowAnonymous]
	[HttpPost("login")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<TokenOutputModel>> Login(LoginUserInputModel inputModel)
	{
		TokenOutputModel? outputModel = await usersService.LoginUser(inputModel);

		return outputModel != null
			? Ok(outputModel)
			: BadRequest("Invalid login attempt!");
	}

	// POST api/users
	[AllowAnonymous]
	[HttpPost("register")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult> Register(CreateUserInputModel inputModel)
	{
		try
		{
			await usersService.CreateUser(inputModel);
		}
		catch (ArgumentException e)
		{
			ModelState.AddModelError(e.ParamName!, e.Message);
			return ValidationProblem(ModelState);
		}

		return NoContent();
	}

	// PUT api/users/5
	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<ActionResult<UpdateUserModel>> Update(int id, UpdateUserModel model)
	{
		if (User.Id() != id)
			return Forbid();

		await usersService.UpdateUser(id, model);

		return Ok(model);
	}
}
