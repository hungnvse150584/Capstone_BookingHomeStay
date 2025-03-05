using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b5d12ae6-9fe5-44da-b1fc-09451d56f31b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "da90f73d-7a4d-48bc-a5c5-33a4815e367b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "db8d73cb-14a3-4c4f-8e29-438c1ffdef9a");

            migrationBuilder.CreateTable(
                name: "ImageCultureExperiences",
                columns: table => new
                {
                    ImageCultureExperiencesID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CultureExperienceID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageCultureExperiences", x => x.ImageCultureExperiencesID);
                    table.ForeignKey(
                        name: "FK_ImageCultureExperiences_CultureExperiences_CultureExperienceID",
                        column: x => x.CultureExperienceID,
                        principalTable: "CultureExperiences",
                        principalColumn: "CultureExperienceID");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "087fbdba-d016-41a8-988e-e40786ddb262", null, "Customer", "CUSTOMER" },
                    { "71803413-785b-4df2-8f05-c63c63ed679c", null, "Admin", "ADMIN" },
                    { "becdd5ce-7c86-4b98-8b25-3d5b0fd25eb5", null, "Owner", "OWNER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageCultureExperiences_CultureExperienceID",
                table: "ImageCultureExperiences",
                column: "CultureExperienceID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageCultureExperiences");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "087fbdba-d016-41a8-988e-e40786ddb262");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "71803413-785b-4df2-8f05-c63c63ed679c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "becdd5ce-7c86-4b98-8b25-3d5b0fd25eb5");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "b5d12ae6-9fe5-44da-b1fc-09451d56f31b", null, "Customer", "CUSTOMER" },
                    { "da90f73d-7a4d-48bc-a5c5-33a4815e367b", null, "Owner", "OWNER" },
                    { "db8d73cb-14a3-4c4f-8e29-438c1ffdef9a", null, "Admin", "ADMIN" }
                });
        }
    }
}
