using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TPI_2026.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixHistoriesAndExceptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_User_EmployeeNumber",
                table: "User",
                column: "EmployeeNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_EmployeeNumber",
                table: "User");
        }
    }
}
