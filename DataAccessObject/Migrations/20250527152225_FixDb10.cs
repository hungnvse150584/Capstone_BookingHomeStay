using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "numberBathRoom",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "numberBed",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "numberWifi",
                table: "RoomTypes");

            migrationBuilder.AddColumn<int>(
                name: "numberBathRoom",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "numberBed",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "numberWifi",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "numberBathRoom",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "numberBed",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "numberWifi",
                table: "Rooms");

            migrationBuilder.AddColumn<int>(
                name: "numberBathRoom",
                table: "RoomTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "numberBed",
                table: "RoomTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "numberWifi",
                table: "RoomTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
