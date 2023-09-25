using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Services.SharedModels;

public class CreateBookingInputModel
{
    [Required]
    public DateTime? CheckInUtc { get; set; }

    [Required]
    public DateTime? CheckOutUtc { get; set; }
}
