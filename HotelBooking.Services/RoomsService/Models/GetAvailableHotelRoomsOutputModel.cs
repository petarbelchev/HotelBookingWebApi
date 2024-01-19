using HotelBooking.Services.SharedModels;

namespace HotelBooking.Services.RoomsService.Models;

public class GetAvailableHotelRoomsOutputModel : BaseHotelInfoOutputModel
{
    public ICollection<CreateGetUpdateRoomOutputModel> AvailableRooms { get; set; }
        = new HashSet<CreateGetUpdateRoomOutputModel>();
}
