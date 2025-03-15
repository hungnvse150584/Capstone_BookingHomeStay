using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0325a81e-11ae-4530-a920-fcdc4517d69d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "94c6652b-13e0-40f6-9f08-46effec1271f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b03e23bc-f35e-406f-b1de-a71953b81cdc");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0705219e-f526-4f4e-b4c8-497f32976489", null, "Customer", "CUSTOMER" },
                    { "0e32d3ad-2548-43ae-85fa-35c16970b6fe", null, "Owner", "OWNER" },
                    { "a6b85be8-b3b8-480e-a881-409333ddead1", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0705219e-f526-4f4e-b4c8-497f32976489");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0e32d3ad-2548-43ae-85fa-35c16970b6fe");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a6b85be8-b3b8-480e-a881-409333ddead1");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0325a81e-11ae-4530-a920-fcdc4517d69d", null, "Admin", "ADMIN" },
                    { "94c6652b-13e0-40f6-9f08-46effec1271f", null, "Customer", "CUSTOMER" },
                    { "b03e23bc-f35e-406f-b1de-a71953b81cdc", null, "Owner", "OWNER" }
                });
        }
    }
}
