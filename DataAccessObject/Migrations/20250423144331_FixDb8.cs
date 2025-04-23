using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomChangeHistories",
                columns: table => new
                {
                    RoomChangeHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingDetailID = table.Column<int>(type: "int", nullable: false),
                    OldRoomID = table.Column<int>(type: "int", nullable: true),
                    NewRoomID = table.Column<int>(type: "int", nullable: true),
                    UsagedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomChangeHistories", x => x.RoomChangeHistoryID);
                    table.ForeignKey(
                        name: "FK_RoomChangeHistories_AspNetUsers_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoomChangeHistories_BookingDetails_BookingDetailID",
                        column: x => x.BookingDetailID,
                        principalTable: "BookingDetails",
                        principalColumn: "BookingDetailID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomChangeHistories_AccountId",
                table: "RoomChangeHistories",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomChangeHistories_BookingDetailID",
                table: "RoomChangeHistories",
                column: "BookingDetailID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomChangeHistories");
        }
    }
}
