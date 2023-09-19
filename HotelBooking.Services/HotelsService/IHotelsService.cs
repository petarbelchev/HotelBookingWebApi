using HotelBooking.Services.HotelsService.Models;
using HotelBooking.Services.SharedModels;

namespace HotelBooking.Services.HotelsService;

public interface IHotelsService
{
	/// <exception cref="KeyNotFoundException">When a city with the given id doesn't exist.</exception>
	Task<GetHotelInfoOutputModel> CreateHotel(int userId, CreateHotelInputModel inputModel);

	/// <exception cref="KeyNotFoundException">When a hotel with the given id doesn't exist.</exception>
	/// <exception cref="UnauthorizedAccessException">When the user is Unauthorized.</exception>
	Task DeleteHotel(int id, int userId);

	Task<GetHotelWithOwnerInfoOutputModel?> GetHotel(int id);

	Task<IEnumerable<BaseHotelInfoOutputModel>> GetHotels();

	/// <exception cref="KeyNotFoundException">When a hotel with the given id doesn't exist.</exception>
	/// <exception cref="ArgumentException">When a city with the given id doesn't exist.</exception>
	/// <exception cref="UnauthorizedAccessException">When the user is Unauthorized.</exception>
	Task UpdateHotel(int id, int userId, UpdateHotelModel model);
}
