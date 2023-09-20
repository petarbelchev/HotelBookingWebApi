using HotelBooking.Data;
using HotelBooking.Services.CitiesService;
using HotelBooking.Services.HotelsService;
using HotelBooking.Services.ImagesService;
using HotelBooking.Services.RoomsService;
using HotelBooking.Services.UsersService;
using HotelBooking.WebApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddControllers();
		
		builder.Services.AddDbContext<ApplicationDbContext>(optBuilder
			=> optBuilder.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContextConnection")));

		builder.Services
			.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.SaveToken = true;
				options.RequireHttpsMetadata = false; // TODO: Set to True in Production!
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidAudience = builder.Configuration["JWT:ValidAudience"],
					ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
				};
			});

		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		builder.Services.AddAutoMapper(
			Assembly.GetAssembly(typeof(UsersService)), 
			Assembly.GetAssembly(typeof(UsersController)));

		builder.Services.AddScoped<IUsersService, UsersService>();
		builder.Services.AddScoped<IHotelsService, HotelsService>();
		builder.Services.AddScoped<IRoomsService, RoomsService>();
		builder.Services.AddScoped<ICitiesService, CitiesService>();
		builder.Services.AddScoped<IImagesService, ImagesService>();

		var app = builder.Build();

		app.UseStaticFiles();

		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();

		app.Run();
	}
}