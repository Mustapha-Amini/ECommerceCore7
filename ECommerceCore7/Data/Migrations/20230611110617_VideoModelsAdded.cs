using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerceCore7.Data.Migrations
{
    /// <inheritdoc />
    public partial class VideoModelsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VideoTypes",
                columns: table => new
                {
                    VideoTypeID = table.Column<int>(type: "int", nullable: false),
                    VideoTypeTitle = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoTypes", x => x.VideoTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    VideoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VideoTypeID = table.Column<int>(type: "int", nullable: false),
                    VideoName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    PageID = table.Column<int>(type: "int", nullable: true),
                    VideoDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VideoComments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.VideoID);
                    table.ForeignKey(
                        name: "FK_Videos_Pages_PageID",
                        column: x => x.PageID,
                        principalTable: "Pages",
                        principalColumn: "PageID");
                    table.ForeignKey(
                        name: "FK_Videos_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID");
                    table.ForeignKey(
                        name: "FK_Videos_VideoTypes_VideoTypeID",
                        column: x => x.VideoTypeID,
                        principalTable: "VideoTypes",
                        principalColumn: "VideoTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "VideoTypes",
                columns: new[] { "VideoTypeID", "VideoTypeTitle" },
                values: new object[,]
                {
                    { 1, "ویدئو برای محصول" },
                    { 2, "ویدئو برای صفحه" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_PageID",
                table: "Videos",
                column: "PageID");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ProductID",
                table: "Videos",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_VideoTypeID",
                table: "Videos",
                column: "VideoTypeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "VideoTypes");
        }
    }
}
