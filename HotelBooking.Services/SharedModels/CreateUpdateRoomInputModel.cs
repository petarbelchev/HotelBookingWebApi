using HotelBooking.Data.Enum;
using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;
using static HotelBooking.Common.Constants.ValidationMessages;

namespace HotelBooking.Services.SharedModels;

public class CreateUpdateRoomInputModel
{
	[Required]
    [StringLength(
        RoomNumberLength, 
        MinimumLength = RoomNumberLength,
        ErrorMessage = InvalidPropertyLength)]
    public string Number { get; set; } = null!;

    [Required]
    [Range(
        RoomMinCapacity, 
        RoomMaxCapacity,
        ErrorMessage = InvalidPropertyRange)]
    public byte Capacity { get; set; }

    [Required]
    [Range(
        RoomMinPricePerNight, 
        RoomMaxPricePerNight,
        ErrorMessage = InvalidPropertyRange)]
    [Display(Name = "Price per night")]
    public decimal PricePerNight { get; set; }

    [Required]
    [Range(
        RoomTypeMinRange, 
        RoomTypeMaxRange,
        ErrorMessage = InvalidPropertyRange)]
	[Display(Name = "Room type")]
	public int? RoomType { get; set; }

    [Required]
	[Display(Name = "Air Conditioner")]
	public bool HasAirConditioner { get; set; }

    [Required]
	[Display(Name = "Balcony")]
	public bool HasBalcony { get; set; }

    [Required]
	[Display(Name = "Kitchen")]
	public bool HasKitchen { get; set; }

    [Required]
	[Display(Name = "Smoking Allowed")]
	public bool IsSmokingAllowed { get; set; }
}
