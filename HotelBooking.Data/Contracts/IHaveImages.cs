using HotelBooking.Data.Entities;

namespace HotelBooking.Data.Contracts;

public interface IHaveImages
{
	public int? MainImageId { get; set; }
	
	public Image? MainImage { get; set; }

	public ICollection<Image> Images { get; set; }
}
