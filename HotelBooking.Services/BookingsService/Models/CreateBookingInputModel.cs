using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Services.BookingsService.Models;

public class CreateBookingInputModel
{
	[Required]
	public DateTime? CheckInUtc { get; set; }

	[Required]
	public DateTime? CheckOutUtc { get; set; }
}
