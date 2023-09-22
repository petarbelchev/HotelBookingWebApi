using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Data.Entities;

public class Comment : RatableEntity
{
	[Required]
	[MaxLength(ContentMaxLength)]
	public string CommentContent { get; set; } = null!;

	[Required]
    [ForeignKey(nameof(Author))]
    public int AuthorId { get; set; }
    public ApplicationUser Author { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Hotel))]
    public int HotelId { get; set; }
    public Hotel Hotel { get; set; } = null!;

	public ICollection<Reply> Replies { get; set; } = new HashSet<Reply>();
}
