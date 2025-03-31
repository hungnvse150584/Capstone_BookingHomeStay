using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "90338ea8-86e3-4590-a946-a6b1bb5a5bd8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cce47e9f-62cb-499f-982c-994dedd7b2c6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f173272a-35d4-4a38-9edb-e20dc8ac8975");

            migrationBuilder.AddColumn<double>(
                name: "TotalRentPrice",
                table: "Bookings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalRentPrice",
                table: "Bookings");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "90338ea8-86e3-4590-a946-a6b1bb5a5bd8", null, "Customer", "CUSTOMER" },
                    { "cce47e9f-62cb-499f-982c-994dedd7b2c6", null, "Admin", "ADMIN" },
                    { "f173272a-35d4-4a38-9edb-e20dc8ac8975", null, "Owner", "OWNER" }
                });
        }
    }
}
