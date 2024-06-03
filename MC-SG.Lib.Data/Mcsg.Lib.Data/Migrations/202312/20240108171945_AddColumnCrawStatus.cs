using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202312
{
    /// <inheritdoc />
    public partial class AddColumnCrawStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShouldCraw",
                table: "CrawComics");

            migrationBuilder.AddColumn<int>(
                name: "CrawStatus",
                table: "CrawComics",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CrawStatus",
                table: "CrawComics");

            migrationBuilder.AddColumn<bool>(
                name: "ShouldCraw",
                table: "CrawComics",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
