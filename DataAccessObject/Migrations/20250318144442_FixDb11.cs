using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "288ac401-2531-4daf-a1de-f96e52a01ebf");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6df49116-3b88-4521-8ff2-bde81201878d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f56c5c1f-4c3d-4e85-8ac9-fc08d8fe6062");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { "288ac401-2531-4daf-a1de-f96e52a01ebf", null, "Customer", "CUSTOMER" },
                    { "6df49116-3b88-4521-8ff2-bde81201878d", null, "Admin", "ADMIN" },
                    { "f56c5c1f-4c3d-4e85-8ac9-fc08d8fe6062", null, "Owner", "OWNER" }
                });
        }
    }
}
