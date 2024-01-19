using HotelBooking.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Data.Entities;

public class Booking
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime CreatedOnUtc { get; set; }

    [Required]
    public DateTime CheckInUtc { get; set; }

    [Required]
    public DateTime CheckOutUtc { get; set; }

    [Required]
    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }
    public ApplicationUser Customer { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Room))]
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    [Required]
    public BookingStatus Status { get; set; }
}
