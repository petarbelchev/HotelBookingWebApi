using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Data.Entities;

public class City
{
	[Key]
	public int Id { get; set; }

	[Required]
	[MaxLength(CityNameMaxLength)]
	public string Name { get; set; } = null!;
}
