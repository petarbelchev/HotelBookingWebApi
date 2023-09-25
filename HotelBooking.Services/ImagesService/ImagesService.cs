using HotelBooking.Data;
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
			.Where(image => image.Id == id &&
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
		return await SaveImages<Hotel>(imageFiles, hotel => hotel.Id == hotelId &&
															hotel.OwnerId == userId &&
															!hotel.IsDeleted);
	}

	public async Task<IEnumerable<ImageData>> SaveRoomImages(
		int roomId,
		int userId,
		IFormFileCollection imagesFiles)
	{
		return await SaveImages<Room>(imagesFiles, room => room.Id == roomId &&
														   room.Hotel.OwnerId == userId &&
														   !room.IsDeleted);
	}

	private async Task<IEnumerable<ImageData>> SaveImages<T>(
		IFormFileCollection imageFiles,
		Expression<Func<T, bool>> filterExpression)
		where T : class
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
}
