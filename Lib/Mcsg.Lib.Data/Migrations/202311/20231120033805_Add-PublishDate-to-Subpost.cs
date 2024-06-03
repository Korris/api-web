using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202311
{
    /// <inheritdoc />
    public partial class AddPublishDatetoSubpost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishDate",
                table: "Posts");

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDate",
                table: "SubPosts",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishDate",
                table: "SubPosts");

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDate",
                table: "Posts",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
