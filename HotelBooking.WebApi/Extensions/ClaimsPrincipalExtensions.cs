using System.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection;

public static class ClaimsPrincipalExtensions
{
    public static int Id(this ClaimsPrincipal user)
        => int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));

    public static int? IdOrNull(this ClaimsPrincipal user)
    {
        bool isAuthenticated = user?.Identity?.IsAuthenticated ??
            throw new ArgumentNullException();

        return isAuthenticated
            ? int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier))
            : null;
    }
}
