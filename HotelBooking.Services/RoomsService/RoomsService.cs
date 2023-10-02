﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Enum;
using HotelBooking.Data.Repositories;
using HotelBooking.Services.ImagesService;
using HotelBooking.Services.RoomsService.Models;
using HotelBooking.Services.SharedModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.RoomsService;

public class RoomsService : IRoomsService
{
	private readonly IRepository<Room> roomsRepo;
	private readonly IRepository<Hotel> hotelsRepo;
	private readonly IImagesService imagesService;
	private readonly IMapper mapper;

	public RoomsService(
		IRepository<Room> roomsRepo,
		IRepository<Hotel> hotelsRepo,
		IImagesService imagesService,
		IMapper mapper)
	{
		this.roomsRepo = roomsRepo;
		this.hotelsRepo = hotelsRepo;
		this.imagesService = imagesService;
		this.mapper = mapper;
	}

	public async Task<CreateGetUpdateRoomOutputModel> CreateRoom(
		int hotelId,
		int userId,
		CreateUpdateRoomInputModel inputModel)
	{
		Hotel? hotel = await hotelsRepo
			.All()
			.Where(hotel => hotel.Id == hotelId && !hotel.IsDeleted)
			.Include(hotel => hotel.Rooms
				.Where(room => room.Number == inputModel.Number))
			.FirstOrDefaultAsync();

		if (hotel == null)
		{
			throw new ArgumentException(
				string.Format(NonexistentEntity, nameof(Hotel), hotelId),
				nameof(hotelId));
		}

		if (hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		Room? room = hotel.Rooms.FirstOrDefault();

		if (room != null)
		{
			throw new ArgumentException(
				string.Format(ExistingRoomNumber, inputModel.Number), 
				nameof(inputModel.Number));
		}

		room = mapper.Map<Room>(inputModel);
		hotel.Rooms.Add(room);
		await hotelsRepo.SaveChangesAsync();

		return mapper.Map<CreateGetUpdateRoomOutputModel>(room);
	}

	public async Task DeleteRoom(int id, int userId)
	{
		Room room = await roomsRepo
			.All()
			.Where(room => room.Id == id && !room.IsDeleted)
			.Include(room => room.Hotel)
			.FirstOrDefaultAsync() ??
				throw new KeyNotFoundException();

		if (room.Hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		room.IsDeleted = true;
		await roomsRepo.SaveChangesAsync();
	}

	public async Task<IEnumerable<GetAvailableHotelRoomsOutputModel>> GetAvailableRooms(
		DateTime checkIn,
		DateTime checkOut)
	{
		var hotelsWithRooms = await hotelsRepo
			.AllAsNoTracking()
			.Where(hotel => !hotel.IsDeleted)
			.ProjectTo<GetAvailableHotelRoomsOutputModel>(
				mapper.ConfigurationProvider,
				new { isAvailableRoom = IsAvailableRoomExpressionBuilder(checkIn, checkOut) })
			.ToArrayAsync();

		foreach (var room in hotelsWithRooms.SelectMany(hotel => hotel.AvailableRooms))
		{
			if (room.MainImage != null)
				room.MainImage.ImageData = await imagesService.GetImageData(room.MainImage.Id);
		}

		return hotelsWithRooms;
	}

	public async Task<CreateGetUpdateRoomOutputModel?> GetAvailableRooms(
		int roomId,
		DateTime checkIn,
		DateTime checkOut)
	{
		var room = await roomsRepo
			.AllAsNoTracking()
			.Where(room => room.Id == roomId)
			.Where(IsAvailableRoomExpressionBuilder(checkIn, checkOut))
			.ProjectTo<CreateGetUpdateRoomOutputModel>(mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();

		if (room?.MainImage != null)
			room.MainImage.ImageData = await imagesService.GetImageData(room.MainImage.Id);

		return room;
	}

	public async Task<CreateGetUpdateRoomOutputModel?> GetRoom(int id)
	{
		var room = await roomsRepo
			.AllAsNoTracking()
			.Where(room => room.Id == id && !room.IsDeleted)
			.ProjectTo<CreateGetUpdateRoomOutputModel>(mapper.ConfigurationProvider)
			.FirstOrDefaultAsync();

		if (room?.MainImage != null)
			room.MainImage.ImageData = await imagesService.GetImageData(room.MainImage.Id);

		return room;
	}

	public async Task<CreateGetUpdateRoomOutputModel> UpdateRoom(
		int id,
		int userId,
		CreateUpdateRoomInputModel inputModel)
	{
		Room room = await roomsRepo
			.All()
			.Where(room => room.Id == id && !room.IsDeleted)
			.Include(room => room.Hotel)
			.FirstOrDefaultAsync() ??
				throw new KeyNotFoundException();

		if (room.Hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		mapper.Map(inputModel, room);
		await roomsRepo.SaveChangesAsync();

		return mapper.Map<CreateGetUpdateRoomOutputModel>(room);
	}

	private static Expression<Func<Room, bool>> IsAvailableRoomExpressionBuilder(
		DateTime checkIn,
		DateTime checkOut)
	{
		Expression<Func<Room, bool>> expression = room =>
			!room.IsDeleted &&
			!room.Bookings.Any(b =>
				b.Status == BookingStatus.Completed &&
				(
					(b.CheckInUtc <= checkIn.Date && checkIn.Date < b.CheckOutUtc) ||
					(b.CheckInUtc < checkOut.Date && checkOut.Date <= b.CheckOutUtc) ||
					(checkIn.Date <= b.CheckInUtc && b.CheckOutUtc <= checkOut.Date))
				);

		return expression;
	}
}
