using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Data.Entities;

public class Image
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Owner))]
    public int? OwnerId { get; set; }
    public ApplicationUser? Owner { get; set; }

    [ForeignKey(nameof(Hotel))]
    public int? HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    [ForeignKey(nameof(Room))]
    public int? RoomId { get; set; }
    public Room? Room { get; set; }
}
