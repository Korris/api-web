using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202310
{
    /// <inheritdoc />
    public partial class ModifyResourceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Resources_ThubnailId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_SubPosts_Resources_ThubnailId",
                table: "SubPosts");

            migrationBuilder.DropIndex(
                name: "IX_SubPosts_ThubnailId",
                table: "SubPosts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_ThubnailId",
                table: "Posts");

            migrationBuilder.AddColumn<string>(
                name: "HashId",
                table: "Resources",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "SubPostId",
                table: "Resources",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_HashId",
                table: "Resources",
                column: "HashId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_SubPostId",
                table: "Resources",
                column: "SubPostId");

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_SubPosts_SubPostId",
                table: "Resources",
                column: "SubPostId",
                principalTable: "SubPosts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resources_SubPosts_SubPostId",
                table: "Resources");

            migrationBuilder.DropIndex(
                name: "IX_Resources_HashId",
                table: "Resources");

            migrationBuilder.DropIndex(
                name: "IX_Resources_SubPostId",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "HashId",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "SubPostId",
                table: "Resources");

            migrationBuilder.CreateIndex(
                name: "IX_SubPosts_ThubnailId",
                table: "SubPosts",
                column: "ThubnailId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ThubnailId",
                table: "Posts",
                column: "ThubnailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Resources_ThubnailId",
                table: "Posts",
                column: "ThubnailId",
                principalTable: "Resources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubPosts_Resources_ThubnailId",
                table: "SubPosts",
                column: "ThubnailId",
                principalTable: "Resources",
                principalColumn: "Id");
        }
    }
}
