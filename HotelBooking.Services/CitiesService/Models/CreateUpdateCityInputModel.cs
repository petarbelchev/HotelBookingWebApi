using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;
using static HotelBooking.Common.Constants.ValidationMessages;

namespace HotelBooking.Services.CitiesService.Models;

public class CreateUpdateCityInputModel
{
    [Required]
    [StringLength(CityNameMaxLength, MinimumLength = CityNameMinLength)]
    [RegularExpression(CityNameRegEx, ErrorMessage = CityNameConstraints)]
    public string Name { get; set; } = null!;
}
