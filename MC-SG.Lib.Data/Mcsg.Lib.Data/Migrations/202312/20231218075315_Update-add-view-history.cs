using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202312
{
    /// <inheritdoc />
    public partial class Updateaddviewhistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TagPosts_TagId",
                table: "TagPosts");

            migrationBuilder.DropIndex(
                name: "IX_SubPosts_PostId",
                table: "SubPosts");

            migrationBuilder.DropIndex(
                name: "IX_SubPostReactions_TargetId",
                table: "SubPostReactions");

            migrationBuilder.DropIndex(
                name: "IX_SubPostComments_PostId",
                table: "SubPostComments");

            migrationBuilder.DropIndex(
                name: "IX_SubPostCommentReactions_TargetId",
                table: "SubPostCommentReactions");

            migrationBuilder.DropIndex(
                name: "IX_Posts_HashId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_PostReactions_TargetId",
                table: "PostReactions");

            migrationBuilder.DropIndex(
                name: "IX_PostComments_PostId",
                table: "PostComments");

            migrationBuilder.DropIndex(
                name: "IX_PostCommentReactions_TargetId",
                table: "PostCommentReactions");

            migrationBuilder.AddColumn<string>(
                name: "HashId",
                table: "SubPosts",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ViewHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EntityType = table.Column<int>(type: "integer", nullable: false),
                    SubType = table.Column<int>(type: "integer", nullable: true),
                    IpAddress = table.Column<string>(type: "text", nullable: true),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsedId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViewHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagPosts_TagId_PostId",
                table: "TagPosts",
                columns: new[] { "TagId", "PostId" });

            migrationBuilder.CreateIndex(
                name: "IX_SubPosts_PostId_HashId_AuthorId",
                table: "SubPosts",
                columns: new[] { "PostId", "HashId", "AuthorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubPostReactions_TargetId_ParentId_AuthorId",
                table: "SubPostReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_SubPostComments_PostId_ParentId_AuthorId",
                table: "SubPostComments",
                columns: new[] { "PostId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_SubPostCommentReactions_TargetId_ParentId_AuthorId",
                table: "SubPostCommentReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_HashId_UserId_Type_Id",
                table: "Posts",
                columns: new[] { "HashId", "UserId", "Type", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_TargetId_ParentId_AuthorId",
                table: "PostReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_PostComments_PostId_ParentId_AuthorId",
                table: "PostComments",
                columns: new[] { "PostId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_PostCommentReactions_TargetId_ParentId_AuthorId",
                table: "PostCommentReactions",
                columns: new[] { "TargetId", "ParentId", "AuthorId" });

            migrationBuilder.CreateIndex(
                name: "IX_ViewHistories_EntityId_UsedId_EntityType_CreatedDate",
                table: "ViewHistories",
                columns: new[] { "EntityId", "UsedId", "EntityType", "CreatedDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ViewHistories_UserId",
                table: "ViewHistories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViewHistories");

            migrationBuilder.DropIndex(
                name: "IX_TagPosts_TagId_PostId",
                table: "TagPosts");

            migrationBuilder.DropIndex(
                name: "IX_SubPosts_PostId_HashId_AuthorId",
                table: "SubPosts");

            migrationBuilder.DropIndex(
                name: "IX_SubPostReactions_TargetId_ParentId_AuthorId",
                table: "SubPostReactions");

            migrationBuilder.DropIndex(
                name: "IX_SubPostComments_PostId_ParentId_AuthorId",
                table: "SubPostComments");

            migrationBuilder.DropIndex(
                name: "IX_SubPostCommentReactions_TargetId_ParentId_AuthorId",
                table: "SubPostCommentReactions");

            migrationBuilder.DropIndex(
                name: "IX_Posts_HashId_UserId_Type_Id",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_PostReactions_TargetId_ParentId_AuthorId",
                table: "PostReactions");

            migrationBuilder.DropIndex(
                name: "IX_PostComments_PostId_ParentId_AuthorId",
                table: "PostComments");

            migrationBuilder.DropIndex(
                name: "IX_PostCommentReactions_TargetId_ParentId_AuthorId",
                table: "PostCommentReactions");

            migrationBuilder.DropColumn(
                name: "HashId",
                table: "SubPosts");

            migrationBuilder.CreateIndex(
                name: "IX_TagPosts_TagId",
                table: "TagPosts",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPosts_PostId",
                table: "SubPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPostReactions_TargetId",
                table: "SubPostReactions",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPostComments_PostId",
                table: "SubPostComments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPostCommentReactions_TargetId",
                table: "SubPostCommentReactions",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_HashId",
                table: "Posts",
                column: "HashId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_TargetId",
                table: "PostReactions",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_PostComments_PostId",
                table: "PostComments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostCommentReactions_TargetId",
                table: "PostCommentReactions",
                column: "TargetId");
        }
    }
}
