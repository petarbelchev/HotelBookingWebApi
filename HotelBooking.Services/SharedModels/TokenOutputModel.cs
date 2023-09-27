namespace HotelBooking.Services.SharedModels;

public class TokenOutputModel : BaseUserInfoOutputModel
{
    public string Token { get; set; } = null!;
}
