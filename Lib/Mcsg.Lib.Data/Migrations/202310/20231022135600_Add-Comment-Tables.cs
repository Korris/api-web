using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202310
{
    /// <inheritdoc />
    public partial class AddCommentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostCommentReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CommentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostCommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostCommentReactions_PostComments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "PostComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostCommentReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubPostComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubPostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubPostComments_SubPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "SubPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubPostComments_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubPostReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubPostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubPostReactions_SubPosts_PostId",
                        column: x => x.PostId,
                        principalTable: "SubPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubPostReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubPostCommentReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CommentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubPostCommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubPostCommentReactions_SubPostComments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "SubPostComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubPostCommentReactions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostCommentReactions_AuthorId",
                table: "PostCommentReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_PostCommentReactions_CommentId",
                table: "PostCommentReactions",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPostCommentReactions_AuthorId",
                table: "SubPostCommentReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPostCommentReactions_CommentId",
                table: "SubPostCommentReactions",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPostComments_AuthorId",
                table: "SubPostComments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPostComments_PostId",
                table: "SubPostComments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPostReactions_AuthorId",
                table: "SubPostReactions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPostReactions_PostId",
                table: "SubPostReactions",
                column: "PostId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostCommentReactions");

            migrationBuilder.DropTable(
                name: "SubPostCommentReactions");

            migrationBuilder.DropTable(
                name: "SubPostReactions");

            migrationBuilder.DropTable(
                name: "SubPostComments");
        }
    }
}
