using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Services.RatingsService.Models;

public class CreateRatingInputModel
{
    [Required]
    [Range(1, RatingMaxValue)]
    public byte Value { get; set; }
}
