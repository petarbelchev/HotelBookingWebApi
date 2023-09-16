using AutoMapper;
using HotelBooking.Data;
using HotelBooking.Data.Entities;
using HotelBooking.Services.HotelsService.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.HotelsService;

public class HotelsService : IHotelsService
{
	private readonly ApplicationDbContext dbContext;
	private readonly IMapper mapper;

	public HotelsService(ApplicationDbContext dbContext,
						 IMapper mapper)
	{
		this.dbContext = dbContext;
		this.mapper = mapper;
	}

	public async Task CreateHotel(int userId, CreateHotelInputModel inputModel)
	{
		if (!await dbContext.Cities.AnyAsync(city => city.Id == inputModel.CityId))
			throw new KeyNotFoundException(string.Format(NonexistentCity, inputModel.CityId));

		Hotel hotel = mapper.Map<Hotel>(inputModel);
		hotel.OwnerId = userId;
		await dbContext.Hotels.AddAsync(hotel);
		await dbContext.SaveChangesAsync();
	}

	public async Task DeleteHotel(int id, int userId)
	{
		Hotel? hotel = await dbContext.Hotels.FindAsync(id) ??
			throw new KeyNotFoundException(string.Format(NonexistentHotel, id));

		if (hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		await dbContext.Database.ExecuteSqlRawAsync(
			"EXEC dbo.usp_MarkHotelAndRoomsAsDeleted @hotelId",
			new SqlParameter("@hotelId", id));
	}

	public async Task UpdateHotel(int id, int userId, UpdateHotelModel model)
	{
		Hotel? hotel = await dbContext.Hotels.FindAsync(id) ??
			throw new KeyNotFoundException(string.Format(NonexistentHotel, id));

		if (hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		if (!await dbContext.Cities.AnyAsync(city => city.Id == model.CityId))
			throw new KeyNotFoundException(string.Format(NonexistentCity, model.CityId));

		mapper.Map(model, hotel);
		await dbContext.SaveChangesAsync();
	}
}
