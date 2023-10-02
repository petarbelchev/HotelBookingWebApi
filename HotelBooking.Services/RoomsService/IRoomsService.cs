using HotelBooking.Services.RoomsService.Models;
using HotelBooking.Services.SharedModels;

namespace HotelBooking.Services.RoomsService;

public interface IRoomsService
{
	/// <exception cref="ArgumentException">When a hotel with the given id doesn't exist or when a room with the number already exists.</exception>
	/// <exception cref="UnauthorizedAccessException">When the user is Unauthorized.</exception>
	Task<CreateGetUpdateRoomOutputModel> CreateRoom(int hotelId, int userId, CreateUpdateRoomInputModel inputModel);

	/// <exception cref="KeyNotFoundException">When a room with the given id doesn't exist.</exception>
	/// <exception cref="UnauthorizedAccessException">When the user is Unauthorized.</exception>
	Task DeleteRoom(int id, int userId);
	
	Task<IEnumerable<GetAvailableHotelRoomsOutputModel>> GetAvailableRooms(DateTime checkIn, DateTime checkOut);

	Task<CreateGetUpdateRoomOutputModel?> GetAvailableRooms(int roomId, DateTime checkIn, DateTime checkOut);

	Task<CreateGetUpdateRoomOutputModel?> GetRoom(int id);

	/// <exception cref="KeyNotFoundException">When a room with the given id doesn't exist.</exception>
	/// <exception cref="UnauthorizedAccessException">When the user is Unauthorized.</exception>
	Task<CreateGetUpdateRoomOutputModel> UpdateRoom(int id, int userId, CreateUpdateRoomInputModel inputModel);
}
