using HotelBooking.Services.HotelsService.Models;

namespace HotelBooking.Services.SharedModels;

public class BaseHotelInfoOutputModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public GetCityOutputModel City { get; set; } = null!;

    public string Description { get; set; } = null!;

    public HotelRatingOutputModel Ratings { get; set; } = null!;
}
