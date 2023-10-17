using HotelBooking.Services.SharedModels;

namespace HotelBooking.Services.HotelsService.Models;

public class UpdateHotelOutputModel
{
	public string Name { get; set; } = null!;

	public string Address { get; set; } = null!;

	public string Description { get; set; } = null!;

	public GetCityOutputModel City { get; set; } = null!;
}
