using HotelBooking.Data;
using HotelBooking.Services.HotelsService;
using HotelBooking.WebApi.Controllers;
using HotelBooking.WebApi.Filters;
using HotelBooking.WebApi.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

internal class Program
{
	private static async Task Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddControllers(cfg => cfg.Filters.Add<HtmlSanitizeResultFilter>());
		
		builder.Services
			.AddDbContext<ApplicationDbContext>(optBuilder => optBuilder
				.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContextConnection")));

		builder.Services
			.ConfigureDefaultIdentity(builder.Configuration
				.GetSection("DefaultIdentityConfigurationSettings")
				.Get<DefaultIdentityConfigurationSettings>());

		builder.Services
			.ConfigureJwtAuthentication(
				builder.Configuration.GetSection("JWT").Get<JwtConfigurationSettings>(), 
				builder.Environment);

		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		builder.Services.AddAutoMapper(
			Assembly.GetAssembly(typeof(HotelsService)), 
			Assembly.GetAssembly(typeof(UsersController)));

		builder.Services.AddApplicationServices();

		var corsSettings = builder.Configuration
				.GetSection("CORS")
				.Get<CorsConfigurationSettings>();

		builder.Services.ConfigureCorsPolicies(corsSettings);

		var app = builder.Build();

		app.UseStaticFiles();

		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}
		else
		{
			app.UseExceptionHandler("/error");
		}

		app.UseHttpsRedirection();

		app.UseCors(corsSettings.PolicyName);

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();

		await app.AddApplicationRoles();
		await app.AddApplicationAdmin();

		app.Run();
	}
}