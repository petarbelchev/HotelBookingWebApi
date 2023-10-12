namespace HotelBooking.Services.SharedModels;

public class GetUserOutputModel : BaseUserInfoOutputModel
{
    public string PhoneNumber { get; set; } = null!;

    public int Comments { get; set; }

    public int FavoriteHotels { get; set; }

    public int OwnedHotels { get; set; }

    public int Ratings { get; set; }

    public int Replies { get; set; }

    public int Trips { get; set; }
}
