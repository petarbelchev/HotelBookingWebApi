using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Data.Entities;

public abstract class BaseDeletableEntity
{
	[Key]
	public int Id { get; set; }

	[Required]
    public bool IsDeleted { get; set; }
}
