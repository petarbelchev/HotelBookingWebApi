using HotelBooking.Services.CommentsService.Models;

namespace HotelBooking.Services.CommentsService;

public interface ICommentsService
{
	/// <exception cref="ArgumentException">When a hotel with the given id doesn't exist.</exception>
	Task<GetCommentOutputModel> AddComment(int hotelId, int userId, CreateCommentInputModel inputModel);

	/// <exception cref="UnauthorizedAccessException">When the user is Unauthorized.</exception>
	/// <exception cref="KeyNotFoundException">When a comment with the given id doesn't exist.</exception>
	Task DeleteComment(int id, int userId);
	
	Task<IEnumerable<GetCommentOutputModel>> GetHotelComments(int hotelId, int? userId);
}
