using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Data.Migrations
{
	public partial class AddedSPForCommentsRepliesSoftDeletion : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql(@"
			    CREATE PROCEDURE usp_MarkReplyRatingsAsDeleted
                    @replyId INT
                AS
                BEGIN TRAN
                BEGIN TRY

                    UPDATE Ratings
                    SET IsDeleted = 1
                    WHERE ReplyId = @replyId

                    COMMIT TRANSACTION

                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION
                END CATCH

                GO

                CREATE PROCEDURE usp_MarkCommentRepliesAndRatingsAsDeleted
                    @commentId INT 
                AS
                BEGIN TRAN
                BEGIN TRY

                    UPDATE Ratings
                    SET IsDeleted = 1
                    WHERE CommentId = @commentId
    
                    UPDATE Replies
                    SET IsDeleted = 1
                    WHERE CommentId = @commentId

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
				DROP PROCEDURE [dbo].[usp_MarkReplyRatingsAsDeleted]
				DROP PROCEDURE [dbo].[usp_MarkCommentRepliesAndRatingsAsDeleted]
			");
		}
	}
}
