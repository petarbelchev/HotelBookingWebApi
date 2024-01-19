using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Data.Migrations
{
    public partial class AddedSPForUserAndHotelRelatedDataSoftDeletion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
				CREATE PROCEDURE [dbo].[usp_MarkUserRelatedDataAsDeleted]
					@userId INT 
				AS
				BEGIN TRANSACTION
				BEGIN TRY
	
					SELECT [Id]
					  INTO #HotelIds
					  FROM [Hotels]
					 WHERE [OwnerId] = @userId

					UPDATE [Hotels]
					   SET [IsDeleted] = 1
					 WHERE [Id] IN (SELECT [Id] FROM #HotelIds)

					UPDATE [Rooms]
					   SET [IsDeleted] = 1
					 WHERE [HotelId] IN (SELECT [Id] FROM #HotelIds)
	
					SELECT [Id]
					  INTO #CommentIds
					  FROM [Comments]
					 WHERE [AuthorId] = @userId
						OR [HotelId] IN (SELECT [Id] FROM #HotelIds)

					UPDATE [Comments]
					   SET [IsDeleted] = 1
					 WHERE [Id] IN (SELECT [Id] FROM #CommentIds)
	
					SELECT [Id]
					  INTO #ReplyIds
					  FROM [Replies]
					 WHERE [AuthorId] = @userId
						OR [CommentId] IN (SELECT [Id] FROM #CommentIds)

					UPDATE [Replies]
					   SET [IsDeleted] = 1
					 WHERE [Id] IN (SELECT [Id] FROM #ReplyIds)

					UPDATE [Ratings]
					   SET [IsDeleted] = 1
					 WHERE [OwnerId] = @userId
						OR [HotelId] IN (SELECT [Id] FROM #HotelIds)
						OR [CommentId] IN (SELECT [Id] FROM #CommentIds)
						OR [ReplyId] IN (SELECT [Id] FROM #ReplyIds)

					COMMIT TRANSACTION
				END TRY
				BEGIN CATCH
					ROLLBACK TRANSACTION
				END CATCH

				GO

				CREATE PROCEDURE [dbo].[usp_MarkHotelRelatedDataAsDeleted]
					@hotelId INT
				AS
				BEGIN TRANSACTION
				BEGIN TRY

					UPDATE [Hotels]
					   SET [IsDeleted] = 1
					 WHERE [Id] = @hotelId

					UPDATE [Rooms]
					   SET [IsDeleted] = 1
					 WHERE [HotelId] = @hotelId

					SELECT [Id]
					  INTO #CommentIds
					  FROM [Comments]
					 WHERE [HotelId] = @hotelId
	
					UPDATE [Comments]
					   SET [IsDeleted] = 1
					 WHERE [Id] IN (SELECT [Id] FROM #CommentIds)
	
					SELECT [Id]
					  INTO #ReplyIds
					  FROM [Replies]
					 WHERE [CommentId] IN (SELECT [Id] FROM #CommentIds)
	
					UPDATE [Replies]
					   SET [IsDeleted] = 1
					 WHERE [Id] IN (SELECT [Id] FROM #ReplyIds)
	
					UPDATE [Ratings]
					   SET [IsDeleted] = 1
					 WHERE [HotelId] = @hotelId 
						OR [CommentId] IN (SELECT [Id] FROM #CommentIds) 
						OR [ReplyId] IN (SELECT [Id] FROM #ReplyIds)

					COMMIT TRANSACTION
				END TRY
				BEGIN CATCH
					ROLLBACK TRANSACTION
				END CATCH
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
				DROP PROCEDURE [dbo].[usp_MarkUserRelatedDataAsDeleted]
				DROP PROCEDURE [dbo].[usp_MarkHotelRelatedDataAsDeleted]
			");
        }
    }
}
