using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;
using static HotelBooking.Common.Constants.ValidationMessages;

namespace HotelBooking.Services.HotelsService.Models;

public class UpdateHotelInputModel
{
    [Required]
    [StringLength(
        HotelNameMaxLength,
        MinimumLength = HotelNameMinLength,
        ErrorMessage = InvalidPropertyLength)]
    public string Name { get; set; } = null!;

    [Required]
    [StringLength(
        HotelAddressMaxLength,
        MinimumLength = HotelAddressMinLength,
        ErrorMessage = InvalidPropertyLength)]
    public string Address { get; set; } = null!;

    [Required]
    public int CityId { get; set; }

    [Required]
    [StringLength(
        HotelDescriptionMaxLength,
        MinimumLength = HotelDescriptionMinLength,
        ErrorMessage = InvalidPropertyLength)]
    public string Description { get; set; } = null!;
}
