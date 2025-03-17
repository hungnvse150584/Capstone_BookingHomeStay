using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0705219e-f526-4f4e-b4c8-497f32976489");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0e32d3ad-2548-43ae-85fa-35c16970b6fe");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a6b85be8-b3b8-480e-a881-409333ddead1");

            migrationBuilder.DropColumn(
                name: "RentPrice",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "RoomTypes");

            migrationBuilder.DropColumn(
                name: "RentPrice",
                table: "HomeStayRentals");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "HomeStayRentals");

            migrationBuilder.AddColumn<int>(
                name: "CancellationID",
                table: "HomeStays",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "UnitPrice",
                table: "BookingDetails",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "CancelPolicy",
                columns: table => new
                {
                    CancellationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DayBeforeCancel = table.Column<int>(type: "int", nullable: false),
                    RefundPercentage = table.Column<double>(type: "float", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HomeStayID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancelPolicy", x => x.CancellationID);
                    table.ForeignKey(
                        name: "FK_CancelPolicy_HomeStays_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStays",
                        principalColumn: "HomeStayID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    PricingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitPrice = table.Column<double>(type: "float", nullable: false),
                    RentPrice = table.Column<double>(type: "float", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    HomeStayRentalID = table.Column<int>(type: "int", nullable: true),
                    RoomTypesID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.PricingID);
                    table.ForeignKey(
                        name: "FK_Prices_HomeStayRentals_HomeStayRentalID",
                        column: x => x.HomeStayRentalID,
                        principalTable: "HomeStayRentals",
                        principalColumn: "HomeStayRentalID");
                    table.ForeignKey(
                        name: "FK_Prices_RoomTypes_RoomTypesID",
                        column: x => x.RoomTypesID,
                        principalTable: "RoomTypes",
                        principalColumn: "RoomTypesID");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "b8be1a2a-88d5-4258-a1ec-e3c3d71ab871", null, "Owner", "OWNER" },
                    { "c4ccc551-6758-4825-8e05-a678444d6046", null, "Admin", "ADMIN" },
                    { "ddfe4eaa-bbd5-438c-9816-c81b432b2487", null, "Customer", "CUSTOMER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CancelPolicy_HomeStayID",
                table: "CancelPolicy",
                column: "HomeStayID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prices_HomeStayRentalID",
                table: "Prices",
                column: "HomeStayRentalID");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_RoomTypesID",
                table: "Prices",
                column: "RoomTypesID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CancelPolicy");

            migrationBuilder.DropTable(
                name: "Prices");

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

            migrationBuilder.DropColumn(
                name: "CancellationID",
                table: "HomeStays");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "BookingDetails");

            migrationBuilder.AddColumn<double>(
                name: "RentPrice",
                table: "RoomTypes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "UnitPrice",
                table: "RoomTypes",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RentPrice",
                table: "HomeStayRentals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "UnitPrice",
                table: "HomeStayRentals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0705219e-f526-4f4e-b4c8-497f32976489", null, "Customer", "CUSTOMER" },
                    { "0e32d3ad-2548-43ae-85fa-35c16970b6fe", null, "Owner", "OWNER" },
                    { "a6b85be8-b3b8-480e-a881-409333ddead1", null, "Admin", "ADMIN" }
                });
        }
    }
}
