using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Services.SharedModels;

public class CreateBookingInputModel
{
    [Required]
    [Display(Name = "Check-In")]
    public DateTime? CheckInUtc { get; set; }

    [Required]
    [Display(Name = "Check-Out")]
    public DateTime? CheckOutUtc { get; set; }
}
