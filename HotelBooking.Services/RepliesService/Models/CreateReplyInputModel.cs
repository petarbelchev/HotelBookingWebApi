using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;
using static HotelBooking.Common.Constants.ValidationMessages;

namespace HotelBooking.Services.RepliesService.Models;

public class CreateReplyInputModel
{
    [Required]
    [StringLength(
        ContentMaxLength,
        MinimumLength = ContentMinLength,
        ErrorMessage = InvalidPropertyLength)]
    [Display(Name = "Reply")]
    public string Content { get; set; } = null!;
}
