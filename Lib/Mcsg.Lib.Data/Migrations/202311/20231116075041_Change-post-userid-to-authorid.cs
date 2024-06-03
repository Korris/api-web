using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202311
{
    /// <inheritdoc />
    public partial class Changepostuseridtoauthorid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubPosts_Users_UserId",
                table: "SubPosts");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "SubPosts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "SubPosts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "Posts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SubPosts_Users_UserId",
                table: "SubPosts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubPosts_Users_UserId",
                table: "SubPosts");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "SubPosts");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Posts");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "SubPosts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SubPosts_Users_UserId",
                table: "SubPosts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
