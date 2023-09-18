namespace HotelBooking.Services.UsersService.Models;

public class TokenOutputModel : BaseUserInfoOutputModel
{
	public string Token { get; set; } = null!;
}
