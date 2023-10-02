namespace HotelBooking.Common.Constants;

public static class ExceptionMessages
{
	public const string ExistingEmailAddress = "The email address \"{0}\" is already in use.";
	public const string ExistingPhoneNumber = "The phone number \"{0}\" is already in use.";
	public const string ExistingCity = "A city with name \"{0}\" already exists.";
	public const string NonexistentEntity = "A {0} with id \"{1}\" doesn't exists.";
	public const string NotAvailableRoom = "A room with id \"{0}\" is not available.";
	public const string ExistingRoomNumber = "A room with number \"{0}\" already exists.";
	public const string NonexistentNavigationProperty = "The Image entity doesn't have navigation property to {0}";
	public const string NonexistentImageOrUnauthorizedUser = "The image doesn't exists or the user is unauthorized.";
	public const string CantCancelOnCheckInOrAfter = "Cannot cancel a booking on check-in day or after.";
}
