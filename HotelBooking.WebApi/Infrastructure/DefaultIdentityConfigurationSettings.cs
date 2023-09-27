using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.WebApi.Infrastructure;

public class DefaultIdentityConfigurationSettings
{
	public bool RequireConfirmedAccount { get; set; }

	public bool RequireDigit { get; set; }

	public int RequiredLength { get; set; } = PasswordMinLength;

	public bool RequireLowercase { get; set; }

	public bool RequireNonAlphanumeric { get; set; }

	public bool RequireUniqueEmail { get; set; }

	public bool RequireUppercase { get; set; }
}
