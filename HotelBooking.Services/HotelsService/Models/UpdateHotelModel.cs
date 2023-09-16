using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Services.HotelsService.Models;

public class UpdateHotelModel
{
	[Required]
	[StringLength(HotelNameMaxLength, MinimumLength = HotelNameMinLength)]
	public string Name { get; set; } = null!;

	[Required]
	[StringLength(HotelAddressMaxLength, MinimumLength = HotelAddressMinLength)]
	public string Address { get; set; } = null!;

	[Required]
	public int CityId { get; set; }

	[Required]
	[StringLength(HotelDescriptionMaxLength, MinimumLength = HotelDescriptionMinLength)]
	public string Description { get; set; } = null!;
}
