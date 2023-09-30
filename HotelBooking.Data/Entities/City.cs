using System.ComponentModel.DataAnnotations;
using HotelBooking.Data.Contracts;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Data.Entities;

public class City : BaseSoftDeleteEntity
{
	[Required]
	[MaxLength(CityNameMaxLength)]
	public string Name { get; set; } = null!;
}
