using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TPI_2026.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MigrationAfterNamespacesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Adress",
                table: "User",
                newName: "Address");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "User",
                newName: "Adress");
        }
    }
}
