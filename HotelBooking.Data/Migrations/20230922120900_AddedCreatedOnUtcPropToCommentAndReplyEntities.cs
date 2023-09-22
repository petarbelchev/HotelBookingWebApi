using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Data.Migrations
{
    public partial class AddedCreatedOnUtcPropToCommentAndReplyEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Bookings",
                newName: "CreatedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CheckOut",
                table: "Bookings",
                newName: "CheckOutUtc");

            migrationBuilder.RenameColumn(
                name: "CheckIn",
                table: "Bookings",
                newName: "CheckInUtc");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUtc",
                table: "Replies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUtc",
                table: "Comments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOnUtc",
                table: "Replies");

            migrationBuilder.DropColumn(
                name: "CreatedOnUtc",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                table: "Bookings",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "CheckOutUtc",
                table: "Bookings",
                newName: "CheckOut");

            migrationBuilder.RenameColumn(
                name: "CheckInUtc",
                table: "Bookings",
                newName: "CheckIn");
        }
    }
}
