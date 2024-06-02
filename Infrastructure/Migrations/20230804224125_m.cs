using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class m : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    { "83f2b981-06ec-4fa8-8fe7-66c5e4e7a739", "97b60fa3-e22e-43aa-9d87-d812006734e1", "Administrator", "ADMINISTRATOR" },
                    { "e41eaa20-7a44-40e6-8cca-0279cd87bc66", "8a3b47e6-3b2a-4ac6-9e3f-b3db04d8fa88", "Visitor", "VISITOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
