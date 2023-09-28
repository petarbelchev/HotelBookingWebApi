using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Data.Migrations
{
    public partial class AddedSPForSoftDeletingRepliesAndRatings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var storedProcedure = @"
				CREATE OR ALTER PROCEDURE usp_MarkReplyRatingsAsDeleted
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
				END CATCH"
			;

			migrationBuilder.Sql(storedProcedure);

			storedProcedure = @"
				CREATE OR ALTER PROCEDURE usp_MarkCommentRepliesAndRatingsAsDeleted
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
				END CATCH"
			;

			migrationBuilder.Sql(storedProcedure);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DROP PROCEDURE usp_MarkReplyRatingsAsDeleted");
			migrationBuilder.Sql("DROP PROCEDURE usp_MarkCommentRepliesAndRatingsAsDeleted");
		}
    }
}
