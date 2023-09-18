using HotelBooking.Data.Enum;
using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Services.SharedModels;

public class CreateUpdateRoomInputModel
{
    [Required]
    [StringLength(RoomNumberLength, MinimumLength = RoomNumberLength)]
    public string Number { get; set; } = null!;

    [Required]
    [Range(RoomMinCapacity, RoomMaxCapacity)]
    public byte Capacity { get; set; }

    [Required]
    [Range(RoomMinPricePerNight, RoomMaxPricePerNight)]
    public decimal PricePerNight { get; set; }

    [Required]
    public RoomType RoomType { get; set; }

    [Required]
    public bool HasAirConditioner { get; set; }

    [Required]
    public bool HasBalcony { get; set; }

    [Required]
    public bool HasKitchen { get; set; }

    [Required]
    public bool IsSmokingAllowed { get; set; }
}
