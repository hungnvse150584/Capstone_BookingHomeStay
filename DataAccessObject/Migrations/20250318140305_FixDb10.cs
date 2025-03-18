using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "33910f06-8466-4b84-b41e-afd4c490a51c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ffa7a99-5bd4-4eda-9144-468c8924ed80");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a8df72cb-bf34-42b1-af03-f58a3432bb47");

            migrationBuilder.CreateTable(
                name: "ImageHomeStays",
                columns: table => new
                {
                    ImageHomeStayID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomeStayID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageHomeStays", x => x.ImageHomeStayID);
                    table.ForeignKey(
                        name: "FK_ImageHomeStays_HomeStays_HomeStayID",
                        column: x => x.HomeStayID,
                        principalTable: "HomeStays",
                        principalColumn: "HomeStayID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageHomeStays_HomeStayID",
                table: "ImageHomeStays",
                column: "HomeStayID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageHomeStays");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "33910f06-8466-4b84-b41e-afd4c490a51c", null, "Customer", "CUSTOMER" },
                    { "5ffa7a99-5bd4-4eda-9144-468c8924ed80", null, "Admin", "ADMIN" },
                    { "a8df72cb-bf34-42b1-af03-f58a3432bb47", null, "Owner", "OWNER" }
                });
        }
    }
}
