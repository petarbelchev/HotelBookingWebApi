namespace HotelBooking.Common.Constants;

public static class EntityValidationConstants
{
	public const int EmailMaxLength = 320;

	public const int FirstNameMaxLength = 50;
	public const int FirstNameMinLength = 2;
	
	public const int LastNameMaxLength = 50;
	public const int LastNameMinLength = 2;
	
	public const int PhoneNumberLength = 10;

	public const int HotelNameMaxLength = 100;
	public const int HotelNameMinLength = 10;

	public const int HotelAddressMaxLength = 100;
	public const int HotelAddressMinLength = 10;

	public const int HotelDescriptionMaxLength = 1000;
	public const int HotelDescriptionMinLength = 10;

	public const int RoomNumberLength = 3;
	public const int RoomMaxCapacity = 10;
	public const int RoomMinCapacity = 1;
	public const double RoomMaxPricePerNight = double.MaxValue;
	public const int RoomMinPricePerNight = 1;

	public const int RatingMaxValue = 10;

	public const int PasswordMaxLength = 16;
	public const int PasswordMinLength = 6;

	public const int CityNameMaxLength = 50;
	public const int CityNameMinLength = 2;
	public const string CityNameRegEx = @"^[A-Z][a-z]+(\s[A-Z][a-z]+)?$";

	public const int ContentMaxLength = 500;
	public const int ContentMinLength = 5;
}
