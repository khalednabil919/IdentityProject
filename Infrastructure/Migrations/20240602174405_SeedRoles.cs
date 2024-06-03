using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
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
                    { "ab42ed04-acc8-40e2-9c3d-c2865ae3756c", "bd8dd589-a510-4fe6-9d63-2201a84985bf", "Administrator", "ADMINISTRATOR" },
                    { "e1c1e2a6-44b5-4b07-a520-f2772732e4f1", "cd9ddc08-10a9-49c5-8c14-1f4b4e38d665", "Visitor", "VISITOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "ab42ed04-acc8-40e2-9c3d-c2865ae3756c");

            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "e1c1e2a6-44b5-4b07-a520-f2772732e4f1");
        }
    }
}
