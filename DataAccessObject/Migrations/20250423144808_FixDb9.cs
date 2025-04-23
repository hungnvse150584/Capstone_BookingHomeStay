using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomChangeHistories_AspNetUsers_AccountId",
                table: "RoomChangeHistories");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "RoomChangeHistories");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "RoomChangeHistories",
                newName: "AccountID");

            migrationBuilder.RenameIndex(
                name: "IX_RoomChangeHistories_AccountId",
                table: "RoomChangeHistories",
                newName: "IX_RoomChangeHistories_AccountID");

            migrationBuilder.AlterColumn<string>(
                name: "AccountID",
                table: "RoomChangeHistories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomChangeHistories_AspNetUsers_AccountID",
                table: "RoomChangeHistories",
                column: "AccountID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomChangeHistories_AspNetUsers_AccountID",
                table: "RoomChangeHistories");

            migrationBuilder.RenameColumn(
                name: "AccountID",
                table: "RoomChangeHistories",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomChangeHistories_AccountID",
                table: "RoomChangeHistories",
                newName: "IX_RoomChangeHistories_AccountId");

            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "RoomChangeHistories",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ChangedBy",
                table: "RoomChangeHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomChangeHistories_AspNetUsers_AccountId",
                table: "RoomChangeHistories",
                column: "AccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
