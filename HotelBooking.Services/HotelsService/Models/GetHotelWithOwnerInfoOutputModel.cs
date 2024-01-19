using HotelBooking.Services.SharedModels;

namespace HotelBooking.Services.HotelsService.Models;

public class GetHotelWithOwnerInfoOutputModel : BaseHotelInfoOutputModel
{
    public string Address { get; set; } = null!;

    public int UsersWhoFavoritedCount { get; set; }

    public int RoomsCount { get; set; }

    public int CommentsCount { get; set; }

    public IEnumerable<int> ImageIds { get; set; } = null!;

    public BaseUserInfoOutputModel Owner { get; set; } = null!;
}
