using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;
using static HotelBooking.Common.Constants.ValidationMessages;

namespace HotelBooking.Services.CommentsService.Models;

public class CreateCommentInputModel
{
    [Required]
    [StringLength(
        ContentMaxLength, 
        MinimumLength = ContentMinLength,
        ErrorMessage = InvalidPropertyLength)]
    [Display(Name = "Comment")]
    public string CommentContent { get; set; } = null!;
}
