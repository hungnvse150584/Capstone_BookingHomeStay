using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDbFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetails_RoomTypes_RoomTypesID",
                table: "BookingDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Transactions_transactionID",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingServices_Transactions_transactionID",
                table: "BookingServices");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomAvailabilities_RoomAvailabilityID",
                table: "Rooms");

            migrationBuilder.DropTable(
                name: "RoomAvailabilities");

            migrationBuilder.DropIndex(
                name: "IX_BookingServices_transactionID",
                table: "BookingServices");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_transactionID",
                table: "Bookings");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "53511dd3-e47a-4858-b96c-eddba78bdf46");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "952952ce-e13f-4c7d-aeb4-27554578a86d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e268cc3e-69a1-435b-99fa-6baba1159b91");

            migrationBuilder.DropColumn(
                name: "transactionID",
                table: "BookingServices");

            migrationBuilder.DropColumn(
                name: "transactionID",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "BookingDetails");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Rooms",
                newName: "isUsed");

            migrationBuilder.RenameColumn(
                name: "RoomAvailabilityID",
                table: "Rooms",
                newName: "RoomTypesID");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_RoomAvailabilityID",
                table: "Rooms",
                newName: "IX_Rooms_RoomTypesID");

            migrationBuilder.RenameColumn(
                name: "RoomTypesID",
                table: "BookingDetails",
                newName: "RoomID");

            migrationBuilder.RenameIndex(
                name: "IX_BookingDetails_RoomTypesID",
                table: "BookingDetails",
                newName: "IX_BookingDetails_RoomID");

            migrationBuilder.AddColumn<int>(
                name: "BookingID",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BookingServicesID",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isActive",
                table: "Rooms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "bookingServiceDeposit",
                table: "BookingServices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "remainingBalance",
                table: "BookingServices",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "remainingBalance",
                table: "Bookings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0325a81e-11ae-4530-a920-fcdc4517d69d", null, "Admin", "ADMIN" },
                    { "94c6652b-13e0-40f6-9f08-46effec1271f", null, "Customer", "CUSTOMER" },
                    { "b03e23bc-f35e-406f-b1de-a71953b81cdc", null, "Owner", "OWNER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BookingID",
                table: "Transactions",
                column: "BookingID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BookingServicesID",
                table: "Transactions",
                column: "BookingServicesID");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetails_Rooms_RoomID",
                table: "BookingDetails",
                column: "RoomID",
                principalTable: "Rooms",
                principalColumn: "RoomID");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomTypes_RoomTypesID",
                table: "Rooms",
                column: "RoomTypesID",
                principalTable: "RoomTypes",
                principalColumn: "RoomTypesID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_BookingServices_BookingServicesID",
                table: "Transactions",
                column: "BookingServicesID",
                principalTable: "BookingServices",
                principalColumn: "BookingServicesID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Bookings_BookingID",
                table: "Transactions",
                column: "BookingID",
                principalTable: "Bookings",
                principalColumn: "BookingID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingDetails_Rooms_RoomID",
                table: "BookingDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_RoomTypes_RoomTypesID",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_BookingServices_BookingServicesID",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Bookings_BookingID",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_BookingID",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_BookingServicesID",
                table: "Transactions");

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

            migrationBuilder.DropColumn(
                name: "BookingID",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "BookingServicesID",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "isActive",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "bookingServiceDeposit",
                table: "BookingServices");

            migrationBuilder.DropColumn(
                name: "remainingBalance",
                table: "BookingServices");

            migrationBuilder.DropColumn(
                name: "remainingBalance",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "isUsed",
                table: "Rooms",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "RoomTypesID",
                table: "Rooms",
                newName: "RoomAvailabilityID");

            migrationBuilder.RenameIndex(
                name: "IX_Rooms_RoomTypesID",
                table: "Rooms",
                newName: "IX_Rooms_RoomAvailabilityID");

            migrationBuilder.RenameColumn(
                name: "RoomID",
                table: "BookingDetails",
                newName: "RoomTypesID");

            migrationBuilder.RenameIndex(
                name: "IX_BookingDetails_RoomID",
                table: "BookingDetails",
                newName: "IX_BookingDetails_RoomTypesID");

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

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "BookingDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RoomAvailabilities",
                columns: table => new
                {
                    RoomAvailabilityID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomTypesID = table.Column<int>(type: "int", nullable: true),
                    AvailableRooms = table.Column<int>(type: "int", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RemainingRooms = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    UsedRooms = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAvailabilities", x => x.RoomAvailabilityID);
                    table.ForeignKey(
                        name: "FK_RoomAvailabilities_RoomTypes_RoomTypesID",
                        column: x => x.RoomTypesID,
                        principalTable: "RoomTypes",
                        principalColumn: "RoomTypesID");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "53511dd3-e47a-4858-b96c-eddba78bdf46", null, "Admin", "ADMIN" },
                    { "952952ce-e13f-4c7d-aeb4-27554578a86d", null, "Owner", "OWNER" },
                    { "e268cc3e-69a1-435b-99fa-6baba1159b91", null, "Customer", "CUSTOMER" }
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

            migrationBuilder.CreateIndex(
                name: "IX_RoomAvailabilities_RoomTypesID",
                table: "RoomAvailabilities",
                column: "RoomTypesID");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingDetails_RoomTypes_RoomTypesID",
                table: "BookingDetails",
                column: "RoomTypesID",
                principalTable: "RoomTypes",
                principalColumn: "RoomTypesID");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_RoomAvailabilities_RoomAvailabilityID",
                table: "Rooms",
                column: "RoomAvailabilityID",
                principalTable: "RoomAvailabilities",
                principalColumn: "RoomAvailabilityID");
        }
    }
}
