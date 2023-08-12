using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceCore7.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedSliderImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SliderImages",
                columns: table => new
                {
                    SliderImageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SlideImageMainTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SlideImageShortTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SlideImageShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SlideImageFilename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SliderImageEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SliderImages", x => x.SliderImageID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SliderImages");
        }
    }
}
