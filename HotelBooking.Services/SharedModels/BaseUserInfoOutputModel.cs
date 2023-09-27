namespace HotelBooking.Services.SharedModels;

public class BaseUserInfoOutputModel
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
}
