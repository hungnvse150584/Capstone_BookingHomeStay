using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class TransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6c17d153-a9da-492b-bb4d-ee364d5701f3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a1a0deac-1eaf-45ea-8209-15b0384889a3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "edf64c66-6ba9-444f-8259-d0f1378c1528");

            migrationBuilder.AddColumn<string>(
                name: "transactionID",
                table: "BookingServices",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "transactionID",
                table: "Bookings",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    ResponseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TmnCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TxnRef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    OrderInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BankTranNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PayDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BankCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecureHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.ResponseId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingServices_transactionID",
                table: "BookingServices",
                column: "transactionID",
                unique: true,
                filter: "[transactionID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_transactionID",
                table: "Bookings",
                column: "transactionID",
                unique: true,
                filter: "[transactionID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Transactions_transactionID",
                table: "Bookings",
                column: "transactionID",
                principalTable: "Transactions",
                principalColumn: "ResponseId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingServices_Transactions_transactionID",
                table: "BookingServices",
                column: "transactionID",
                principalTable: "Transactions",
                principalColumn: "ResponseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Transactions_transactionID",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingServices_Transactions_transactionID",
                table: "BookingServices");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_BookingServices_transactionID",
                table: "BookingServices");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_transactionID",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "transactionID",
                table: "BookingServices");

            migrationBuilder.DropColumn(
                name: "transactionID",
                table: "Bookings");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6c17d153-a9da-492b-bb4d-ee364d5701f3", null, "Owner", "OWNER" },
                    { "a1a0deac-1eaf-45ea-8209-15b0384889a3", null, "Admin", "ADMIN" },
                    { "edf64c66-6ba9-444f-8259-d0f1378c1528", null, "Customer", "CUSTOMER" }
                });
        }
    }
}
