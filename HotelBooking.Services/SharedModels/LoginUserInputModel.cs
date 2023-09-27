using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Services.SharedModels;

public class LoginUserInputModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(PasswordMaxLength, MinimumLength = PasswordMinLength)]
    public string Password { get; set; } = null!;
}
