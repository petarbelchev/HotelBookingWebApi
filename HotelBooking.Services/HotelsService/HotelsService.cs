using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelBooking.Data;
using HotelBooking.Data.Entities;
using HotelBooking.Services.HotelsService.Models;
using HotelBooking.Services.SharedModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.HotelsService;

public class HotelsService : IHotelsService
{
	private readonly ApplicationDbContext dbContext;
	private readonly IMapper mapper;

	public HotelsService(
		ApplicationDbContext dbContext,
		IMapper mapper)
	{
		this.dbContext = dbContext;
		this.mapper = mapper;
	}

	public async Task<GetHotelInfoOutputModel> CreateHotel(
		int userId,
		CreateHotelInputModel inputModel)
	{
		GetCityOutputModel? city = await dbContext.Cities
			.Where(city => city.Id == inputModel.CityId && !city.IsDeleted)
			.ProjectTo<GetCityOutputModel>(mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();

		if (city == null)
			throw new KeyNotFoundException(string.Format(NonexistentCity, inputModel.CityId));

		Hotel hotel = mapper.Map<Hotel>(inputModel);
		hotel.OwnerId = userId;
		await dbContext.Hotels.AddAsync(hotel);
		await dbContext.SaveChangesAsync();

		var outputModel = mapper.Map<GetHotelInfoOutputModel>(hotel);
		outputModel.City = city;

		return outputModel;
	}

	public async Task DeleteHotel(int id, int userId)
	{
		Hotel? hotel = await dbContext.Hotels
			.Where(hotel => hotel.Id == id && !hotel.IsDeleted)
			.FirstOrDefaultAsync() ??
				throw new KeyNotFoundException(string.Format(NonexistentHotel, id));

		if (hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		await dbContext.Database.ExecuteSqlRawAsync(
			"EXEC dbo.usp_MarkHotelAndRoomsAsDeleted @hotelId",
			new SqlParameter("@hotelId", id));
	}

	public async Task<FavoriteHotelOutputModel> FavoriteHotel(int hotelId, int userId)
	{
		Hotel hotel = await dbContext.Hotels
			.Where(hotel => hotel.Id == hotelId)
			.Include(hotel => hotel.UsersWhoFavorited.Where(user => user.Id == userId))
			.FirstOrDefaultAsync() ??
				throw new KeyNotFoundException(string.Format(NonexistentEntity, typeof(Hotel).Name, hotelId));

		var output = new FavoriteHotelOutputModel();
		ApplicationUser? user = hotel.UsersWhoFavorited.FirstOrDefault();

		if (user == null)
		{
			user = await dbContext.Users.FindAsync(userId);
			hotel.UsersWhoFavorited.Add(user!);
			output.IsFavorite = true;
		}
		else
		{
			hotel.UsersWhoFavorited.Remove(user);
		}

		await dbContext.SaveChangesAsync();
		return output;
	}

	public async Task<GetHotelWithOwnerInfoOutputModel?> GetHotels(int id, int userId)
	{
		return await dbContext.Hotels
			.Where(hotel => hotel.Id == id && !hotel.IsDeleted)
			.ProjectTo<GetHotelWithOwnerInfoOutputModel>(mapper.ConfigurationProvider, new { userId })
			.FirstOrDefaultAsync();
	}

	public async Task<IEnumerable<BaseHotelInfoOutputModel>> GetHotels(int userId)
	{
		return await dbContext.Hotels
			.Where(hotel => !hotel.IsDeleted)
			.ProjectTo<BaseHotelInfoOutputModel>(mapper.ConfigurationProvider, new { userId })
			.ToArrayAsync();
	}

	public async Task UpdateHotel(
		int id,
		int userId,
		UpdateHotelModel model)
	{
		Hotel? hotel = await dbContext.Hotels
			.Where(hotel => hotel.Id == id && !hotel.IsDeleted)
			.FirstOrDefaultAsync() ??
				throw new KeyNotFoundException();

		if (hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		if (!await dbContext.Cities.AnyAsync(city => city.Id == model.CityId))
			throw new ArgumentException(string.Format(NonexistentCity, model.CityId));

		mapper.Map(model, hotel);
		await dbContext.SaveChangesAsync();
	}
}
