using HotelBooking.Services.SharedModels;

namespace HotelBooking.Services.HotelsService.Models;

public class GetHotelWithOwnerInfoOutputModel : GetHotelInfoOutputModel
{
	public BaseUserInfoOutputModel Owner { get; set; } = null!;
}
