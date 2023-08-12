using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceCore7.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedStaticPages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PageShowLinkDirectlyInTopMenu",
                table: "Pages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PageShowLinkInFooter",
                table: "Pages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageShowLinkDirectlyInTopMenu",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "PageShowLinkInFooter",
                table: "Pages");
        }
    }
}
