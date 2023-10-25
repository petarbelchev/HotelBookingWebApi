using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Repositories;
using HotelBooking.Services.CommentsService.Models;
using HotelBooking.Services.SharedModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.CommentsService;

public class CommentsService : ICommentsService
{
	private readonly IRepository<Comment> commentsRepo;
	private readonly IRepository<Hotel> hotelsRepo;
	private readonly UserManager<ApplicationUser> userManager;
	private readonly IMapper mapper;

	public CommentsService(
		IRepository<Comment> commentsRepo,
		IRepository<Hotel> hotelsRepo,
		UserManager<ApplicationUser> userManager,
		IMapper mapper)
	{
		this.commentsRepo = commentsRepo;
		this.hotelsRepo = hotelsRepo;
		this.userManager = userManager;
		this.mapper = mapper;
	}

	public async Task<GetCommentOutputModel> AddComment(
		int hotelId,
		int userId,
		CreateCommentInputModel inputModel)
	{
		bool hotelExists = await hotelsRepo
			.AllAsNoTracking()
			.AnyAsync(hotel => hotel.Id == hotelId && !hotel.IsDeleted);

		if (!hotelExists)
		{
			throw new ArgumentException(
				string.Format(NonexistentEntity, nameof(Hotel), hotelId),
				nameof(hotelId));
		}

		Comment comment = new Comment
		{
			Content = inputModel.Content,
			HotelId = hotelId,
			AuthorId = userId,
			CreatedOnUtc = DateTime.UtcNow,
		};

		await commentsRepo.AddAsync(comment);
		await commentsRepo.SaveChangesAsync();

		var outputModel = mapper.Map<GetCommentOutputModel>(comment);
		outputModel.Author = await userManager.Users
			.Where(user => user.Id == userId)
			.ProjectTo<BaseUserInfoOutputModel>(mapper.ConfigurationProvider)
			.FirstAsync();

		return outputModel;
	}

	public async Task DeleteComment(int id, int userId)
	{
		Comment? comment = await commentsRepo.FindAsync(id) ??
			throw new KeyNotFoundException();

		if (comment.AuthorId != userId)
			throw new UnauthorizedAccessException();

		comment.IsDeleted = true;
		await commentsRepo.SaveChangesAsync();

		await commentsRepo.ExecuteSqlRawAsync(
			"EXEC dbo.usp_MarkCommentRepliesAndRatingsAsDeleted @commentId",
			new SqlParameter("@commentId", comment.Id));
	}

	public async Task<IEnumerable<GetCommentOutputModel>> GetHotelComments(int hotelId)
	{
		return await commentsRepo
			.AllAsNoTracking()
			.Where(comment => comment.HotelId == hotelId && !comment.IsDeleted)
			.ProjectTo<GetCommentOutputModel>(mapper.ConfigurationProvider)
			.ToArrayAsync();
	}
}
