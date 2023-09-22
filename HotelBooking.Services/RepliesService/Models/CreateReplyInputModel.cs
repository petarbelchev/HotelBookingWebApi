using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Services.RepliesService.Models;

public class CreateReplyInputModel
{
	[Required]
	[StringLength(ContentMaxLength, MinimumLength = ContentMinLength)]
	public string ReplyContent { get; set; } = null!;
}
