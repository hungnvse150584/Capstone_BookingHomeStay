using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookingID",
                table: "Rating",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RatingID",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RatingID",
                table: "Bookings",
                column: "RatingID",
                unique: true,
                filter: "[RatingID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Rating_RatingID",
                table: "Bookings",
                column: "RatingID",
                principalTable: "Rating",
                principalColumn: "RatingID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Rating_RatingID",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_RatingID",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "BookingID",
                table: "Rating");

            migrationBuilder.DropColumn(
                name: "RatingID",
                table: "Bookings");
        }
    }
}
