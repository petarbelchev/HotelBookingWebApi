using AutoMapper;
using HotelBooking.Data.Contracts;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Repositories;
using HotelBooking.Services.RatingsService.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.RatingsService;

public class RatingsService : IRatingsService
{
	private readonly IRepository<Rating> ratingsRepo;
	private readonly IRepository<Comment> commentsRepo;
	private readonly IRepository<Reply> repliesRepo;
	private readonly IRepository<Hotel> hotelsRepo;
	private readonly IMapper mapper;

	public RatingsService(
		IRepository<Rating> ratingsRepo,
		IRepository<Comment> commentsRepo,
		IRepository<Reply> repliesRepo,
		IRepository<Hotel> hotelsRepo,
		IMapper mapper)
	{
		this.ratingsRepo = ratingsRepo;
		this.commentsRepo = commentsRepo;
		this.repliesRepo = repliesRepo;
		this.hotelsRepo = hotelsRepo;
		this.mapper = mapper;
	}

	public async Task<CreateRatingOutputModel> RateComment(
		int commentId,
		int userId,
		CreateRatingInputModel inputModel)
		=> await Rate(commentsRepo, commentId, userId, inputModel);

	public async Task<CreateRatingOutputModel> RateHotel(
		int hotelId,
		int userId,
		CreateRatingInputModel inputModel)
		=> await Rate(hotelsRepo, hotelId, userId, inputModel);

	public async Task<CreateRatingOutputModel> RateReply(
		int replyId,
		int userId,
		CreateRatingInputModel inputModel)
		=> await Rate(repliesRepo, replyId, userId, inputModel);

	private async Task<CreateRatingOutputModel> Rate<T>(
		IRepository<T> repository,
		int entityId,
		int userId,
		CreateRatingInputModel inputModel)
		where T : RatableEntity
	{
		T? entity = await repository
			.All()
			.Where(entity => entity.Id == entityId && !entity.IsDeleted)
			.Include(entity => entity.Ratings
				.Where(rating => rating.OwnerId == userId))
			.FirstOrDefaultAsync();

		if (entity == null)
		{
			string entityName = typeof(T).Name.ToLower();

			throw new ArgumentException(
				string.Format(NonexistentEntity, entityName, entityId),
				entityName + "Id");
		}

		Rating? rating = entity.Ratings.FirstOrDefault();

		if (rating == null)
		{
			rating = new Rating { OwnerId = userId };
			entity.Ratings.Add(rating);
		}

		rating.Value = inputModel.Value;
		await repository.SaveChangesAsync();

		return mapper.Map<CreateRatingOutputModel>(rating);
	}
}
