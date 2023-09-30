using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Data.Contracts;

public abstract class BaseSoftDeleteEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public bool IsDeleted { get; set; }
}
