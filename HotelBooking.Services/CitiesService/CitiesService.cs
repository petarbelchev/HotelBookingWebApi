using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Repositories;
using HotelBooking.Services.CitiesService.Models;
using HotelBooking.Services.SharedModels;
using Microsoft.EntityFrameworkCore;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.CitiesService;

public class CitiesService : ICitiesService
{
	private readonly IRepository<City> citiesRepo;
	private readonly IMapper mapper;

	public CitiesService(
		IRepository<City> citiesRepo,
		IMapper mapper)
	{
		this.citiesRepo = citiesRepo;
		this.mapper = mapper;
	}

	public async Task<GetCityOutputModel> CreateCity(CreateUpdateCityInputModel inputModel)
	{
		City? city = await citiesRepo
			.All()
			.FirstOrDefaultAsync(city => city.Name == inputModel.Name);

		if (city != null)
		{
			if (!city.IsDeleted)
			{
				throw new ArgumentException(
					string.Format(ExistingCity, inputModel.Name), 
					nameof(inputModel.Name));
			}

			city.IsDeleted = false;
		}
		else
		{
			city = new City { Name = inputModel.Name };
			await citiesRepo.AddAsync(city);
		}

		await citiesRepo.SaveChangesAsync();
		return mapper.Map<GetCityOutputModel>(city);
	}

	public async Task DeleteCity(int id)
	{
		City? city = await citiesRepo
			.All()
			.FirstOrDefaultAsync(city => city.Id == id && !city.IsDeleted) ??
				throw new KeyNotFoundException();

		city.IsDeleted = true;
		await citiesRepo.SaveChangesAsync();
	}

	public async Task<IEnumerable<GetCityOutputModel>> GetCities()
	{
		return await citiesRepo
			.AllAsNoTracking()
			.Where(city => !city.IsDeleted)
			.ProjectTo<GetCityOutputModel>(mapper.ConfigurationProvider)
			.ToArrayAsync();
	}

	public async Task<GetCityOutputModel?> GetCity(int id)
	{
		return await citiesRepo
			.AllAsNoTracking()
			.Where(city => city.Id == id && !city.IsDeleted)
			.ProjectTo<GetCityOutputModel>(mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();
	}

	public async Task<GetCityOutputModel> UpdateCity(
		int id,
		CreateUpdateCityInputModel inputModel)
	{
		City? city = await citiesRepo
			.All()
			.FirstOrDefaultAsync(city => city.Id == id && !city.IsDeleted) ??
				throw new KeyNotFoundException(string.Format(NonexistentEntity, nameof(City), id));

		bool cityExists = await citiesRepo
			.AllAsNoTracking()
			.AnyAsync(city => city.Name == inputModel.Name);

		if (cityExists)
		{
			throw new ArgumentException(
				string.Format(ExistingCity, inputModel.Name), 
				nameof(inputModel.Name));
		}

		city.Name = inputModel.Name;
		await citiesRepo.SaveChangesAsync();

		return mapper.Map<GetCityOutputModel>(city);
	}
}
