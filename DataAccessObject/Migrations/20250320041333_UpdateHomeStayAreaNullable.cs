using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHomeStayAreaNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "532acf00-6fbc-41b8-802f-2fa10cc143b1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "57e83061-893c-43ad-9995-35123c42adb9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "aab086d5-4835-4161-bfe2-b61d8f4100f6");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "84528f74-3377-40b2-8f94-472738a1114a", null, "Owner", "OWNER" },
                    { "a02bdc66-b11d-4e49-8d2b-5ac2190e547e", null, "Customer", "CUSTOMER" },
                    { "e35cc949-c923-46c2-b04f-0782126f5ce0", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "84528f74-3377-40b2-8f94-472738a1114a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a02bdc66-b11d-4e49-8d2b-5ac2190e547e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e35cc949-c923-46c2-b04f-0782126f5ce0");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "532acf00-6fbc-41b8-802f-2fa10cc143b1", null, "Admin", "ADMIN" },
                    { "57e83061-893c-43ad-9995-35123c42adb9", null, "Owner", "OWNER" },
                    { "aab086d5-4835-4161-bfe2-b61d8f4100f6", null, "Customer", "CUSTOMER" }
                });
        }
    }
}
