using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HomeStayID",
                table: "Conversations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_HomeStayID",
                table: "Conversations",
                column: "HomeStayID");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_HomeStays_HomeStayID",
                table: "Conversations",
                column: "HomeStayID",
                principalTable: "HomeStays",
                principalColumn: "HomeStayID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_HomeStays_HomeStayID",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_HomeStayID",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "HomeStayID",
                table: "Conversations");
        }
    }
}
