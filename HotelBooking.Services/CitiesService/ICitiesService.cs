using HotelBooking.Services.CitiesService.Models;
using HotelBooking.Services.SharedModels;

namespace HotelBooking.Services.CitiesService;

public interface ICitiesService
{
	/// <exception cref="ArgumentException">When a city with the given name exists.</exception>
	/// <exception cref="UnauthorizedAccessException">When the user is not an admin.</exception>
	Task<GetCityOutputModel> CreateCity(CreateUpdateCityInputModel inputModel);

	/// <exception cref="KeyNotFoundException">When a city with the given id doesn't exists.</exception>
	/// <exception cref="UnauthorizedAccessException">When the user is not an admin.</exception>
	Task DeleteCity(int id);

	Task<IEnumerable<GetCityOutputModel>> GetCities();

	Task<GetCityOutputModel?> GetCity(int id);

	/// <exception cref="ArgumentException">When a city with the given name exists.</exception>
	/// <exception cref="KeyNotFoundException">When a city with the given id doesn't exists.</exception>
	/// <exception cref="UnauthorizedAccessException">When the user is not an admin.</exception>
	Task<GetCityOutputModel> UpdateCity(int id, CreateUpdateCityInputModel inputModel);
}
