using HotelBooking.Services.SharedModels;

namespace HotelBooking.Services.HotelsService.Models;

public class GetHotelInfoOutputModel : BaseHotelInfoOutputModel
{
	public string Address { get; set; } = null!;

	public int UsersWhoFavoritedCount { get; set; }

    public int RoomsCount { get; set; }
}
