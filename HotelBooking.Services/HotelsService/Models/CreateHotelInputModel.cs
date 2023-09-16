using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Services.HotelsService.Models;

public class CreateHotelInputModel : UpdateHotelModel
{
	[Required]
	public ICollection<CreateRoomInputModel> Rooms { get; set; } = null!;
}
