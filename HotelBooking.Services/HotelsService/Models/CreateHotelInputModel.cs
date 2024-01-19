using System.ComponentModel.DataAnnotations;
using HotelBooking.Services.SharedModels;

namespace HotelBooking.Services.HotelsService.Models;

public class CreateHotelInputModel : UpdateHotelInputModel
{
    [Required]
    public ICollection<CreateUpdateRoomInputModel> Rooms { get; set; } = null!;
}
