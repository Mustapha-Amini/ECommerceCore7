using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace ECommerceCore7.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedPagesSliderInHomeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ProductLastModificationDate",
                table: "Products",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductLastModificationDate",
                table: "Products");
        }
    }
}
