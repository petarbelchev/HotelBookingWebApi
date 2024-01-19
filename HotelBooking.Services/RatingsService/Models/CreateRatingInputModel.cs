using System.ComponentModel.DataAnnotations;
using static HotelBooking.Common.Constants.EntityValidationConstants;
using static HotelBooking.Common.Constants.ValidationMessages;

namespace HotelBooking.Services.RatingsService.Models;

public class CreateRatingInputModel
{
    [Required]
    [Range(
        RatingMinValue,
        RatingMaxValue,
        ErrorMessage = InvalidPropertyRange)]
    [Display(Name = "Rating")]
    public byte Value { get; set; }
}
