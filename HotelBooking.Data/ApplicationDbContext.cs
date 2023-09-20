using HotelBooking.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{ }

	public DbSet<ApplicationUser> Users { get; set; } = null!;

	public DbSet<Hotel> Hotels { get; set; } = null!;

	public DbSet<Room> Rooms { get; set; } = null!;

	public DbSet<Booking> Bookings { get; set; } = null!;

	public DbSet<Comment> Comments { get; set; } = null!;

	public DbSet<Rating> Ratings { get; set; } = null!;
	
	public DbSet<Reply> Replies { get; set; } = null!;

	public DbSet<City> Cities { get; set; } = null!;

	public DbSet<Image> Images { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<ApplicationUser>(user =>
		{
			user
				.HasMany(x => x.OwnedHotels)
				.WithOne(x => x.Owner)
				.OnDelete(DeleteBehavior.Restrict);

			user
				.HasMany(x => x.Ratings)
				.WithOne(x => x.Owner)
				.OnDelete(DeleteBehavior.Restrict);

			user
				.HasMany(x => x.Replies)
				.WithOne(x => x.Author)
				.OnDelete(DeleteBehavior.Restrict);

			user
				.HasMany(x => x.FavoriteHotels)
				.WithMany(x => x.UsersWhoFavorited)
				.UsingEntity(
					"UserHotelFavoritesJoinTable",
					j =>
					{
						j.Property("FavoriteHotelsId").HasColumnName("HotelId");
						j.Property("UsersWhoFavoritedId").HasColumnName("CustomerId");
					});
		});
	}
}
