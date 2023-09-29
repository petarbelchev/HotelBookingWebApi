using HotelBooking.Data;
using HotelBooking.Data.Entities;
using HotelBooking.Services.BookingsService;
using HotelBooking.Services.CitiesService;
using HotelBooking.Services.CommentsService;
using HotelBooking.Services.HotelsService;
using HotelBooking.Services.ImagesService;
using HotelBooking.Services.RatingsService;
using HotelBooking.Services.RepliesService;
using HotelBooking.Services.RoomsService;
using HotelBooking.Services.UsersService;
using HotelBooking.WebApi.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		services.AddScoped<IHotelsService, HotelsService>();
		services.AddScoped<IRoomsService, RoomsService>();
		services.AddScoped<ICitiesService, CitiesService>();
		services.AddScoped<IImagesService, ImagesService>();
		services.AddScoped<ICommentsService, CommentsService>();
		services.AddScoped<IRatingsService, RatingsService>();
		services.AddScoped<IRepliesService, RepliesService>();
		services.AddScoped<IBookingsService, BookingsService>();
		services.AddScoped<IUsersService, UsersService>();

		return services;
	}

	public static IServiceCollection ConfigureDefaultIdentity(
		this IServiceCollection services,
		DefaultIdentityConfigurationSettings settings)
	{
		services
			.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
			{
				options.SignIn.RequireConfirmedAccount = settings.RequireConfirmedAccount;
				options.User.RequireUniqueEmail = settings.RequireUniqueEmail;
				options.Password.RequireDigit = settings.RequireDigit;
				options.Password.RequiredLength = settings.RequiredLength;
				options.Password.RequireLowercase = settings.RequireLowercase;
				options.Password.RequireNonAlphanumeric = settings.RequireNonAlphanumeric;
				options.Password.RequireUppercase = settings.RequireUppercase;
			})
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

		return services;
	}

	public static IServiceCollection ConfigureJwtAuthentication(
		this IServiceCollection services,
		JwtConfigurationSettings settings,
		IHostEnvironment environment)
	{
		services
			.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.SaveToken = true;
				options.RequireHttpsMetadata = !environment.IsDevelopment();
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidAudience = settings.ValidAudience,
					ValidIssuer = settings.ValidIssuer,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret))
				};
			});

		return services;
	}
}
