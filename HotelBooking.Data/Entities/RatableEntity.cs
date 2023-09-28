namespace HotelBooking.Data.Entities;

public abstract class RatableEntity : BaseSoftDeleteEntity
{
    public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
}
