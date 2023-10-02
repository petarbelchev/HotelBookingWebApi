﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Enum;
using HotelBooking.Data.Repositories;
using HotelBooking.Services.BookingsService.Models;
using HotelBooking.Services.RoomsService;
using HotelBooking.Services.RoomsService.Models;
using HotelBooking.Services.SharedModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.BookingsService;

public class BookingsService : IBookingsService
{
	private readonly IRepository<Booking> bookingRepo;
	private readonly UserManager<ApplicationUser> userManager;
	private readonly IRoomsService roomsService;
	private readonly IMapper mapper;

	public BookingsService(
		IRepository<Booking> bookingRepo,
		UserManager<ApplicationUser> userManager,
		IRoomsService roomsService,
		IMapper mapper)
	{
		this.bookingRepo = bookingRepo;
		this.userManager = userManager;
		this.roomsService = roomsService;
		this.mapper = mapper;
	}

	public async Task<CreateGetBookingOutputModel> CreateBooking(
		int roomId,
		int userId,
		CreateBookingInputModel inputModel)
	{
		var checkIn = inputModel.CheckInUtc!.Value.Date;
		var checkOut = inputModel.CheckOutUtc!.Value.Date;

		var room = await roomsService.GetAvailableRooms(roomId, checkIn, checkOut);

		if (room == null)
		{
			throw new ArgumentException(
				string.Format(NotAvailableRoom, roomId),
				nameof(roomId));
		}

		var booking = new Booking
		{
			CreatedOnUtc = DateTime.UtcNow,
			CheckInUtc = checkIn,
			CheckOutUtc = checkOut,
			CustomerId = userId,
			RoomId = roomId,
			Status = BookingStatus.Completed,
		};

		await bookingRepo.AddAsync(booking);
		await bookingRepo.SaveChangesAsync();

		var output = mapper.Map<CreateGetBookingOutputModel>(booking);
		output.Room = room;
		output.Customer = await userManager.Users
			.Where(user => user.Id == userId)
			.ProjectTo<BaseUserInfoOutputModel>(mapper.ConfigurationProvider)
			.FirstAsync();

		return output;
	}

	public async Task CancelBooking(int id, int userId)
	{
		Booking? booking = await bookingRepo.FindAsync(id) ??
			throw new KeyNotFoundException();

		if (booking.CustomerId != userId)
			throw new UnauthorizedAccessException();

		if (booking.CheckInUtc > DateTime.UtcNow.AddDays(-1))
			throw new ArgumentException(CantCancelOnCheckInOrAfter, nameof(booking.CheckInUtc));

		booking.Status = BookingStatus.Cancelled;
		await bookingRepo.SaveChangesAsync();
	}

	public async Task<IEnumerable<CreateGetBookingOutputModel>> GetBookings()
	{
		return await bookingRepo
			.AllAsNoTracking()
			.ProjectTo<CreateGetBookingOutputModel>(mapper.ConfigurationProvider)
			.ToArrayAsync();
	}

	public async Task<CreateGetBookingOutputModel?> GetBookings(int id, int userId)
	{
		CreateGetBookingOutputModel? booking = await bookingRepo
			.AllAsNoTracking()
			.Where(booking => booking.Id == id)
			.ProjectTo<CreateGetBookingOutputModel>(mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();

		if (booking != null && booking.Customer.Id != userId)
			throw new UnauthorizedAccessException();

		return booking;
	}

	public async Task<IEnumerable<CreateGetBookingOutputModel>> GetBookings(int userId)
	{
		return await bookingRepo
			.AllAsNoTracking()
			.Where(booking => booking.CustomerId == userId)
			.ProjectTo<CreateGetBookingOutputModel>(mapper.ConfigurationProvider)
			.ToArrayAsync();
	}
}
