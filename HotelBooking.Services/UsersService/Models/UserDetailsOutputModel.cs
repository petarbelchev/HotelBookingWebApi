namespace HotelBooking.Services.UsersService.Models;

public class UserDetailsOutputModel : UserInfoOutputModel
{
	public int Comments { get; set; }
	
	public int FavoriteHotels { get; set; }
	
	public int OwnedHotels { get; set; }

	public int Ratings { get; set; }

	public int Replies { get; set; }

	public int Trips { get; set; }
}
