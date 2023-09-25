using HotelBooking.Services.BookingsService.Models;
using HotelBooking.Services.SharedModels;

namespace HotelBooking.Services.BookingsService;

public interface IBookingsService
{
	/// <exception cref="KeyNotFoundException">When a room with the given id doesn't exist.</exception>
	Task<CreateGetBookingOutputModel> CreateBooking(int roomId, int userId, CreateBookingInputModel inputModel);

	/// <exception cref="KeyNotFoundException">When a booking with the given id doesn't exist.</exception>
	/// <exception cref="ArgumentException">When the user tries to cancel on the check-in day or after.</exception>
	/// <exception cref="UnauthorizedAccessException">When the user is Unauthorized.</exception>
	Task CancelBooking(int id, int userId);

	Task<IEnumerable<CreateGetBookingOutputModel>> GetBookings();

	/// <exception cref="UnauthorizedAccessException">When the user is Unauthorized.</exception>
	Task<CreateGetBookingOutputModel?> GetBookings(int id, int userId);
}
