using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "06afa541-360e-413a-875a-0bdb1f66ddb4");

            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "15427634-8f57-495e-89f7-cd783d202476");

            migrationBuilder.CreateTable(
                name: "RefreshTokenDevCreed",
                schema: "security",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokenDevCreed", x => new { x.UserId, x.Id });
                    table.ForeignKey(
                        name: "FK_RefreshTokenDevCreed_Users_UserId",
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
                    { "c9b20a50-0059-49b0-91fe-525b8d976909", "79303c0a-cb71-4c1c-99b5-e7c1ac9d264f", "Administrator", "ADMINISTRATOR" },
                    { "e6e66b24-c4ee-42d0-adc8-e860d9b7fa7a", "b0bce26b-3309-43eb-a2e8-9e6483230b9c", "Visitor", "VISITOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokenDevCreed",
                schema: "security");

            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "c9b20a50-0059-49b0-91fe-525b8d976909");

            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "e6e66b24-c4ee-42d0-adc8-e860d9b7fa7a");

            migrationBuilder.InsertData(
                schema: "security",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "06afa541-360e-413a-875a-0bdb1f66ddb4", "b34978b6-e3a0-4458-8fa9-78e3adb8e57f", "Administrator", "ADMINISTRATOR" },
                    { "15427634-8f57-495e-89f7-cd783d202476", "c4ed1b6e-dbb6-42d0-a54f-f2af472cc16c", "Visitor", "VISITOR" }
                });
        }
    }
}
