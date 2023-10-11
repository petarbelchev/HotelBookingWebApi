using HotelBooking.Services.SharedModels;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Services.RoomsService.Models;

public class GetAvailableRoomsInputModel : CreateBookingInputModel
{
    [Required]
    public int? CityId { get; set; }
}
