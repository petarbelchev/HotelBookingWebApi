using HotelBooking.Services.ImagesService.Models;

namespace HotelBooking.Services.SharedModels;

public class MainImageOutputModel
{
    public int Id { get; set; }

    public ImageData? ImageData { get; set; } = null!;
}
