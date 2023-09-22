namespace HotelBooking.Data.Entities;

public abstract class RatableEntity : BaseDeletableEntity
{
    public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
}
