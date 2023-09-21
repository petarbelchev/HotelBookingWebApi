namespace HotelBooking.Common.Constants;

public static class ValidationMessages
{
	public const string CityNameConstraints = "Each word in the city name must begin with a single uppercase letter and only lowercase letters after that, with a single space between words.";
	public const string UnsupportedImageFileType = "The image file type must be JPEG or PNG.";
	public const string UnsupportedImageFileSize = "The image file size cannot exceed {0} KB.";
	public const string EmptyFileCollection = "You should upload at least one image.";
}
