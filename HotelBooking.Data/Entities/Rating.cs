using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Data.Entities;

public class Rating : BaseDeletableEntity
{
    [Required]
    public byte Value { get; set; }

    [Required]
    [ForeignKey(nameof(Owner))]
    public int OwnerId { get; set; }
    public ApplicationUser Owner { get; set; } = null!;

    [ForeignKey(nameof(Hotel))]
    public int? HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    [ForeignKey(nameof(Comment))]
    public int? CommentId { get; set; }    
    public Comment? Comment { get; set; }

	[ForeignKey(nameof(Reply))]
	public int? ReplyId { get; set; }
	public Reply? Reply { get; set; }
}
