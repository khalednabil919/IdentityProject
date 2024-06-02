using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class region : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "9067ace1-2d30-47da-aecd-1aaef242ee5e");

            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "c38ee270-dae7-47d0-8b16-9c27726a9296");

            migrationBuilder.CreateTable(
                name: "region",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Done = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_region", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "security",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "991846d2-f1bd-47df-b30e-a1efbbab6981", "a7474f3c-68eb-4207-ba3d-d92a21fe4fc6", "Visitor", "VISITOR" },
                    { "ad8a10bd-c07d-481b-aaad-3bcc9941a7c0", "11cdad03-5027-4513-b67f-d7f77b5e30f2", "Administrator", "ADMINISTRATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "region");

            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "991846d2-f1bd-47df-b30e-a1efbbab6981");

            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "ad8a10bd-c07d-481b-aaad-3bcc9941a7c0");

            migrationBuilder.InsertData(
                schema: "security",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "9067ace1-2d30-47da-aecd-1aaef242ee5e", "5dc47c43-294d-4608-a389-ee627c573a79", "Visitor", "VISITOR" },
                    { "c38ee270-dae7-47d0-8b16-9c27726a9296", "e5a0b480-56ac-4828-9604-3096143f5b59", "Administrator", "ADMINISTRATOR" }
                });
        }
    }
}
