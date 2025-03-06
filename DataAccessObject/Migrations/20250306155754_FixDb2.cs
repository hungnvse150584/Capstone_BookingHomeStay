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
                keyValue: "0220ff71-788d-4d7a-b1d8-b0223f47c9e5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "380f43d7-6d1d-41ae-a028-4237a9916610");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6cc48218-c375-4dfc-99f3-173fc928e109");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0220ff71-788d-4d7a-b1d8-b0223f47c9e5", null, "Owner", "OWNER" },
                    { "380f43d7-6d1d-41ae-a028-4237a9916610", null, "Customer", "CUSTOMER" },
                    { "6cc48218-c375-4dfc-99f3-173fc928e109", null, "Admin", "ADMIN" }
                });
        }
    }
}
