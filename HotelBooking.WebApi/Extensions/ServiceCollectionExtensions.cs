using HotelBooking.Data;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Repositories;
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
		services.AddScoped<IRepository<Hotel>, EFRepository<Hotel>>();
		services.AddScoped<IRepository<Room>, EFRepository<Room>>();
		services.AddScoped<IRepository<City>, EFRepository<City>>();
		services.AddScoped<IRepository<Image>, EFRepository<Image>>();
		services.AddScoped<IRepository<Comment>, EFRepository<Comment>>();
		services.AddScoped<IRepository<Rating>, EFRepository<Rating>>();
		services.AddScoped<IRepository<Reply>, EFRepository<Reply>>();
		services.AddScoped<IRepository<Booking>, EFRepository<Booking>>();
		services.AddScoped<IRepository<ApplicationUser>, EFRepository<ApplicationUser>>();

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

	public static IServiceCollection ConfigureCorsPolicies(
		this IServiceCollection services,
		CorsConfigurationSettings settings)
	{
		services.AddCors(options => options.AddPolicy(settings.PolicyName, policy =>
		{
			policy
				.WithOrigins(settings.Origins)
				.AllowAnyHeader()
				.AllowAnyMethod();
		}));

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
