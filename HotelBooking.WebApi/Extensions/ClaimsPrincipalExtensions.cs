using System.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection;

public static class ClaimsPrincipalExtensions
{
	public static int Id(this ClaimsPrincipal user)
		=> int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
}
