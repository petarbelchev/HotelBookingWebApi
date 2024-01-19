using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;
using static HotelBooking.Common.Constants.ValidationMessages;

namespace HotelBooking.Services.SharedModels;

public class LoginUserInputModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(
        PasswordMaxLength,
        MinimumLength = PasswordMinLength,
        ErrorMessage = InvalidPropertyLength)]
    public string Password { get; set; } = null!;
}
