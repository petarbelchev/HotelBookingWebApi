using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelBooking.Data;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Enum;
using HotelBooking.Services.BookingsService.Models;
using HotelBooking.Services.RoomsService;
using HotelBooking.Services.RoomsService.Models;
using HotelBooking.Services.UsersService.Models;
using Microsoft.EntityFrameworkCore;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.BookingsService;

public class BookingsService : IBookingsService
{
	private readonly ApplicationDbContext dbContext;
	private readonly IRoomsService roomsService;
	private readonly IMapper mapper;

	public BookingsService(ApplicationDbContext dbContext,
						   IRoomsService roomsService,
						   IMapper mapper)
	{
		this.dbContext = dbContext;
		this.roomsService = roomsService;
		this.mapper = mapper;
	}

	public async Task<CreateGetBookingOutputModel> CreateBooking(int roomId,
																 int userId,
																 CreateBookingInputModel inputModel)
	{
		var checkIn = inputModel.CheckInUtc!.Value.Date;
		var checkOut = inputModel.CheckOutUtc!.Value.Date;

		CreateGetUpdateRoomOutputModel? room = await roomsService.GetAvailableRooms(roomId, checkIn, checkOut) ??
			throw new KeyNotFoundException(string.Format(NotAvailableRoom, roomId));

		var booking = new Booking
		{
			CreatedOnUtc = DateTime.UtcNow,
			CheckInUtc = checkIn,
			CheckOutUtc = checkOut,
			CustomerId = userId,
			RoomId = roomId,
			Status = BookingStatus.Completed,
		};

		await dbContext.Bookings.AddAsync(booking);
		await dbContext.SaveChangesAsync();

		var output = mapper.Map<CreateGetBookingOutputModel>(booking);
		output.Room = room;
		output.Customer = await dbContext.Users
			.Where(user => user.Id == userId)
			.ProjectTo<BaseUserInfoOutputModel>(mapper.ConfigurationProvider)
			.FirstAsync();

		return output;
	}

	public async Task CancelBooking(int id, int userId)
	{
		Booking? booking = await dbContext.Bookings.FindAsync(id) ??
			throw new KeyNotFoundException(string.Format(NonexistentEntity, typeof(Booking).Name, id)); // TODO: Use this message template everywhere.

		if (booking.CustomerId != userId)
			throw new UnauthorizedAccessException();

		if (booking.CheckInUtc > DateTime.UtcNow.AddDays(-1))
			throw new ArgumentException(CantCancelOnCheckInOrAfter);

		booking.Status = BookingStatus.Cancelled;
		await dbContext.SaveChangesAsync();
	}

	public async Task<IEnumerable<CreateGetBookingOutputModel>> GetBookings()
	{
		return await dbContext.Bookings
			.ProjectTo<CreateGetBookingOutputModel>(mapper.ConfigurationProvider)
			.ToArrayAsync();
	}

	public async Task<CreateGetBookingOutputModel?> GetBookings(int id, int userId)
	{
		CreateGetBookingOutputModel? booking = await dbContext.Bookings
			.Where(booking => booking.Id == id)
			.ProjectTo<CreateGetBookingOutputModel>(mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();

		if (booking != null && booking.Customer.Id != userId)
			throw new UnauthorizedAccessException();

		return booking;
	}
}
