using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Data.Entities;

public class ApplicationUser : BaseDeletableEntity
{
	[Required]
    [MaxLength(FirstNameMaxLength)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(LastNameMaxLength)]
    public string LastName { get; set; } = null!;

    [Required]
    [MaxLength(EmailMaxLength)]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    [Required]
    public string Salt { get; set; } = null!;

    [Required]
    [MaxLength(PhoneNumberLength)]
    public string PhoneNumber { get; set; } = null!;

    [InverseProperty("Owner")]
    public ICollection<Hotel> OwnedHotels { get; set; } = new HashSet<Hotel>();

    public ICollection<Hotel> FavoriteHotels { get; set; } = new HashSet<Hotel>();

    public ICollection<Booking> Trips { get; set; } = new HashSet<Booking>();

    public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();

    public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

	public ICollection<Reply> Replies { get; set; } = new HashSet<Reply>();
}
