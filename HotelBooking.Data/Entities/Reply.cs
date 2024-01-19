using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotelBooking.Data.Contracts;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Data.Entities;

public class Reply : RatableEntity
{
    [Required]
    [MaxLength(ContentMaxLength)]
    public string Content { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Author))]
    public int AuthorId { get; set; }
    public ApplicationUser Author { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Comment))]
    public int CommentId { get; set; }
    public Comment Comment { get; set; } = null!;

    [Required]
    public DateTime CreatedOnUtc { get; set; }
}
