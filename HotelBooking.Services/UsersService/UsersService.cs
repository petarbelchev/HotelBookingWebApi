using HotelBooking.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Services.UsersService;

public class UsersService : IUsersService
{
	private readonly ApplicationDbContext dbContext;

	public UsersService(ApplicationDbContext dbContext)
		=> this.dbContext = dbContext;

	public async Task DeleteUserInfo(int userId)
	{
		await dbContext.Database.ExecuteSqlRawAsync(
			"EXEC [dbo].[usp_MarkUserRelatedDataAsDeleted] @userId",
			new SqlParameter("@userId", userId));
	}
}
