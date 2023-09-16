using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Data.Entities;

public class City : BaseDeletableEntity
{
	[Required]
	[MaxLength(CityNameMaxLength)]
	public string Name { get; set; } = null!;
}
