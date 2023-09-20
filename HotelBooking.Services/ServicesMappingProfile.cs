using AutoMapper;
using HotelBooking.Data.Entities;
using HotelBooking.Services.CommentsService.Models;
using HotelBooking.Services.HotelsService.Models;
using HotelBooking.Services.RoomsService.Models;
using HotelBooking.Services.SharedModels;
using HotelBooking.Services.UsersService.Models;

namespace HotelBooking.Services;

public class ServicesMappingProfile : Profile
{
	public ServicesMappingProfile()
	{
		CreateMap<CreateUserInputModel, ApplicationUser>();
		CreateMap<UpdateUserModel, ApplicationUser>();
		CreateMap<ApplicationUser, BaseUserInfoOutputModel>();
		CreateMap<ApplicationUser, TokenOutputModel>();

		CreateMap<ApplicationUser, GetUserOutputModel>()
			.ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments.Count()))
			.ForMember(d => d.FavoriteHotels, o => o.MapFrom(s => s.FavoriteHotels.Count()))
			.ForMember(d => d.OwnedHotels, o => o.MapFrom(s => s.OwnedHotels.Count()))
			.ForMember(d => d.Ratings, o => o.MapFrom(s => s.Ratings.Count()))
			.ForMember(d => d.Replies, o => o.MapFrom(s => s.Replies.Count()))
			.ForMember(d => d.Trips, o => o.MapFrom(s => s.Trips.Count()));

		CreateMap<CreateUpdateRoomInputModel, Room>();
		CreateMap<Room, CreateGetUpdateRoomOutputModel>();
		CreateMap<UpdateHotelModel, Hotel>();
		CreateMap<CreateHotelInputModel, Hotel>();
		CreateMap<City, GetCityOutputModel>();

		CreateMap<ICollection<Rating>, RatingOutputModel>()
			.ForMember(d => d.Rating, o => o.MapFrom(s => s.Count != 0 
				? s.Sum(rating => rating.Value) / (float)s.Count 
				: 0))
			.ForMember(d => d.RatingsCount, o => o.MapFrom(s => s.Count));

		CreateMap<Hotel, BaseHotelInfoOutputModel>();
		CreateMap<Hotel, GetHotelInfoOutputModel>();
		CreateMap<Hotel, GetHotelWithOwnerInfoOutputModel>();

		DateTime checkIn = default;
		DateTime checkOut = default;

		CreateMap<Hotel, GetAvailableHotelRoomsOutputModel>()
			.ForMember(d => d.AvailableRooms, o => o.MapFrom(s => s.Rooms
				.Where(room => !room.IsDeleted && !room.Bookings
					.Any(b => (b.CheckIn <= checkIn && checkIn < b.CheckOut) ||
							  (b.CheckIn < checkOut && checkOut <= b.CheckOut) ||
							  (checkIn <= b.CheckIn && b.CheckOut <= checkOut)))));

		CreateMap<Comment, GetCommentOutputModel>();
	}
}
