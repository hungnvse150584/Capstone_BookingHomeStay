using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Staffs_StaffID",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_StaffID",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "StaffID",
                table: "Transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StaffID",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_StaffID",
                table: "Transactions",
                column: "StaffID");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Staffs_StaffID",
                table: "Transactions",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "StaffID");
        }
    }
}
