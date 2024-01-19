using HotelBooking.Data.Entities;
using HotelBooking.WebApi.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection;

public static class WebApplicationExtensions
{
    public static async Task<WebApplication> AddApplicationRoles(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        if (!await roleManager.RoleExistsAsync(AppRoles.Admin))
            await roleManager.CreateAsync(new IdentityRole<int>(AppRoles.Admin));

        if (!await roleManager.RoleExistsAsync(AppRoles.User))
            await roleManager.CreateAsync(new IdentityRole<int>(AppRoles.User));

        return app;
    }

    public static async Task<WebApplication> AddApplicationAdmin(this WebApplication app)
    {
        string adminEmail = "alfa.admin@admin.com";

        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        ApplicationUser admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                FirstName = "Alfa",
                LastName = "Admin",
                Email = adminEmail,
                UserName = adminEmail,
                PhoneNumber = "0888888888",
                EmailConfirmed = true,
            };

            await userManager.CreateAsync(admin, "Admin123!");
        }

        if (!await userManager.IsInRoleAsync(admin, AppRoles.Admin))
            await userManager.AddToRoleAsync(admin, AppRoles.Admin);

        return app;
    }
}
