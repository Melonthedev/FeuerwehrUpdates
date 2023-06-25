using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeuerwehrUpdates.Migrations
{
    /// <inheritdoc />
    public partial class DocumentUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentUrl",
                table: "Einsaetze",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentUrl",
                table: "Einsaetze");
        }
    }
}
