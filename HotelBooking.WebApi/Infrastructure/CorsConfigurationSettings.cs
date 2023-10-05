namespace HotelBooking.WebApi.Infrastructure;

public class CorsConfigurationSettings
{
    public string PolicyName { get; set; } = null!;

    public string[] Origins { get; set; } = null!;
}
