using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Analytic.Migrations
{
    /// <inheritdoc />
    public partial class InitData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserViewPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostType = table.Column<int>(type: "integer", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubPostId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserType = table.Column<int>(type: "integer", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    BrowserAgent = table.Column<string>(type: "text", nullable: true),
                    UserHashString = table.Column<string>(type: "text", nullable: true),
                    TimeSpan = table.Column<long>(type: "bigint", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserViewPosts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserViewPosts_PostId_SubPostId_AuthorId_UserId_UserHashStri~",
                table: "UserViewPosts",
                columns: new[] { "PostId", "SubPostId", "AuthorId", "UserId", "UserHashString", "CreatedDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserViewPosts");
        }
    }
}
