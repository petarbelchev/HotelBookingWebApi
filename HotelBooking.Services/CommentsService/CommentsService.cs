using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelBooking.Data;
using HotelBooking.Data.Entities;
using HotelBooking.Services.CommentsService.Models;
using HotelBooking.Services.UsersService.Models;
using Microsoft.EntityFrameworkCore;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.CommentsService;

public class CommentsService : ICommentsService
{
	private readonly ApplicationDbContext dbContext;
	private readonly IMapper mapper;

	public CommentsService(ApplicationDbContext dbContext,
						   IMapper mapper)
	{
		this.dbContext = dbContext;
		this.mapper = mapper;
	}

	public async Task<GetCommentOutputModel> AddComment(int hotelId, int userId, CreateCommentInputModel inputModel)
	{
		if (!await dbContext.Hotels.AnyAsync(hotel => hotel.Id == hotelId && !hotel.IsDeleted))
			throw new KeyNotFoundException(string.Format(NonexistentHotel, hotelId));

		Comment comment = new Comment
		{
			CommentContent = inputModel.CommentContent,
			HotelId = hotelId,
			AuthorId = userId,
			CreatedOnUtc = DateTime.UtcNow,
		};

		await dbContext.AddAsync(comment);
		await dbContext.SaveChangesAsync();

		var outputModel = mapper.Map<GetCommentOutputModel>(comment);
		outputModel.Author = await dbContext.Users
			.Where(user => user.Id == userId)
			.ProjectTo<BaseUserInfoOutputModel>(mapper.ConfigurationProvider)
			.FirstAsync();

		return outputModel;
	}

	public async Task DeleteComment(int id, int userId)
	{
		Comment? comment = await dbContext.Comments.FindAsync(id) ??
			throw new KeyNotFoundException(string.Format(NonexistentComment, id));

		if (comment.AuthorId != userId)
			throw new UnauthorizedAccessException();

		dbContext.Comments.Remove(comment); // TODO: Remove IsDeleted property from the comment entity.
		await dbContext.SaveChangesAsync();
	}

	public async Task<IEnumerable<GetCommentOutputModel>> GetHotelComments(int hotelId)
	{
		return await dbContext.Comments
			.Where(comment => comment.HotelId == hotelId)
			.ProjectTo<GetCommentOutputModel>(mapper.ConfigurationProvider)
			.ToArrayAsync();
	}
}
