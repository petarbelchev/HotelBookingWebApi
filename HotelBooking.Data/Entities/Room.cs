using HotelBooking.Data.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Data.Entities;

public class Room
{
	[Key]
	public int Id { get; set; }

    [Required]
    [MaxLength(RoomNumberLength)]
    public int Number { get; set; }

    [Required]
    [MaxLength(RoomCapacityMaxLength)]
    public byte Capacity { get; set; }

    [Required]
    [Column(TypeName = "decimal(6,2)")]
    public decimal PricePerNight { get; set; }

    [Required]
	[Column(TypeName = "tinyint")]
	public RoomType RoomType { get; set; }

    [Required]
    [ForeignKey(nameof(Hotel))]
    public int HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;

    [Required]
    public bool HasAirConditioner { get; set; }

    [Required]
    public bool HasBalcony { get; set; }

    [Required]
    public bool HasKitchen { get; set; }

    [Required]
    public bool IsSmokingAllowed { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
}
