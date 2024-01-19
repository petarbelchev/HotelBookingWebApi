namespace HotelBooking.Common.Constants;

public static class ValidationMessages
{
    public const string CityNameConstraints = "Each word in the city name must begin with a single uppercase letter and only lowercase letters after that, with a single space between words.";

    public const string EmptyFileCollection = "You should upload at least one image.";

    public const string InvalidPropertyLength = "The {0} must be between {2} and {1} characters long.";

    public const string InvalidPropertyRange = "The {0} must be between {1} and {2}.";

    public const string PhoneNumberConstraints = "The phone number must be 10 digits long, starting with zero.";

    public const string UnsupportedImageFileSize = "The image file size cannot exceed {0} KB.";

    public const string UnsupportedImageFileType = "The image file type must be JPEG or PNG.";
}
