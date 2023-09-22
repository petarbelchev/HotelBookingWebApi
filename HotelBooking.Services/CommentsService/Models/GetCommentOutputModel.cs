using HotelBooking.Services.SharedModels;
using HotelBooking.Services.UsersService.Models;

namespace HotelBooking.Services.CommentsService.Models;

public class GetCommentOutputModel
{
    public int Id { get; set; }

    public string CommentContent { get; set; } = null!;

    public BaseUserInfoOutputModel Author { get; set; } = null!;

	public AvRatingOutputModel Ratings { get; set; } = null!;

    public DateTime CreatedOnUtc { get; set; }

    public int RepliesCount { get; set; }
}
