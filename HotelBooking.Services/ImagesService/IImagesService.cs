using HotelBooking.Services.ImagesService.Models;
using Microsoft.AspNetCore.Http;

namespace HotelBooking.Services.ImagesService;

public interface IImagesService
{
	/// <exception cref="KeyNotFoundException">When a hotel or room with the given id doesn't exists or the user is unauthorized.</exception>
	Task DeleteImage(int id, int userId);

	Task<IEnumerable<ImageData>> GetHotelImagesData(int hotelId);

	Task<ImageData?> GetImageData(int imageId);

	Task<IEnumerable<ImageData>> GetRoomImagesData(int roomId);

	/// <exception cref="KeyNotFoundException">When a hotel with the given id doesn't exists or the user is unauthorized.</exception>
	Task<IEnumerable<ImageData>> SaveHotelImages(int hotelId, int userId, IFormFileCollection imageFiles);

	/// <exception cref="KeyNotFoundException">When a room with the given id doesn't exists or the user is unauthorized.</exception>
	Task<IEnumerable<ImageData>> SaveRoomImages(int roomId, int userId, IFormFileCollection imageFiles);

	/// <exception cref="ArgumentException">When a hotel or image with the given id doesn't exists.</exception>
	/// <exception cref="UnauthorizedAccessException">When the user is unauthorized.</exception>
	Task SetHotelMainImage(int imageId, int hotelId, int userId);

	/// <exception cref="ArgumentException">When a room or image with the given id doesn't exists.</exception>
	/// <exception cref="UnauthorizedAccessException">When the user is unauthorized.</exception>
	Task SetRoomMainImage(int imageId, int roomId, int userId);
}
