using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Services.SharedModels;

public class CreateBookingInputModel
{
    [Required]
    [Display(Name = "Check-In")]
    public DateTime? CheckInLocal { get; set; }

    [Required]
    [Display(Name = "Check-Out")]
    public DateTime? CheckOutLocal { get; set; }
}
