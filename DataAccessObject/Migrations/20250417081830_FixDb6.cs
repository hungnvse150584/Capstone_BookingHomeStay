using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteAt",
                table: "Services",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteAt",
                table: "RoomTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteAt",
                table: "Rooms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteAt",
                table: "HomeStays",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleteAt",
                table: "HomeStayRentals",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PricingHistories",
                columns: table => new
                {
                    HistoryPricingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PricingID = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitPrice = table.Column<double>(type: "float", nullable: false),
                    RentPrice = table.Column<double>(type: "float", nullable: false),
                    Percentage = table.Column<double>(type: "float", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    DayType = table.Column<int>(type: "int", nullable: false),
                    HomeStayRentalID = table.Column<int>(type: "int", nullable: true),
                    RoomTypesID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingHistories", x => x.HistoryPricingID);
                    table.ForeignKey(
                        name: "FK_PricingHistories_HomeStayRentals_HomeStayRentalID",
                        column: x => x.HomeStayRentalID,
                        principalTable: "HomeStayRentals",
                        principalColumn: "HomeStayRentalID");
                    table.ForeignKey(
                        name: "FK_PricingHistories_Prices_PricingID",
                        column: x => x.PricingID,
                        principalTable: "Prices",
                        principalColumn: "PricingID");
                    table.ForeignKey(
                        name: "FK_PricingHistories_RoomTypes_RoomTypesID",
                        column: x => x.RoomTypesID,
                        principalTable: "RoomTypes",
                        principalColumn: "RoomTypesID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PricingHistories_HomeStayRentalID",
                table: "PricingHistories",
                column: "HomeStayRentalID");

            migrationBuilder.CreateIndex(
                name: "IX_PricingHistories_PricingID",
                table: "PricingHistories",
                column: "PricingID");

            migrationBuilder.CreateIndex(
                name: "IX_PricingHistories_RoomTypesID",
                table: "PricingHistories",
                column: "RoomTypesID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricingHistories");

            migrationBuilder.DropColumn(
                name: "DeleteAt",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "DeleteAt",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "DeleteAt",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "DeleteAt",
                table: "HomeStays");

            migrationBuilder.DropColumn(
                name: "DeleteAt",
                table: "HomeStayRentals");
        }
    }
}
