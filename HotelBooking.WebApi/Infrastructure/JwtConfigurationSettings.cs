namespace HotelBooking.WebApi.Infrastructure;

public class JwtConfigurationSettings
{
	public string ValidAudience { get; set; } = null!;

	public string ValidIssuer { get; set; } = null!;

	public string Secret { get; set; } = null!;
}
