using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Data.Migrations
{
    public partial class ChangedSPForUserHotelsDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
		{
			string storedProcedure = @"
				CREATE OR ALTER PROCEDURE usp_MarkUserHotelsAndRoomsAsDeleted
					@userId INT 
				AS
				BEGIN TRAN
				BEGIN TRY

					UPDATE Hotels
					SET IsDeleted = 1
					WHERE OwnerId = @userId

					UPDATE Rooms
					SET IsDeleted = 1
					WHERE Id IN (
						SELECT r.Id
						FROM Rooms AS r
						INNER JOIN Hotels AS h ON h.Id = r.HotelId
						WHERE h.OwnerId = @userId
					)

					COMMIT TRANSACTION
				END TRY
				BEGIN CATCH
					ROLLBACK TRANSACTION
				END CATCH"
			;

			migrationBuilder.Sql(storedProcedure);
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql("DROP PROCEDURE usp_MarkUserHotelsAndRoomsAsDeleted");
        }
    }
}
