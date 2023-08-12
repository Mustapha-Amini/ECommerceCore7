using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceCore7.Data.Migrations
{
    /// <inheritdoc />
    public partial class FinishFasl20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ProductIsActive",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProductMetaDescription",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductMetaTags",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageMetaDescription",
                table: "Pages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageMetaTags",
                table: "Pages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PageShowLinkInTopMenuInGroup",
                table: "Pages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductIsActive",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductMetaDescription",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductMetaTags",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PageMetaDescription",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "PageMetaTags",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "PageShowLinkInTopMenuInGroup",
                table: "Pages");
        }
    }
}
