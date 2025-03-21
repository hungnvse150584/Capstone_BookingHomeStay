using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0531b1ab-b3bb-4748-9b63-e54821990f43");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95be26d3-3d4a-410c-a927-381e7ce6f493");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d1d1631c-92d8-4fb6-a623-eb816e84e192");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0531b1ab-b3bb-4748-9b63-e54821990f43", null, "Admin", "ADMIN" },
                    { "95be26d3-3d4a-410c-a927-381e7ce6f493", null, "Owner", "OWNER" },
                    { "d1d1631c-92d8-4fb6-a623-eb816e84e192", null, "Customer", "CUSTOMER" }
                });
        }
    }
}
