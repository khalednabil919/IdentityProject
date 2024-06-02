using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                schema: "security",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "969a1f5f-6a47-4832-a6a7-9570fb60c2fb", "d8c7673a-0836-4d4e-8098-c228c53491d5", "Administrator", "ADMINISTRATOR" },
                    { "b6c65785-bb7d-41cd-943c-24a0a05431c3", "19f9e2ea-777e-4e35-bee7-8a574c358c05", "Visitor", "VISITOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "969a1f5f-6a47-4832-a6a7-9570fb60c2fb");

            migrationBuilder.DeleteData(
                schema: "security",
                table: "Roles",
                keyColumn: "Id",
                keyValue: "b6c65785-bb7d-41cd-943c-24a0a05431c3");

            migrationBuilder.InsertData(
                schema: "security",
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "402786fe-b997-4c90-b7fb-823b611171d7", "eea3446f-14dd-4478-b9bb-8df2b75c0d91", "Visitor", "VISITOR" },
                    { "b5b1675d-040a-49c4-b7d7-38e11dc4f502", "6d42002d-39c5-4840-af26-8716ce399dd0", "Administrator", "ADMINISTRATOR" }
                });
        }
    }
}
