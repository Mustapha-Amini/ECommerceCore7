using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerceCore7.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedInventoryAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryActionTypes",
                columns: table => new
                {
                    InventoryActionID = table.Column<int>(type: "int", nullable: false),
                    InventoryActionTypeTitle = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryActionTypes", x => x.InventoryActionID);
                });

            migrationBuilder.CreateTable(
                name: "InventoryActions",
                columns: table => new
                {
                    InventoryActionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryActionTypeID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    InventoryActionComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InventoryActionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InventoryActionCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryActions", x => x.InventoryActionID);
                    table.ForeignKey(
                        name: "FK_InventoryActions_InventoryActionTypes_InventoryActionTypeID",
                        column: x => x.InventoryActionTypeID,
                        principalTable: "InventoryActionTypes",
                        principalColumn: "InventoryActionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryActions_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK_InventoryActions_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryActions_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.InsertData(
                table: "InventoryActionTypes",
                columns: new[] { "InventoryActionID", "InventoryActionTypeTitle" },
                values: new object[,]
                {
                    { 1, "ورود به انبار" },
                    { 2, "خروج از انبار" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryActions_InventoryActionTypeID",
                table: "InventoryActions",
                column: "InventoryActionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryActions_OrderID",
                table: "InventoryActions",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryActions_ProductID",
                table: "InventoryActions",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryActions_UserID",
                table: "InventoryActions",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryActions");

            migrationBuilder.DropTable(
                name: "InventoryActionTypes");
        }
    }
}
