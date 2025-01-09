using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2864732b-7191-4f8b-8baf-f62423b1b448");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4616877d-6c00-47b5-809d-37cad745742c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "afceb571-500c-4d9d-beb8-be1bb7ac78bc");

            migrationBuilder.AddColumn<string>(
                name: "FullAddress",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullAddress",
                table: "Locations");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2864732b-7191-4f8b-8baf-f62423b1b448", null, "Customer", "CUSTOMER" },
                    { "4616877d-6c00-47b5-809d-37cad745742c", null, "Owner", "OWNER" },
                    { "afceb571-500c-4d9d-beb8-be1bb7ac78bc", null, "Admin", "ADMIN" }
                });
        }
    }
}
