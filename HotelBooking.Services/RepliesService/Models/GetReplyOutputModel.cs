using HotelBooking.Services.SharedModels;

namespace HotelBooking.Services.RepliesService.Models;

public class GetReplyOutputModel
{
    public int Id { get; set; }

    public string Content { get; set; } = null!;

    public BaseUserInfoOutputModel Author { get; set; } = null!;

    public AvRatingOutputModel Ratings { get; set; } = null!;

    public DateTime CreatedOnLocal { get; set; }
}
