using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelBooking.Data;
using HotelBooking.Data.Entities;
using HotelBooking.Services.RepliesService.Models;
using HotelBooking.Services.UsersService.Models;
using Microsoft.EntityFrameworkCore;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.RepliesService;

public class RepliesService : IRepliesService
{
	private readonly ApplicationDbContext dbContext;
	private readonly IMapper mapper;

	public RepliesService(
		ApplicationDbContext dbContext,
		IMapper mapper)
	{
		this.dbContext = dbContext;
		this.mapper = mapper;
	}

	public async Task<GetReplyOutputModel> AddReply(
		int commentId,
		int userId,
		CreateReplyInputModel inputModel)
	{
		if (!await dbContext.Comments.AnyAsync(comment => comment.Id == commentId && !comment.IsDeleted))
			throw new KeyNotFoundException(string.Format(NonexistentEntity, nameof(Comment), commentId));

		Reply reply = new Reply
		{
			ReplyContent = inputModel.ReplyContent,
			CommentId = commentId,
			AuthorId = userId,
			CreatedOnUtc = DateTime.UtcNow,
		};

		await dbContext.AddAsync(reply);
		await dbContext.SaveChangesAsync();

		var outputModel = mapper.Map<GetReplyOutputModel>(reply);
		outputModel.Author = await dbContext.Users
			.Where(user => user.Id == userId)
			.ProjectTo<BaseUserInfoOutputModel>(mapper.ConfigurationProvider)
			.FirstAsync();

		return outputModel;
	}

	public async Task DeleteReply(int id, int userId)
	{
		Reply? reply = await dbContext.Replies.FindAsync(id) ??
			throw new KeyNotFoundException(string.Format(NonexistentEntity, nameof(Reply), id));

		if (reply.AuthorId != userId)
			throw new UnauthorizedAccessException();

		dbContext.Replies.Remove(reply); // TODO: Remove IsDeleted property from the reply entity.
		await dbContext.SaveChangesAsync();
	}

	public async Task<IEnumerable<GetReplyOutputModel>> GetCommentReplies(int commentId)
	{
		return await dbContext.Replies
			.Where(reply => reply.CommentId == commentId)
			.ProjectTo<GetReplyOutputModel>(mapper.ConfigurationProvider)
			.ToArrayAsync();
	}
}
