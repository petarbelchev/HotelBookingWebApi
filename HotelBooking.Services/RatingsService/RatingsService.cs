using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelBooking.Data.Contracts;
using HotelBooking.Data.Entities;
using HotelBooking.Data.Repositories;
using HotelBooking.Services.RatingsService.Models;
using HotelBooking.Services.SharedModels;
using Microsoft.EntityFrameworkCore;
using static HotelBooking.Common.Constants.ExceptionMessages;

namespace HotelBooking.Services.RatingsService;

public class RatingsService : IRatingsService
{
    private readonly IRepository<Comment> commentsRepo;
    private readonly IRepository<Reply> repliesRepo;
    private readonly IRepository<Hotel> hotelsRepo;
    private readonly IMapper mapper;

    public RatingsService(
        IRepository<Comment> commentsRepo,
        IRepository<Reply> repliesRepo,
        IRepository<Hotel> hotelsRepo,
        IMapper mapper)
    {
        this.commentsRepo = commentsRepo;
        this.repliesRepo = repliesRepo;
        this.hotelsRepo = hotelsRepo;
        this.mapper = mapper;
    }

    public async Task<AvRatingOutputModel> RateComment(
        int commentId,
        int userId,
        CreateRatingInputModel inputModel)
        => await Rate(commentsRepo, commentId, userId, inputModel);

    public async Task<AvRatingOutputModel> RateHotel(
        int hotelId,
        int userId,
        CreateRatingInputModel inputModel)
        => await Rate(hotelsRepo, hotelId, userId, inputModel);

    public async Task<AvRatingOutputModel> RateReply(
        int replyId,
        int userId,
        CreateRatingInputModel inputModel)
        => await Rate(repliesRepo, replyId, userId, inputModel);

    private async Task<AvRatingOutputModel> Rate<T>(
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

        AvRatingOutputModel outputModel = await repository
            .AllAsNoTracking()
            .Where(entity => entity.Id == entityId)
            .Select(hotel => hotel.Ratings)
            .ProjectTo<AvRatingOutputModel>(mapper.ConfigurationProvider, new { userId })
            .FirstAsync();

        return outputModel;
    }
}
