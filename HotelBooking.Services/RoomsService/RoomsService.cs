using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelBooking.Data;
using HotelBooking.Data.Entities;
using HotelBooking.Services.RoomsService.Models;
using HotelBooking.Services.SharedModels;
using Microsoft.EntityFrameworkCore;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.RoomsService;

public class RoomsService : IRoomsService
{
	private readonly ApplicationDbContext dbContext;
	private readonly IMapper mapper;

	public RoomsService(ApplicationDbContext dbContext,
						IMapper mapper)
	{
		this.dbContext = dbContext;
		this.mapper = mapper;
	}

	public async Task<CreateGetUpdateRoomOutputModel> CreateRoom(int hotelId, int userId, CreateUpdateRoomInputModel inputModel)
	{
		Hotel? hotel = await dbContext.Hotels
			.Where(hotel => hotel.Id == hotelId && !hotel.IsDeleted)
			.FirstOrDefaultAsync() ??
				throw new KeyNotFoundException(string.Format(NonexistentHotel, hotelId));

		if (hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		Room room = mapper.Map<Room>(inputModel);
		hotel.Rooms.Add(room);
		await dbContext.SaveChangesAsync();

		return mapper.Map<CreateGetUpdateRoomOutputModel>(room);
	}

	public async Task DeleteRoom(int id, int userId)
	{
		Room room = await dbContext.Rooms
			.Where(room => room.Id == id && !room.IsDeleted)
			.Include(room => room.Hotel)
			.FirstOrDefaultAsync() ??
				throw new KeyNotFoundException(string.Format(NonexistentRoom, id));

		if (room.Hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		room.IsDeleted = true;
		await dbContext.SaveChangesAsync();
	}

	public async Task<IEnumerable<GetAvailableHotelRoomsOutputModel>> GetAvailableRooms(DateTime checkIn, DateTime checkOut)
	{
		return await dbContext.Hotels
			.Where(hotel => !hotel.IsDeleted)
			.ProjectTo<GetAvailableHotelRoomsOutputModel>(mapper.ConfigurationProvider, new { checkIn, checkOut })
			.ToArrayAsync();
	}

	public async Task<CreateGetUpdateRoomOutputModel?> GetRoom(int id)
	{
		return await dbContext.Rooms
			.Where(room => room.Id == id && !room.IsDeleted)
			.ProjectTo<CreateGetUpdateRoomOutputModel>(mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();
	}

	public async Task<CreateGetUpdateRoomOutputModel> UpdateRoom(int id, int userId, CreateUpdateRoomInputModel inputModel)
	{
		Room room = await dbContext.Rooms
			.Where(room => room.Id == id && !room.IsDeleted)
			.Include(room => room.Hotel)
			.FirstOrDefaultAsync() ??
				throw new KeyNotFoundException(string.Format(NonexistentRoom, id));

		if (room.Hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		mapper.Map(inputModel, room);
		await dbContext.SaveChangesAsync();

		return mapper.Map<CreateGetUpdateRoomOutputModel>(room);
	}
}
