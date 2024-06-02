using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seeding2Roles : Migration
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
                    { "2b8da07a-d914-4036-a66f-c5bb4890d620", "c41f0d91-e8c9-4a05-b7ba-a300c2b83f9c", "Administrator", "ADMINISTRATOR" },
                    { "3135210b-e627-4214-ab59-50ba268a3c9b", "b7564c96-8817-472a-9d59-c5237da9a576", "Visitor", "VISITOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "2b8da07a-d914-4036-a66f-c5bb4890d620");

            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "3135210b-e627-4214-ab59-50ba268a3c9b");
        }
    }
}
