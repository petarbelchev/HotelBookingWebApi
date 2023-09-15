﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static HotelBooking.Common.Constants.EntityValidationConstants;

namespace HotelBooking.Data.Entities;

public class Reply
{
	[Key]
	public int Id { get; set; }

	[Required]
	[MaxLength(ContentMaxLength)]
	public string ReplyContent { get; set; } = null!;

	[Required]
	[ForeignKey(nameof(Author))]
	public int AuthorId { get; set; }
	public ApplicationUser Author { get; set; } = null!;

	[Required]
	[ForeignKey(nameof(Comment))]
    public int CommentId { get; set; }
	public Comment Comment { get; set; } = null!;

	public ICollection<Rating> Ratings { get; set; } = new HashSet<Rating>();
}
