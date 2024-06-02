using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedingRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
             "Roles",
             columns: new[] {"Name", "NormalizedName" },
             values: new object[] { "Visitor", "VISITOR" });

            migrationBuilder.InsertData(
                "Roles",
                columns: new[] {"Name", "NormalizedName" },
                values: new object[] { "Adminstration", "ADMINISTRATION" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
