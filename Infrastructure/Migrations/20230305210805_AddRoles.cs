using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
