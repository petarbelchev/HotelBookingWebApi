using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotelBooking.Data.Contracts;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Data.Entities;

public class Comment : RatableEntity
{
	[Required]
	[MaxLength(ContentMaxLength)]
	public string Content { get; set; } = null!;

	[Required]
    [ForeignKey(nameof(Author))]
    public int AuthorId { get; set; }
    public ApplicationUser Author { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Hotel))]
    public int HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;

    [Required]
    public DateTime CreatedOnUtc { get; set; }

    public ICollection<Reply> Replies { get; set; } = new HashSet<Reply>();
}
