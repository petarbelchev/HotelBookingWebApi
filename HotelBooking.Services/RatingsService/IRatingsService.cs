using HotelBooking.Services.RatingsService.Models;

namespace HotelBooking.Services.RatingsService;

public interface IRatingsService
{
	/// <exception cref="KeyNotFoundException">When a comment with the given id doesn't exist.</exception>
	Task<CreateRatingOutputModel> RateComment(int commentId, int userId, CreateRatingInputModel inputModel);

	/// <exception cref="KeyNotFoundException">When a hotel with the given id doesn't exist.</exception>
	Task<CreateRatingOutputModel> RateHotel(int hotelId, int userId, CreateRatingInputModel inputModel);

	/// <exception cref="KeyNotFoundException">When a reply with the given id doesn't exist.</exception>
	Task<CreateRatingOutputModel> RateReply(int replyId, int userId, CreateRatingInputModel inputModel);
}
