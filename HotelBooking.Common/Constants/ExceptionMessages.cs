namespace HotelBooking.Common.Constants;

public static class ExceptionMessages
{
	public const string ExistingEmailAddress = "The email address \"{0}\" is already in use.";
	public const string ExistingPhoneNumber = "The phone number \"{0}\" is already in use.";
	public const string NonexistentCity = "A city with id \"{0}\" doesn't exists.";
	public const string ExistingCity = "A city with name \"{0}\" already exists.";
	public const string NonexistentEntity = "A {0} with id \"{1}\" doesn't exists.";
	public const string NonexistentHotel = "A hotel with id \"{0}\" doesn't exists.";
	public const string NonexistentRoom = "A room with id \"{0}\" doesn't exists.";
	public const string NonexistentComment = "A comment with id \"{0}\" doesn't exists.";
	public const string NonexistentReply = "A reply with id \"{0}\" doesn't exists.";
	public const string NonexistentNavigationProperty = "The Image entity doesn't have navigation property to {0}";
	public const string NonexistentImageOrUnauthorizedUser = "The image doesn't exists or the user is unauthorized.";
}
