using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202403
{
    /// <inheritdoc />
    public partial class UpdateCrawComicChapterAndPostLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HashId",
                table: "PostLinks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExternalLastedUpdateDate",
                table: "CrawComicChapters",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HashId",
                table: "PostLinks");

            migrationBuilder.DropColumn(
                name: "ExternalLastedUpdateDate",
                table: "CrawComicChapters");
        }
    }
}
