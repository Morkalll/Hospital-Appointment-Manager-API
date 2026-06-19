using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TPI_2026.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FilteredUniqueIndexesForSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_Email",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_EmployeeNumber",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_Credential",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_Dni",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_User_EmployeeNumber",
                table: "User",
                column: "EmployeeNumber",
                unique: true,
                filter: "[EmployeeNumber] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_User_Credential",
                table: "User",
                column: "Credential",
                unique: true,
                filter: "[Credential] IS NOT NULL AND [IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_User_Dni",
                table: "User",
                column: "Dni",
                unique: true,
                filter: "[Dni] IS NOT NULL AND [IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_User_Email",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_EmployeeNumber",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_Credential",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_Dni",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_EmployeeNumber",
                table: "User",
                column: "EmployeeNumber",
                unique: true,
                filter: "[EmployeeNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_User_Credential",
                table: "User",
                column: "Credential",
                unique: true,
                filter: "[Credential] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_User_Dni",
                table: "User",
                column: "Dni",
                unique: true,
                filter: "[Dni] IS NOT NULL");
        }
    }
}
