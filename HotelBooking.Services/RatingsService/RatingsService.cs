using AutoMapper;
using HotelBooking.Data;
using HotelBooking.Data.Contracts;
using HotelBooking.Data.Entities;
using HotelBooking.Services.RatingsService.Models;
using Microsoft.EntityFrameworkCore;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.RatingsService;

public class RatingsService : IRatingsService
{
	private readonly ApplicationDbContext dbContext;
	private readonly IMapper mapper;

	public RatingsService(ApplicationDbContext dbContext,
						  IMapper mapper)
	{
		this.dbContext = dbContext;
		this.mapper = mapper;
	}

	public async Task<CreateRatingOutputModel> RateComment(
		int commentId,
		int userId,
		CreateRatingInputModel inputModel)
		=> await Rate<Comment>(commentId, userId, inputModel);

	public async Task<CreateRatingOutputModel> RateHotel(
		int hotelId,
		int userId,
		CreateRatingInputModel inputModel)
		=> await Rate<Hotel>(hotelId, userId, inputModel);

	public async Task<CreateRatingOutputModel> RateReply(
		int replyId,
		int userId,
		CreateRatingInputModel inputModel)
		=> await Rate<Reply>(replyId, userId, inputModel);

	private async Task<CreateRatingOutputModel> Rate<T>(
		int entityId,
		int userId,
		CreateRatingInputModel inputModel)
		where T : RatableEntity
	{
		T? entity = await dbContext
			.Set<T>()
			.Where(entity => entity.Id == entityId && !entity.IsDeleted)
			.Include(entity => entity.Ratings
				.Where(rating => rating.OwnerId == userId))
			.FirstOrDefaultAsync() ??
				throw new KeyNotFoundException(string.Format(NonexistentEntity, typeof(T).Name, entityId));

		Rating? rating = entity.Ratings.FirstOrDefault();

		if (rating == null)
		{
			rating = new Rating { OwnerId = userId };
			entity.Ratings.Add(rating);
		}

		rating.Value = inputModel.Value;
		await dbContext.SaveChangesAsync();

		return mapper.Map<CreateRatingOutputModel>(rating);
	}
}
