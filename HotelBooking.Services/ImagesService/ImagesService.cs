using HotelBooking.Data.Contracts;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Repositories;
using HotelBooking.Services.ImagesService.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.ImagesService;

public class ImagesService : IImagesService
{
	private readonly IRepository<Image> imagesRepo;
	private readonly IRepository<Hotel> hotelsRepo;
	private readonly IRepository<Room> roomsRepo;
	private readonly string imagesRootPath;

	public ImagesService(
		IRepository<Image> imagesRepo,
		IRepository<Hotel> hotelsRepo,
		IRepository<Room> roomsRepo,
		IWebHostEnvironment webHostEnvironment)
	{
		this.imagesRepo = imagesRepo;
		this.hotelsRepo = hotelsRepo;
		this.roomsRepo = roomsRepo;
		imagesRootPath = Path.Combine(webHostEnvironment.WebRootPath, "images");
	}

	public async Task DeleteImage(int id, int userId)
	{
		var navProps = typeof(Image)
			.GetProperties()
			.Where(p => p.PropertyType.IsAssignableTo(typeof(IHaveImages)));

		var imageQuery = imagesRepo
			.All()
			.Where(image => image.Id == id)
			.AsQueryable();

		foreach (var prop in navProps)
			imageQuery = imageQuery.Include(prop.Name);

		Image imageForDelete = await imageQuery.FirstOrDefaultAsync() ??
			throw new KeyNotFoundException();

		bool userIsUnauthorized = imageForDelete.OwnerId != userId;
		if (userIsUnauthorized)
			throw new UnauthorizedAccessException();

		IHaveImages imageEntity = null!;
		foreach (var prop in navProps)
		{
			if (prop.GetValue(imageForDelete) is IHaveImages entity)
			{
				imageEntity = entity;
				break;
			}
		}

		bool isMainImage = imageEntity.MainImageId == imageForDelete.Id;
		if (isMainImage)
		{
			var predicate = CreateMainImageQueryPredicate(imageEntity);

			var mainImageQuery = imagesRepo
				.AllAsNoTracking()
				.Where(img => img.Id != imageForDelete.Id)
				.Where(predicate)
				.AsQueryable();

			int mainImageId = await mainImageQuery
				.Select(i => i.Id)
				.FirstOrDefaultAsync();

			imageEntity.MainImageId = mainImageId != 0 ? mainImageId : null;
		}

		imagesRepo.Delete(imageForDelete);
		await imagesRepo.SaveChangesAsync();
		File.Delete(Path.Combine(imagesRootPath, imageForDelete.Name));
	}

	public async Task<ImageData?> GetImageData(int imageId)
	{
		string? imageName = await imagesRepo
			.AllAsNoTracking()
			.Where(image => image.Id == imageId)
			.Select(image => image.Name)
			.FirstOrDefaultAsync();

		if (imageName == null)
			return null;

		return new ImageData()
		{
			Data = File.ReadAllBytes(Path.Combine(imagesRootPath, imageName)),
			ContentType = "image/" + imageName.Split('.').Last()
		};
	}

	public async Task<SavedImagesOutputModel> SaveHotelImages(
		int hotelId,
		int userId,
		IFormFileCollection imageFiles)
	{
		Hotel? hotel = await hotelsRepo
			.All()
			.Where(hotel => hotel.Id == hotelId && !hotel.IsDeleted)
			.FirstOrDefaultAsync();

		if (hotel == null)
		{
			throw new ArgumentException(
				string.Format(NonexistentEntity, nameof(Hotel), hotelId),
				nameof(hotelId));
		}

		if (hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		return await SaveImages(hotel, imageFiles, userId);
	}

	public async Task<SavedImagesOutputModel> SaveRoomImages(
		int roomId,
		int userId,
		IFormFileCollection imagesFiles)
	{
		Room? room = await roomsRepo
			.All()
			.Where(room => room.Id == roomId && !room.IsDeleted)
			.Include(room => room.Hotel)
			.FirstOrDefaultAsync();

		if (room == null)
		{
			throw new ArgumentException(
				string.Format(NonexistentEntity, nameof(Room), roomId),
				nameof(roomId));
		}

		if (room.Hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		return await SaveImages(room, imagesFiles, userId);
	}

	public async Task SetHotelMainImage(int imageId, int hotelId, int userId)
	{
		Hotel? hotel = await hotelsRepo
			.All()
			.Where(hotel => hotel.Id == hotelId)
			.Include(hotel => hotel.Images.Where(image => image.Id == imageId))
			.FirstOrDefaultAsync();

		if (hotel != null && hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		await SetMainImage(hotel, hotelId, imageId);
	}

	public async Task SetRoomMainImage(int imageId, int roomId, int userId)
	{
		Room? room = await roomsRepo
			.All()
			.Where(room => room.Id == roomId)
			.Include(room => room.Images.Where(image => image.Id == imageId))
			.Include(room => room.Hotel)
			.FirstOrDefaultAsync();

		if (room != null && room.Hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		await SetMainImage(room, roomId, imageId);
	}

	private static Expression<Func<Image, bool>> CreateMainImageQueryPredicate(IHaveImages imageEntity)
	{
		ParameterExpression parameter = Expression.Parameter(typeof(Image));
		string propertyName = imageEntity.GetType().Name + "Id";
		MemberExpression property = Expression.Property(parameter, propertyName);
		ConstantExpression value = Expression.Constant(imageEntity.Id);
		BinaryExpression equal = Expression.Equal(property, Expression.Convert(value, typeof(int?)));

		return Expression.Lambda<Func<Image, bool>>(equal, parameter);
	}

	private async Task<SavedImagesOutputModel> SaveImages<T>(
		T entity,
		IFormFileCollection imageFiles,
		int userId)
		where T : class, IHaveImages
	{
		var imageEntities = new Image[imageFiles.Count];

		for (int i = 0; i < imageFiles.Count; i++)
		{
			IFormFile currImageFile = imageFiles[i];
			string fileType = currImageFile.ContentType.Split('/')[1];
			string fileName = $"{Guid.NewGuid()}.{fileType}";
			string imagePath = Path.Combine(imagesRootPath, fileName);

			using (var stream = new FileStream(imagePath, FileMode.Create))
			{
				await currImageFile.CopyToAsync(stream);
			}

			var image = new Image { Name = fileName, OwnerId = userId };
			var navProp = image.GetType().GetProperty(typeof(T).Name) ??
				throw new InvalidOperationException(string.Format(NonexistentNavigationProperty, typeof(T).Name));
			navProp.SetValue(image, entity);

			imageEntities[i] = image;
		}

		await imagesRepo.AddRangeAsync(imageEntities);

		if (entity.MainImageId == null)
			entity.MainImage = imageEntities[0];

		await imagesRepo.SaveChangesAsync();

		return new SavedImagesOutputModel { Ids = imageEntities.Select(entity => entity.Id) };
	}

	/// <exception cref="ArgumentException">When an entity with the given id doesn't exists.</exception>
	private async Task SetMainImage<T>(T? entity, int entityId, int imageId)
		where T : IHaveImages
	{
		if (entity == null)
		{
			string typeName = typeof(T).Name.ToLower();

			throw new ArgumentException(
				string.Format(NonexistentEntity, typeName, entityId),
				typeName + "Id");
		}

		if (!entity.Images.Any())
		{
			throw new ArgumentException(
				string.Format(NonexistentEntity, nameof(Image), imageId),
				nameof(imageId));
		}

		entity.MainImageId = entity.Images.First().Id;
		await imagesRepo.SaveChangesAsync();
	}
}
