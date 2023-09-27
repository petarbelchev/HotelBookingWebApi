using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Services.SharedModels;

public class UpdateUserModel
{
    [Required]
    [StringLength(FirstNameMaxLength, MinimumLength = FirstNameMinLength)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(LastNameMaxLength, MinimumLength = LastNameMinLength)]
    public string LastName { get; set; } = null!;

    [Required]
    [RegularExpression(@"^0\d{9}$")]
    public string PhoneNumber { get; set; } = null!;
}
