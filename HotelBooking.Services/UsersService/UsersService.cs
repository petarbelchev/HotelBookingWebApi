using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelBooking.Data;
using HotelBooking.Data.Entities;
using HotelBooking.Services.UsersService.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.UsersService;

public class UsersService : IUsersService
{
	private const int hashLength = 64;
	private const int hashIterations = 350000;
	private readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

	private readonly ApplicationDbContext dbContext;
	private readonly IConfiguration configuration;
	private readonly IMapper mapper;

	public UsersService(ApplicationDbContext dbContext,
						IConfiguration configuration,
						IMapper mapper)
	{
		this.dbContext = dbContext;
		this.configuration = configuration;
		this.mapper = mapper;
	}

	public async Task CreateUser(CreateUserInputModel inputModel)
	{
		ApplicationUser? user = await dbContext.Users
			.FirstOrDefaultAsync(user => 
				user.Email == inputModel.Email || 
				user.PhoneNumber == inputModel.PhoneNumber);

		if (user != null)
		{
			string message = user.Email == inputModel.Email
				? string.Format(ExistingEmailAddress, inputModel.Email)
				: string.Format(ExistingPhoneNumber, inputModel.PhoneNumber);

			throw new ArgumentException(message);
		}

		user = mapper.Map<ApplicationUser>(inputModel);
		user.PasswordHash = HashPassword(inputModel.Password, out byte[] salt);
		user.Salt = Convert.ToHexString(salt);

		await dbContext.Users.AddAsync(user);
		await dbContext.SaveChangesAsync();
	}

	public async Task DeleteUser(int id)
	{
		await dbContext.Database.ExecuteSqlRawAsync(
			"EXEC dbo.usp_MarkUserHotelsAndRoomsAsDeleted @userId",
			new SqlParameter("@userId", id));
	}

	public async Task<UserDetailsOutputModel?> GetUser(int id)
	{
		return await dbContext.Users
			.Where(user => user.Id == id)
			.ProjectTo<UserDetailsOutputModel>(mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();
	}

	public async Task<TokenOutputModel?> LoginUser(LoginUserInputModel inputModel)
	{
		ApplicationUser? user = await dbContext.Users
			.AsNoTracking()
			.FirstOrDefaultAsync(user => user.Email == inputModel.Email);

		if (user != null && VerifyPassword(inputModel.Password, user.PasswordHash, user.Salt))
		{
			var outputModel = mapper.Map<TokenOutputModel>(user);
			outputModel.Token = GenerateJsonWebToken(user);

			return outputModel;
		}

		return null;
	}

	public async Task UpdateUser(int id, UpdateUserModel inputModel)
	{
		ApplicationUser user = await dbContext.Users.FirstAsync(user => user.Id == id);
		mapper.Map(inputModel, user);
		await dbContext.SaveChangesAsync();
	}

	private string GenerateJsonWebToken(ApplicationUser user)
	{
		var authClaims = new Claim[]
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.Email),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
		};

		string jwtSecret = configuration["JWT:Secret"];
		byte[] jwtSecretBytes = Encoding.UTF8.GetBytes(jwtSecret);
		var authSigningKey = new SymmetricSecurityKey(jwtSecretBytes);

		var token = new JwtSecurityToken(
			issuer: configuration["JWT:ValidIssuer"],
			audience: configuration["JWT:ValidAudience"],
			expires: DateTime.Now.AddHours(3),
			claims: authClaims,
			signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha512));
		
		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	private string HashPassword(string password, out byte[] hashSalt)
	{
		hashSalt = RandomNumberGenerator.GetBytes(hashLength);

		byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
			Encoding.UTF8.GetBytes(password),
			hashSalt,
			hashIterations,
			hashAlgorithm,
			hashLength);

		return Convert.ToHexString(hash);
	}

	private bool VerifyPassword(string password, string hash, string salt)
	{
		byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
			Encoding.UTF8.GetBytes(password),
			Convert.FromHexString(salt),
			hashIterations,
			hashAlgorithm,
			hashLength);

		byte[] actualHash = Convert.FromHexString(hash);

		return CryptographicOperations.FixedTimeEquals(hashToCompare, actualHash);
	}
}
