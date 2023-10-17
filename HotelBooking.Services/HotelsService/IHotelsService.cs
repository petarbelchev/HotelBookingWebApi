using HotelBooking.Services.HotelsService.Models;
using HotelBooking.Services.SharedModels;

namespace HotelBooking.Services.HotelsService;

public interface IHotelsService
{
	/// <exception cref="ArgumentException">When a city with the given id doesn't exist.</exception>
	Task<CreateHotelOutputModel> CreateHotel(int userId, CreateHotelInputModel inputModel);

	/// <exception cref="KeyNotFoundException">When a hotel with the given id doesn't exist.</exception>
	/// <exception cref="UnauthorizedAccessException">When the user is Unauthorized.</exception>
	Task DeleteHotels(int id, int userId);

	/// <exception cref="ArgumentException">When a hotel with the given id doesn't exist.</exception>
	Task<FavoriteHotelOutputModel> FavoriteHotel(int hotelId, int userId);
	
	Task<GetHotelWithOwnerInfoOutputModel?> GetHotels(int id, int userId);

	Task<IEnumerable<BaseHotelInfoOutputModel>> GetHotels(int userId);

	/// <exception cref="KeyNotFoundException">When a hotel with the given id doesn't exist.</exception>
	/// <exception cref="ArgumentException">When a city with the given id doesn't exist.</exception>
	/// <exception cref="UnauthorizedAccessException">When the user is Unauthorized.</exception>
	Task<UpdateHotelOutputModel> UpdateHotel(int id, int userId, UpdateHotelInputModel model);
}
