using System.ComponentModel.DataAnnotations;
using HotelBooking.Services.SharedModels;

namespace HotelBooking.Services.HotelsService.Models;

public class CreateHotelInputModel : UpdateHotelModel
{
	[Required]
	public ICollection<CreateUpdateRoomInputModel> Rooms { get; set; } = null!;
}
