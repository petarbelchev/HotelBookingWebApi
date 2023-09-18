using HotelBooking.Services.UsersService.Models;

namespace HotelBooking.Services.HotelsService.Models;

public class GetHotelWithOwnerInfoOutputModel : GetHotelInfoOutputModel
{
	public BaseUserInfoOutputModel Owner { get; set; } = null!;
}
