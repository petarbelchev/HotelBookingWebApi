using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Repositories;
using HotelBooking.Services.RepliesService.Models;
using HotelBooking.Services.SharedModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.RepliesService;

public class RepliesService : IRepliesService
{
	private readonly IRepository<Reply> repliesRepo;
	private readonly IRepository<Comment> commentsRepo;
	private readonly UserManager<ApplicationUser> userManager;
	private readonly IMapper mapper;

	public RepliesService(
		IRepository<Reply> repliesRepo,
		IRepository<Comment> commentsRepo,
		UserManager<ApplicationUser> userManager,
		IMapper mapper)
	{
		this.repliesRepo = repliesRepo;
		this.commentsRepo = commentsRepo;
		this.userManager = userManager;
		this.mapper = mapper;
	}

	public async Task<GetReplyOutputModel> AddReply(
		int commentId,
		int userId,
		CreateReplyInputModel inputModel)
	{
		bool commentExists = await commentsRepo
			.AllAsNoTracking()
			.AnyAsync(comment => comment.Id == commentId && !comment.IsDeleted);

		if (!commentExists)
		{
			throw new ArgumentException(
				string.Format(NonexistentEntity, nameof(Comment), commentId),
				nameof(commentId));
		}

		var reply = new Reply
		{
			Content = inputModel.Content,
			CommentId = commentId,
			AuthorId = userId,
			CreatedOnUtc = DateTime.UtcNow,
		};

		await repliesRepo.AddAsync(reply);
		await repliesRepo.SaveChangesAsync();

		var outputModel = mapper.Map<GetReplyOutputModel>(reply);
		outputModel.Author = await userManager.Users
			.Where(user => user.Id == userId)
			.ProjectTo<BaseUserInfoOutputModel>(mapper.ConfigurationProvider)
			.FirstAsync();

		return outputModel;
	}

	public async Task DeleteReply(int id, int userId)
	{
		Reply? reply = await repliesRepo.FindAsync(id) ??
			throw new KeyNotFoundException();

		if (reply.AuthorId != userId)
			throw new UnauthorizedAccessException();

		reply.IsDeleted = true;
		await repliesRepo.SaveChangesAsync();

		await repliesRepo.ExecuteSqlRawAsync(
			"EXEC dbo.usp_MarkReplyRatingsAsDeleted @replyId",
			new SqlParameter("@replyId", reply.Id));
	}

	public async Task<IEnumerable<GetReplyOutputModel>> GetCommentReplies(int commentId)
	{
		return await repliesRepo
			.AllAsNoTracking()
			.Where(reply => reply.CommentId == commentId)
			.ProjectTo<GetReplyOutputModel>(mapper.ConfigurationProvider)
			.ToArrayAsync();
	}
}
