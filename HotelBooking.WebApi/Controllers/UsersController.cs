using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelBooking.Data.Entities;
using HotelBooking.Services.SharedModels;
using HotelBooking.Services.UsersService;
using HotelBooking.WebApi.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.WebApi.Controllers;

[Authorize]
[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
	private readonly UserManager<ApplicationUser> userManager;
	private readonly IUsersService usersService;
	private readonly IMapper mapper;
	private readonly IConfiguration configuration;

	public UsersController(
		UserManager<ApplicationUser> userManager,
		IUsersService usersService,
		IMapper mapper,
		IConfiguration configuration)
	{
		this.userManager = userManager;
		this.usersService = usersService;
		this.mapper = mapper;
		this.configuration = configuration;
	}

	// GET: api/users
	[Authorize(Roles = AppRoles.Admin)]
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetUserOutputModel>))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> Get()
	{
		var users = await userManager.Users
			.Where(user => !user.IsDeleted)
			.ProjectTo<GetUserOutputModel>(mapper.ConfigurationProvider)
			.ToArrayAsync();

		return Ok(users);
	}

	// GET api/users/5
	[HttpGet("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserOutputModel))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Get(int id)
	{
		GetUserOutputModel? outputModel = await userManager.Users
			.Where(user => user.Id == id && !user.IsDeleted)
			.ProjectTo<GetUserOutputModel>(mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();

		return outputModel != null
			? Ok(outputModel)
			: NotFound();
	}

	// DELETE api/users/5
	[HttpDelete("{id}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<ActionResult> Delete(int id)
	{
		if (User.Id() != id)
			return Forbid();

		string deletedValue = "deleted" + id;

		ApplicationUser user = await userManager.GetUserAsync(User);
		user.IsDeleted = true;
		user.PhoneNumber = deletedValue;
		user.UserName = deletedValue;
		user.Email = deletedValue;
		user.FirstName = deletedValue;
		user.LastName = deletedValue;
		var result = await userManager.UpdateAsync(user);

		if (result.Succeeded)
			await usersService.DeleteUserInfo(id);

		return NoContent();
	}

	// POST api/users/login
	[AllowAnonymous]
	[HttpPost("login")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TokenOutputModel))]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Login(LoginUserInputModel inputModel)
	{
		var user = await userManager.FindByEmailAsync(inputModel.Email);

		if (user != null && await userManager.CheckPasswordAsync(user, inputModel.Password))
		{
			var outputModel = mapper.Map<TokenOutputModel>(user);
			outputModel.Token = await GenerateJsonWebToken(user);

			return Ok(outputModel);
		}

		ModelState.AddModelError(string.Empty, "Invalid login attempt!");
		return ValidationProblem();
	}

	// POST api/users
	[AllowAnonymous]
	[HttpPost("register")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult> Register(CreateUserInputModel inputModel)
	{
		var user = await userManager.FindByEmailAsync(inputModel.Email);

		if (user != null)
		{
			ModelState.AddModelError(
				nameof(inputModel.Email),
				string.Format(ExistingEmailAddress, inputModel.Email));

			return ValidationProblem();
		}

		user = mapper.Map<ApplicationUser>(inputModel);
		user.UserName = inputModel.Email;
		var result = await userManager.CreateAsync(user, inputModel.Password);

		if (!result.Succeeded)
		{
			foreach (var error in result.Errors)
				ModelState.AddModelError(string.Empty, error.Description);

			return ValidationProblem();
		}

		await userManager.AddToRoleAsync(user, AppRoles.User);

		return NoContent();
	}

	// PUT api/users/5
	[HttpPut("{id}")]
	[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateUserModel))]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	public async Task<IActionResult> Update(int id, UpdateUserModel model)
	{
		if (User.Id() != id)
			return Forbid();

		ApplicationUser user = await userManager.GetUserAsync(User);
		mapper.Map(model, user);
		await userManager.UpdateAsync(user);

		return Ok(model);
	}

	private async Task<string> GenerateJsonWebToken(ApplicationUser user)
	{
		var authClaims = new List<Claim>()
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.Email),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
		};

		IEnumerable<string> userRoles = await userManager.GetRolesAsync(user);

		foreach (string userRole in userRoles)
			authClaims.Add(new Claim(ClaimTypes.Role, userRole));

		var jwtSettings = configuration
			.GetRequiredSection("JWT")
			.Get<JwtConfigurationSettings>();

		byte[] jwtSecretBytes = Encoding.UTF8.GetBytes(jwtSettings.Secret);
		var authSigningKey = new SymmetricSecurityKey(jwtSecretBytes);

		var token = new JwtSecurityToken(
			issuer: jwtSettings.ValidIssuer,
			audience: jwtSettings.ValidAudience,
			expires: DateTime.Now.AddHours(3),
			claims: authClaims,
			signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha512));

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}
