using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace ECommerceCore7.Data.Migrations
{
    /// <inheritdoc />
    public partial class addWriter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PageImageFilename",
                table: "Pages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PageLastModified",
                table: "Pages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "Pages",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.CreateIndex(
                name: "IX_Pages_UserID",
                table: "Pages",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Users_UserID",
                table: "Pages",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Users_UserID",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Pages_UserID",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "PageImageFilename",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "PageLastModified",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "Pages");
        }
    }
}
