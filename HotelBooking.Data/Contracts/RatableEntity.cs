using HotelBooking.Data.Entities;

namespace HotelBooking.Data.Contracts;

public abstract class RatableEntity : BaseSoftDeleteEntity
{
    public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
}
