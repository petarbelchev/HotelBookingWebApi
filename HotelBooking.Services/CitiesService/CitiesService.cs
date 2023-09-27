using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelBooking.Data;
using HotelBooking.Data.Entities;
using HotelBooking.Services.CitiesService.Models;
using HotelBooking.Services.SharedModels;
using Microsoft.EntityFrameworkCore;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.CitiesService;

public class CitiesService : ICitiesService
{
	private readonly ApplicationDbContext dbContext;
	private readonly IMapper mapper;

	public CitiesService(
		ApplicationDbContext dbContext,
		IMapper mapper)
	{
		this.dbContext = dbContext;
		this.mapper = mapper;
	}

	public async Task<GetCityOutputModel> CreateCity(CreateUpdateCityInputModel inputModel)
	{
		// TODO: Add authentication validation (for admin).

		City? city = await dbContext.Cities.FirstOrDefaultAsync(city => city.Name == inputModel.Name);

		if (city != null)
		{
			if (!city.IsDeleted)
				throw new ArgumentException(string.Format(ExistingCity, inputModel.Name), nameof(inputModel.Name));

			city.IsDeleted = false;
		}
		else
		{
			city = new City { Name = inputModel.Name };
			await dbContext.AddAsync(city);
		}

		await dbContext.SaveChangesAsync();
		return mapper.Map<GetCityOutputModel>(city);
	}

	public async Task DeleteCity(int id)
	{
		City? city = await dbContext.Cities
			.FirstOrDefaultAsync(city => city.Id == id && !city.IsDeleted) ??
				throw new KeyNotFoundException();

		// TODO: Add authentication validation (for admin).

		city.IsDeleted = true;
		await dbContext.SaveChangesAsync();
	}

	public async Task<IEnumerable<GetCityOutputModel>> GetCities()
	{
		return await dbContext.Cities
			.Where(city => !city.IsDeleted)
			.ProjectTo<GetCityOutputModel>(mapper.ConfigurationProvider)
			.ToArrayAsync();
	}

	public async Task<GetCityOutputModel?> GetCity(int id)
	{
		return await dbContext.Cities
			.Where(city => city.Id == id && !city.IsDeleted)
			.ProjectTo<GetCityOutputModel>(mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();
	}

	public async Task<GetCityOutputModel> UpdateCity(
		int id,
		CreateUpdateCityInputModel inputModel)
	{
		City? city = await dbContext.Cities
			.FirstOrDefaultAsync(city => city.Id == id && !city.IsDeleted) ??
				throw new KeyNotFoundException(string.Format(NonexistentEntity, nameof(City), id));

		// TODO: Add authentication validation (for admin).

		if (await dbContext.Cities.AnyAsync(city => city.Name == inputModel.Name))
			throw new ArgumentException(string.Format(ExistingCity, inputModel.Name), nameof(inputModel.Name));

		city.Name = inputModel.Name;
		await dbContext.SaveChangesAsync();

		return mapper.Map<GetCityOutputModel>(city);
	}
}
