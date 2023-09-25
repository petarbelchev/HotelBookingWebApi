using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelBooking.Data;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Enum;
using HotelBooking.Services.RoomsService.Models;
using HotelBooking.Services.SharedModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
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

	public async Task<CreateGetUpdateRoomOutputModel> CreateRoom(int hotelId, 
																 int userId, 
																 CreateUpdateRoomInputModel inputModel)
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

	public async Task<IEnumerable<GetAvailableHotelRoomsOutputModel>> GetAvailableRooms(DateTime checkIn, 
																						DateTime checkOut)
	{
		return await dbContext.Hotels
			.Where(hotel => !hotel.IsDeleted)
			.ProjectTo<GetAvailableHotelRoomsOutputModel>(
				mapper.ConfigurationProvider, 
				new { isAvailableRoom = IsAvailableRoomExpressionBuilder(checkIn, checkOut) })
			.ToArrayAsync();
	}

	public async Task<CreateGetUpdateRoomOutputModel?> GetAvailableRooms(int roomId,
																		 DateTime checkIn,
																		 DateTime checkOut)
	{
		return await dbContext.Rooms
			.Where(room => room.Id == roomId)
			.Where(IsAvailableRoomExpressionBuilder(checkIn, checkOut))
			.ProjectTo<CreateGetUpdateRoomOutputModel>(mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();
	}

	public async Task<CreateGetUpdateRoomOutputModel?> GetRoom(int id)
	{
		return await dbContext.Rooms
			.Where(room => room.Id == id && !room.IsDeleted)
			.ProjectTo<CreateGetUpdateRoomOutputModel>(mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();
	}

	public async Task<CreateGetUpdateRoomOutputModel> UpdateRoom(int id, 
																 int userId, 
																 CreateUpdateRoomInputModel inputModel)
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

	private static Expression<Func<Room, bool>> IsAvailableRoomExpressionBuilder(DateTime checkIn, 
																				 DateTime checkOut)
	{
		Expression<Func<Room, bool>> expression = room =>
			!room.IsDeleted &&
			!room.Bookings.Any(b =>
				b.Status == BookingStatus.Completed &&
				(
					(b.CheckInUtc <= checkIn && checkIn < b.CheckOutUtc) ||
					(b.CheckInUtc < checkOut && checkOut <= b.CheckOutUtc) ||
					(checkIn <= b.CheckInUtc && b.CheckOutUtc <= checkOut))
				);

		return expression;
	}
}
