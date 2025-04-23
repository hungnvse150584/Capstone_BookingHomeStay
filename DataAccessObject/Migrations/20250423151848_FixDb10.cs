using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessObject.Migrations
{
    /// <inheritdoc />
    public partial class FixDb10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Rating");

            migrationBuilder.AddColumn<double>(
                name: "CleaningRate",
                table: "Rating",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Rating",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "FacilityRate",
                table: "Rating",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ServiceRate",
                table: "Rating",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SumRate",
                table: "Rating",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "ImageRatings",
                columns: table => new
                {
                    ImageRatingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RatingID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageRatings", x => x.ImageRatingID);
                    table.ForeignKey(
                        name: "FK_ImageRatings_Rating_RatingID",
                        column: x => x.RatingID,
                        principalTable: "Rating",
                        principalColumn: "RatingID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageRatings_RatingID",
                table: "ImageRatings",
                column: "RatingID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageRatings");

            migrationBuilder.DropColumn(
                name: "CleaningRate",
                table: "Rating");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Rating");

            migrationBuilder.DropColumn(
                name: "FacilityRate",
                table: "Rating");

            migrationBuilder.DropColumn(
                name: "ServiceRate",
                table: "Rating");

            migrationBuilder.DropColumn(
                name: "SumRate",
                table: "Rating");

            migrationBuilder.AddColumn<int>(
                name: "Rate",
                table: "Rating",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
