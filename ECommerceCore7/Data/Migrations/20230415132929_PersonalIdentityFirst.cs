using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace ECommerceCore7.Data.Migrations
{
    /// <inheritdoc />
    public partial class PersonalIdentityFirst : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreRegisterations",
                columns: table => new
                {
                    RegisterationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Confirmed = table.Column<bool>(type: "bit", nullable: false),
                    ExpireTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthenticationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthenticationKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreRegisterations", x => x.RegisterationID);
                });

            migrationBuilder.CreateTable(
                name: "UserLoginWithSms",
                columns: table => new
                {
                    LoginWithSmsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AuthenticationCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AuthenticationKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginWithSms", x => x.LoginWithSmsID);
                    table.ForeignKey(
                        name: "FK_UserLoginWithSms_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginWithSms_UserID",
                table: "UserLoginWithSms",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreRegisterations");

            migrationBuilder.DropTable(
                name: "UserLoginWithSms");
        }
    }
}
