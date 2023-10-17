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
	
	Task<IEnumerable<GetAvailableHotelRoomsOutputModel>> GetAvailableRooms(GetAvailableRoomsInputModel inputModel);

	Task<CreateGetUpdateRoomOutputModel?> GetAvailableRooms(int roomId, DateTime checkInUtc, DateTime checkOutUtc);

	/// <exception cref="KeyNotFoundException">When a hotel with the given id and owner id doesn't exist.</exception>
	Task<IEnumerable<CreateGetUpdateRoomOutputModel>> GetHotelRooms(int hotelId, int userId);
	
	Task<CreateGetUpdateRoomOutputModel?> GetRoom(int id);

	/// <exception cref="KeyNotFoundException">When a room with the given id doesn't exist.</exception>
	/// <exception cref="UnauthorizedAccessException">When the user is Unauthorized.</exception>
	Task<CreateGetUpdateRoomOutputModel> UpdateRoom(int id, int userId, CreateUpdateRoomInputModel inputModel);
}
