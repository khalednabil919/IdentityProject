using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addRefreshTokenDevCreed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokenDevCreed_Users_UserId",
                schema: "security",
                table: "RefreshTokenDevCreed");

            migrationBuilder.DropTable(
                name: "refreshToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokenDevCreed",
                schema: "security",
                table: "RefreshTokenDevCreed");

            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "83f2b981-06ec-4fa8-8fe7-66c5e4e7a739");

            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "e41eaa20-7a44-40e6-8cca-0279cd87bc66");

            migrationBuilder.RenameTable(
                name: "RefreshTokenDevCreed",
                schema: "security",
                newName: "refreshTokenDevCreed");

            migrationBuilder.AddPrimaryKey(
                name: "PK_refreshTokenDevCreed",
                table: "refreshTokenDevCreed",
                column: "Id");

            migrationBuilder.InsertData(
                schema: "security",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "402786fe-b997-4c90-b7fb-823b611171d7", "eea3446f-14dd-4478-b9bb-8df2b75c0d91", "Visitor", "VISITOR" },
                    { "b5b1675d-040a-49c4-b7d7-38e11dc4f502", "6d42002d-39c5-4840-af26-8716ce399dd0", "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_refreshTokenDevCreed_UserId",
                table: "refreshTokenDevCreed",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_refreshTokenDevCreed_Users_UserId",
                table: "refreshTokenDevCreed",
                column: "UserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refreshTokenDevCreed_Users_UserId",
                table: "refreshTokenDevCreed");

            migrationBuilder.DropPrimaryKey(
                name: "PK_refreshTokenDevCreed",
                table: "refreshTokenDevCreed");

            migrationBuilder.DropIndex(
                name: "IX_refreshTokenDevCreed_UserId",
                table: "refreshTokenDevCreed");

            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "402786fe-b997-4c90-b7fb-823b611171d7");

            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "b5b1675d-040a-49c4-b7d7-38e11dc4f502");

            migrationBuilder.RenameTable(
                name: "refreshTokenDevCreed",
                newName: "RefreshTokenDevCreed",
                newSchema: "security");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokenDevCreed",
                schema: "security",
                table: "RefreshTokenDevCreed",
                columns: new[] { "UserId", "Id" });

            migrationBuilder.CreateTable(
                name: "refreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    JwtId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refreshToken_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "security",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "security",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "83f2b981-06ec-4fa8-8fe7-66c5e4e7a739", "97b60fa3-e22e-43aa-9d87-d812006734e1", "Administrator", "ADMINISTRATOR" },
                    { "e41eaa20-7a44-40e6-8cca-0279cd87bc66", "8a3b47e6-3b2a-4ac6-9e3f-b3db04d8fa88", "Visitor", "VISITOR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_refreshToken_UserId",
                table: "refreshToken",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokenDevCreed_Users_UserId",
                schema: "security",
                table: "RefreshTokenDevCreed",
                column: "UserId",
                principalSchema: "security",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
