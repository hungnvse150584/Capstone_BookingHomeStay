using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269ed675-f6be-4e6a-be52-9622998dabd3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "aba3ace7-0caa-4d5d-a384-fd35eb2dba4a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c14d5067-3c55-40d4-97a0-ee999ca1e9c0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c39df32b-df21-4266-bd0d-37ecc5c077f6");

            migrationBuilder.AlterColumn<string>(
                name: "BankAccountNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BankAccountNumber",
                table: "AspNetUsers",
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
                    { "269ed675-f6be-4e6a-be52-9622998dabd3", null, "Staff", "STAFF" },
                    { "aba3ace7-0caa-4d5d-a384-fd35eb2dba4a", null, "Customer", "CUSTOMER" },
                    { "c14d5067-3c55-40d4-97a0-ee999ca1e9c0", null, "Admin", "ADMIN" },
                    { "c39df32b-df21-4266-bd0d-37ecc5c077f6", null, "Owner", "OWNER" }
                });
        }
    }
}
