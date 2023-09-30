using HotelBooking.Data;
using HotelBooking.Data.Contracts;
using HotelBooking.Data.Entities;
using HotelBooking.Services.ImagesService.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.ImagesService;

public class ImagesService : IImagesService
{
	private readonly ApplicationDbContext dbContext;
	private readonly string imagesRootPath;

	public ImagesService(
		ApplicationDbContext dbContext,
		IWebHostEnvironment webHostEnvironment)
	{
		this.dbContext = dbContext;
		imagesRootPath = Path.Combine(webHostEnvironment.WebRootPath, "images");
	}

	public async Task DeleteImage(int id, int userId)
	{
		Image? image = await dbContext.Images
			.Where(image => 
				image.Id == id &&
				(
					(image.Hotel != null && image.Hotel.OwnerId == userId) ||
					(image.Room != null && image.Room.Hotel.OwnerId == userId))
				)
			.FirstOrDefaultAsync() ??
				throw new KeyNotFoundException(NonexistentImageOrUnauthorizedUser);

		File.Delete(Path.Combine(imagesRootPath, image.Name));
		dbContext.Images.Remove(image);
		await dbContext.SaveChangesAsync();
	}

	public async Task<IEnumerable<ImageData>> GetHotelImagesData(int hotelId)
		=> await GetImageDataModels(image => image.HotelId == hotelId);

	public async Task<ImageData?> GetImageData(int imageId)
	{
		string? imageName = await dbContext.Images
			.Where(image => image.Id == imageId)
			.Select(image => image.Name)
			.FirstOrDefaultAsync();

		return imageName != null
			? GetImageDataModel(imageName)
			: null;
	}

	public async Task<IEnumerable<ImageData>> GetRoomImagesData(int roomId)
		=> await GetImageDataModels(image => image.RoomId == roomId);

	public async Task<IEnumerable<ImageData>> SaveHotelImages(
		int hotelId,
		int userId,
		IFormFileCollection imageFiles)
	{
		return await SaveImages<Hotel>(imageFiles, hotel =>
			hotel.Id == hotelId &&
			hotel.OwnerId == userId &&
			!hotel.IsDeleted);
	}

	public async Task<IEnumerable<ImageData>> SaveRoomImages(
		int roomId,
		int userId,
		IFormFileCollection imagesFiles)
	{
		return await SaveImages<Room>(imagesFiles, room =>
			room.Id == roomId &&
			room.Hotel.OwnerId == userId &&
			!room.IsDeleted);
	}

	public async Task SetHotelMainImage(int imageId, int hotelId, int userId)
	{
		Hotel? hotel = await dbContext.Hotels
			.Where(hotel => hotel.Id == hotelId)
			.Include(hotel => hotel.Images.Where(image => image.Id == imageId))
			.FirstOrDefaultAsync();

		if (hotel != null && hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		await SetMainImage(hotel, hotelId, imageId);
	}

	public async Task SetRoomMainImage(int imageId, int roomId, int userId)
	{
		Room? room = await dbContext.Rooms
			.Where(room => room.Id == roomId)
			.Include(room => room.Images.Where(image => image.Id == imageId))
			.Include(room => room.Hotel)
			.FirstOrDefaultAsync();

		if (room != null && room.Hotel.OwnerId != userId)
			throw new UnauthorizedAccessException();

		await SetMainImage(room, roomId, imageId);
	}

	private async Task<IEnumerable<ImageData>> SaveImages<T>(
		IFormFileCollection imageFiles,
		Expression<Func<T, bool>> filterExpression)
		where T : class, IHaveImages
	{
		T? entity = await dbContext.Set<T>()
			.FirstOrDefaultAsync(filterExpression) ??
				throw new KeyNotFoundException(NonexistentImageOrUnauthorizedUser);

		var imageEntities = new Image[imageFiles.Count];
		var imageDataModels = new ImageData[imageFiles.Count];

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

			var image = new Image { Name = fileName };
			var navProp = image.GetType().GetProperty(typeof(T).Name) ??
				throw new InvalidOperationException(string.Format(NonexistentNavigationProperty, typeof(T).Name));
			navProp.SetValue(image, entity);

			imageEntities[i] = image;
			imageDataModels[i] = GetImageDataModel(fileName);
		}

		await dbContext.Images.AddRangeAsync(imageEntities);
		entity.MainImage ??= imageEntities[0];
		await dbContext.SaveChangesAsync();

		return imageDataModels;
	}

	private async Task<IEnumerable<ImageData>> GetImageDataModels(
		Expression<Func<Image, bool>> filterExpression)
	{
		string[] imageNames = await dbContext.Images
			.Where(filterExpression)
			.Select(image => image.Name)
			.ToArrayAsync();

		var imagesData = new List<ImageData>();

		foreach (var imageName in imageNames)
			imagesData.Add(GetImageDataModel(imageName));

		return imagesData;
	}

	private ImageData GetImageDataModel(string imageName)
	{
		byte[] imageBytes = File.ReadAllBytes(Path.Combine(imagesRootPath, imageName));

		return new ImageData()
		{
			Base64String = Convert.ToBase64String(imageBytes),
			ContentType = "image/" + imageName.Split('.').Last()
		};
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
		await dbContext.SaveChangesAsync();
	}
}
