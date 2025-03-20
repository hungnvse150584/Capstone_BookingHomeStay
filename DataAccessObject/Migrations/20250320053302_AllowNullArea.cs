using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "Area",
                table: "HomeStays",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2c73fbe8-af8c-48b6-94dc-38ccae7e41c5", null, "Owner", "OWNER" },
                    { "58940479-d741-4cf6-9cc0-f4916f085bc5", null, "Admin", "ADMIN" },
                    { "b7559bef-c073-4761-8ded-5188d234a188", null, "Customer", "CUSTOMER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c73fbe8-af8c-48b6-94dc-38ccae7e41c5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "58940479-d741-4cf6-9cc0-f4916f085bc5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b7559bef-c073-4761-8ded-5188d234a188");

            migrationBuilder.AlterColumn<string>(
                name: "Area",
                table: "HomeStays",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

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
    }
}
