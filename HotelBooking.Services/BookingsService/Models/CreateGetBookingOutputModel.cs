using HotelBooking.Services.RoomsService.Models;
using HotelBooking.Services.SharedModels;

namespace HotelBooking.Services.BookingsService.Models;

public class CreateGetBookingOutputModel
{
	public int Id { get; set; }

	public DateTime CreatedOnUtc { get; set; }

	public DateTime CheckInUtc { get; set; }

	public DateTime CheckOutUtc { get; set; }

	public BaseUserInfoOutputModel Customer { get; set; } = null!;

    public CreateGetUpdateRoomOutputModel Room { get; set; } = null!;

	public string Status { get; set; } = null!;
}
