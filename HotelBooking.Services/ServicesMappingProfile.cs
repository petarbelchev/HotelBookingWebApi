using AutoMapper;
using HotelBooking.Data.Entities;
using HotelBooking.Services.HotelsService.Models;
using HotelBooking.Services.UsersService.Models;

namespace HotelBooking.Services;

public class ServicesMappingProfile : Profile
{
    public ServicesMappingProfile()
    {
        CreateMap<CreateUserInputModel, ApplicationUser>();
        CreateMap<UpdateUserModel, ApplicationUser>();
        CreateMap<ApplicationUser, TokenOutputModel>();

        CreateMap<ApplicationUser, UserDetailsOutputModel>()
            .ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments.Count()))
            .ForMember(d => d.FavoriteHotels, o => o.MapFrom(s => s.FavoriteHotels.Count()))
            .ForMember(d => d.OwnedHotels, o => o.MapFrom(s => s.OwnedHotels.Count()))
            .ForMember(d => d.Ratings, o => o.MapFrom(s => s.Ratings.Count()))
            .ForMember(d => d.Replies, o => o.MapFrom(s => s.Replies.Count()))
            .ForMember(d => d.Trips, o => o.MapFrom(s => s.Trips.Count()));
        
        CreateMap<CreateRoomInputModel, Room>();
        CreateMap<UpdateHotelModel, Hotel>();
        CreateMap<CreateHotelInputModel, Hotel>();
	}
}
