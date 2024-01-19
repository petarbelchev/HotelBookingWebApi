using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;
using static HotelBooking.Common.Constants.ValidationMessages;

namespace HotelBooking.Services.SharedModels;

public class UpdateUserModel
{
    [Required]
    [StringLength(
        FirstNameMaxLength,
        MinimumLength = FirstNameMinLength,
        ErrorMessage = InvalidPropertyLength)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(
        LastNameMaxLength,
        MinimumLength = LastNameMinLength,
        ErrorMessage = InvalidPropertyLength)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    [Required]
    [RegularExpression(
        PhoneNumberRegex,
        ErrorMessage = PhoneNumberConstraints)]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = null!;
}
