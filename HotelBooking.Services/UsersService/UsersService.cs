using HotelBooking.Data.Entities;
using HotelBooking.Data.Repositories;
using Microsoft.Data.SqlClient;

namespace HotelBooking.Services.UsersService;

public class UsersService : IUsersService
{
	private readonly IRepository<ApplicationUser> usersRepo;

	public UsersService(IRepository<ApplicationUser> usersRepo)
		=> this.usersRepo = usersRepo;

	public async Task DeleteUserInfo(int userId)
	{
		await usersRepo.ExecuteSqlRawAsync(
			"EXEC [dbo].[usp_MarkUserRelatedDataAsDeleted] @userId",
			new SqlParameter("@userId", userId));
	}
}
