namespace HotelBooking.WebApi.Infrastructure;

public class JwtConfigurationSettings
{
    public string ValidAudience { get; set; } = null!;

    public string ValidIssuer { get; set; } = null!;

    public int ValidYours { get; set; }

    public string Secret { get; set; } = null!;
}
