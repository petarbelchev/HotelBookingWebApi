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
}
