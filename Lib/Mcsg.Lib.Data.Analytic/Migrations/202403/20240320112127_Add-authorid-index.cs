using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Analytic.Migrations._202403
{
    /// <inheritdoc />
    public partial class Addauthoridindex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubPostMappingss");

            migrationBuilder.DropIndex(
                name: "IX_UserViewPosts_PostId_SubPostId_UserId_UserHashString_Create~",
                table: "UserViewPosts");

            migrationBuilder.CreateIndex(
                name: "IX_UserViewPosts_PostId_SubPostId_AuthorId_UserId_UserHashStri~",
                table: "UserViewPosts",
                columns: new[] { "PostId", "SubPostId", "AuthorId", "UserId", "UserHashString", "CreatedDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserViewPosts_PostId_SubPostId_AuthorId_UserId_UserHashStri~",
                table: "UserViewPosts");

            migrationBuilder.CreateTable(
                name: "SubPostMappingss",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    PostType = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubPostMappingss", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserViewPosts_PostId_SubPostId_UserId_UserHashString_Create~",
                table: "UserViewPosts",
                columns: new[] { "PostId", "SubPostId", "UserId", "UserHashString", "CreatedDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubPostMappingss_Id_PostId_UserId_CreatedDate",
                table: "SubPostMappingss",
                columns: new[] { "Id", "PostId", "UserId", "CreatedDate" },
                unique: true);
        }
    }
}
