using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountConversations");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "User1ID",
                table: "Conversations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "User2ID",
                table: "Conversations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_AccountId",
                table: "Messages",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_User1ID",
                table: "Conversations",
                column: "User1ID");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_User2ID",
                table: "Conversations",
                column: "User2ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_User1ID",
                table: "Conversations",
                column: "User1ID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_AspNetUsers_User2ID",
                table: "Conversations",
                column: "User2ID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_AccountId",
                table: "Messages",
                column: "AccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_User1ID",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_AspNetUsers_User2ID",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_AccountId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_AccountId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_User1ID",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_User2ID",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "User1ID",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "User2ID",
                table: "Conversations");

            migrationBuilder.CreateTable(
                name: "AccountConversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConversationID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountConversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountConversations_AspNetUsers_AccountID",
                        column: x => x.AccountID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountConversations_Conversations_ConversationID",
                        column: x => x.ConversationID,
                        principalTable: "Conversations",
                        principalColumn: "ConversationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountConversations_AccountID",
                table: "AccountConversations",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_AccountConversations_ConversationID",
                table: "AccountConversations",
                column: "ConversationID");
        }
    }
}
