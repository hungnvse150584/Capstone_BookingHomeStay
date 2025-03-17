using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b8be1a2a-88d5-4258-a1ec-e3c3d71ab871");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c4ccc551-6758-4825-8e05-a678444d6046");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ddfe4eaa-bbd5-438c-9816-c81b432b2487");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "HomeStays",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "HomeStays",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "33910f06-8466-4b84-b41e-afd4c490a51c", null, "Customer", "CUSTOMER" },
                    { "5ffa7a99-5bd4-4eda-9144-468c8924ed80", null, "Admin", "ADMIN" },
                    { "a8df72cb-bf34-42b1-af03-f58a3432bb47", null, "Owner", "OWNER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "33910f06-8466-4b84-b41e-afd4c490a51c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ffa7a99-5bd4-4eda-9144-468c8924ed80");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a8df72cb-bf34-42b1-af03-f58a3432bb47");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "HomeStays");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "HomeStays");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "b8be1a2a-88d5-4258-a1ec-e3c3d71ab871", null, "Owner", "OWNER" },
                    { "c4ccc551-6758-4825-8e05-a678444d6046", null, "Admin", "ADMIN" },
                    { "ddfe4eaa-bbd5-438c-9816-c81b432b2487", null, "Customer", "CUSTOMER" }
                });
        }
    }
}
