using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "44541194-3ed6-46a7-b188-ae643188c562");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b61f9910-31a9-4e9f-b9bd-35c63eff2308");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b9f5a747-b36d-4860-8a16-7920450f3652");

            migrationBuilder.AddColumn<int>(
                name: "RemainingRooms",
                table: "RoomAvailabilities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingRooms",
                table: "RoomAvailabilities");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "44541194-3ed6-46a7-b188-ae643188c562", null, "Owner", "OWNER" },
                    { "b61f9910-31a9-4e9f-b9bd-35c63eff2308", null, "Customer", "CUSTOMER" },
                    { "b9f5a747-b36d-4860-8a16-7920450f3652", null, "Admin", "ADMIN" }
                });
        }
    }
}
