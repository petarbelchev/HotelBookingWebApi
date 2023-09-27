using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Services.SharedModels;

public class CreateUserInputModel : LoginUserInputModel
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

    [Required]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; } = null!;
}
