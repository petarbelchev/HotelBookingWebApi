using HotelBooking.Services.SharedModels;

namespace HotelBooking.Services.RoomsService.Models;

public class CreateGetUpdateRoomOutputModel : CreateUpdateRoomInputModel
{
    public int Id { get; set; }

    public new string RoomType { get; set; } = null!;

    public int? MainImageId { get; set; }
}
