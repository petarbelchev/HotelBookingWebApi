using HotelBooking.Services.UsersService.Models;

namespace HotelBooking.Services.UsersService;

public interface IUsersService
{
	/// <exception cref="ArgumentException">When a user with the given email or phone number exists.</exception>
	Task CreateUser(CreateUserInputModel inputModel);

	Task DeleteUser(int id);
	
	Task<GetUserOutputModel?> GetUser(int id);

	Task<TokenOutputModel?> LoginUser(LoginUserInputModel inputModel);

	Task UpdateUser(int id, UpdateUserModel inputModel);
}
