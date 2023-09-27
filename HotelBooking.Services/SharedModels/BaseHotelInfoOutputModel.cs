namespace HotelBooking.Services.SharedModels;

public class BaseHotelInfoOutputModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public GetCityOutputModel City { get; set; } = null!;

    public string Description { get; set; } = null!;

    public AvRatingOutputModel Ratings { get; set; } = null!;

    public bool IsUserFavoriteHotel { get; set; }
}
