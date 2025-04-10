using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Transactions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomeStayID",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Percentage",
                table: "Prices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "HomeStayID",
                table: "BookingServices",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "354615ec-da93-4217-9147-be8ca6b5d766", null, "Owner", "OWNER" },
                    { "a7d27115-e9a8-4136-b28f-5a1dcc35220a", null, "Admin", "ADMIN" },
                    { "ba66768a-3a13-43d1-956b-a55b4aa36fc8", null, "Customer", "CUSTOMER" },
                    { "ce80d188-0f20-4be6-8828-8353a7c815a3", null, "Staff", "STAFF" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_HomeStayID",
                table: "Transactions",
                column: "HomeStayID");

            migrationBuilder.CreateIndex(
                name: "IX_BookingServices_HomeStayID",
                table: "BookingServices",
                column: "HomeStayID");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingServices_HomeStays_HomeStayID",
                table: "BookingServices",
                column: "HomeStayID",
                principalTable: "HomeStays",
                principalColumn: "HomeStayID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AspNetUsers_AccountId",
                table: "Transactions",
                column: "AccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_HomeStays_HomeStayID",
                table: "Transactions",
                column: "HomeStayID",
                principalTable: "HomeStays",
                principalColumn: "HomeStayID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingServices_HomeStays_HomeStayID",
                table: "BookingServices");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AspNetUsers_AccountId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_HomeStays_HomeStayID",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AccountId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_HomeStayID",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_BookingServices_HomeStayID",
                table: "BookingServices");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "354615ec-da93-4217-9147-be8ca6b5d766");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a7d27115-e9a8-4136-b28f-5a1dcc35220a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ba66768a-3a13-43d1-956b-a55b4aa36fc8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ce80d188-0f20-4be6-8828-8353a7c815a3");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "HomeStayID",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Percentage",
                table: "Prices");

            migrationBuilder.DropColumn(
                name: "HomeStayID",
                table: "BookingServices");
        }
    }
}
