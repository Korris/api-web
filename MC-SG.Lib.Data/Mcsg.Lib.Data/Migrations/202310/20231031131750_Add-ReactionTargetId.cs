using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202310
{
    /// <inheritdoc />
    public partial class AddReactionTargetId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostCommentReactions_PostComments_CommentId",
                table: "PostCommentReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReactions_Posts_PostId",
                table: "PostReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_SubPostCommentReactions_SubPostComments_CommentId",
                table: "SubPostCommentReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_SubPostReactions_SubPosts_PostId",
                table: "SubPostReactions");

            migrationBuilder.DropIndex(
                name: "IX_SubPostReactions_PostId",
                table: "SubPostReactions");

            migrationBuilder.DropIndex(
                name: "IX_SubPostCommentReactions_CommentId",
                table: "SubPostCommentReactions");

            migrationBuilder.DropIndex(
                name: "IX_PostReactions_PostId",
                table: "PostReactions");

            migrationBuilder.DropIndex(
                name: "IX_PostCommentReactions_CommentId",
                table: "PostCommentReactions");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "SubPostReactions");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "SubPostCommentReactions");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "PostReactions");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "PostCommentReactions");

            migrationBuilder.CreateIndex(
                name: "IX_SubPostReactions_TargetId",
                table: "SubPostReactions",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPostCommentReactions_TargetId",
                table: "SubPostCommentReactions",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_TargetId",
                table: "PostReactions",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_PostCommentReactions_TargetId",
                table: "PostCommentReactions",
                column: "TargetId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostCommentReactions_PostComments_TargetId",
                table: "PostCommentReactions",
                column: "TargetId",
                principalTable: "PostComments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostReactions_Posts_TargetId",
                table: "PostReactions",
                column: "TargetId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubPostCommentReactions_SubPostComments_TargetId",
                table: "SubPostCommentReactions",
                column: "TargetId",
                principalTable: "SubPostComments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubPostReactions_SubPosts_TargetId",
                table: "SubPostReactions",
                column: "TargetId",
                principalTable: "SubPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostCommentReactions_PostComments_TargetId",
                table: "PostCommentReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReactions_Posts_TargetId",
                table: "PostReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_SubPostCommentReactions_SubPostComments_TargetId",
                table: "SubPostCommentReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_SubPostReactions_SubPosts_TargetId",
                table: "SubPostReactions");

            migrationBuilder.DropIndex(
                name: "IX_SubPostReactions_TargetId",
                table: "SubPostReactions");

            migrationBuilder.DropIndex(
                name: "IX_SubPostCommentReactions_TargetId",
                table: "SubPostCommentReactions");

            migrationBuilder.DropIndex(
                name: "IX_PostReactions_TargetId",
                table: "PostReactions");

            migrationBuilder.DropIndex(
                name: "IX_PostCommentReactions_TargetId",
                table: "PostCommentReactions");

            migrationBuilder.AddColumn<Guid>(
                name: "PostId",
                table: "SubPostReactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CommentId",
                table: "SubPostCommentReactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PostId",
                table: "PostReactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CommentId",
                table: "PostCommentReactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubPostReactions_PostId",
                table: "SubPostReactions",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_SubPostCommentReactions_CommentId",
                table: "SubPostCommentReactions",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_PostId",
                table: "PostReactions",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostCommentReactions_CommentId",
                table: "PostCommentReactions",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostCommentReactions_PostComments_CommentId",
                table: "PostCommentReactions",
                column: "CommentId",
                principalTable: "PostComments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostReactions_Posts_PostId",
                table: "PostReactions",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubPostCommentReactions_SubPostComments_CommentId",
                table: "SubPostCommentReactions",
                column: "CommentId",
                principalTable: "SubPostComments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubPostReactions_SubPosts_PostId",
                table: "SubPostReactions",
                column: "PostId",
                principalTable: "SubPosts",
                principalColumn: "Id");
        }
    }
}
