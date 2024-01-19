using HotelBooking.Services.RepliesService.Models;

namespace HotelBooking.Services.RepliesService;

public interface IRepliesService
{
    /// <exception cref="ArgumentException">When a comment with the given id doesn't exist.</exception>
    Task<GetReplyOutputModel> AddReply(int commentId, int userId, CreateReplyInputModel inputModel);

    /// <exception cref="KeyNotFoundException">When a reply with the given id doesn't exist.</exception>
    /// <exception cref="UnauthorizedAccessException">When the user is Unauthorized.</exception>
    Task DeleteReply(int id, int userId);

    Task<IEnumerable<GetReplyOutputModel>> GetCommentReplies(int commentId, int? userId);
}
