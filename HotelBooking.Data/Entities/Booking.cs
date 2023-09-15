﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBooking.Data.Entities;

public class Booking
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime CreatedOn { get; set; }

    [Required]
    public DateTime CheckIn { get; set; }

    [Required]
    public DateTime CheckOut { get; set; }

    [Required]
    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }
    public ApplicationUser Customer { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Room))]
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;
}