using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBooking.Data.Migrations
{
    public partial class AddedMainImageRelationToHotelAndRoomEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MainImageId",
                table: "Rooms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MainImageId",
                table: "Hotels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_MainImageId",
                table: "Rooms",
                column: "MainImageId",
                unique: true,
                filter: "[MainImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_MainImageId",
                table: "Hotels",
                column: "MainImageId",
                unique: true,
                filter: "[MainImageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Images_MainImageId",
                table: "Hotels",
                column: "MainImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Images_MainImageId",
                table: "Rooms",
                column: "MainImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Images_MainImageId",
                table: "Hotels");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Images_MainImageId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_MainImageId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Hotels_MainImageId",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "MainImageId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "MainImageId",
                table: "Hotels");
        }
    }
}
