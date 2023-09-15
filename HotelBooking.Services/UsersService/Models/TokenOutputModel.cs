namespace HotelBooking.Services.UsersService.Models;

public class TokenOutputModel : UserInfoOutputModel
{
	public string Token { get; set; } = null!;
}
