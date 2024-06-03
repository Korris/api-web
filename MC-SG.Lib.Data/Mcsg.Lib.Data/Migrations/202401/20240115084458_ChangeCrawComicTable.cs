using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202401
{
    /// <inheritdoc />
    public partial class ChangeCrawComicTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChapterNumber",
                table: "CrawComicChapters");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "CrawComicChapters",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "CrawComicChapters");

            migrationBuilder.AddColumn<double>(
                name: "ChapterNumber",
                table: "CrawComicChapters",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
