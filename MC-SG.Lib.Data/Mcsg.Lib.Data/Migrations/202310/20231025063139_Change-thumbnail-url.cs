using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202310
{
    /// <inheritdoc />
    public partial class Changethumbnailurl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThubnailId",
                table: "SubPosts");

            migrationBuilder.DropColumn(
                name: "ThubnailId",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "ThubnailUrl",
                table: "Posts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThubnailUrl",
                table: "Posts");

            migrationBuilder.AddColumn<Guid>(
                name: "ThubnailId",
                table: "SubPosts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ThubnailId",
                table: "Posts",
                type: "uuid",
                nullable: true);
        }
    }
}
