using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202310
{
    /// <inheritdoc />
    public partial class AddReactionBase : Migration
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

            migrationBuilder.AlterColumn<Guid>(
                name: "PostId",
                table: "SubPostReactions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "TargetId",
                table: "SubPostReactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "CommentId",
                table: "SubPostCommentReactions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "TargetId",
                table: "SubPostCommentReactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "PostId",
                table: "PostReactions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "TargetId",
                table: "PostReactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "CommentId",
                table: "PostCommentReactions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "TargetId",
                table: "PostCommentReactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "SubPostReactions");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "SubPostCommentReactions");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "PostReactions");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "PostCommentReactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "PostId",
                table: "SubPostReactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CommentId",
                table: "SubPostCommentReactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PostId",
                table: "PostReactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CommentId",
                table: "PostCommentReactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PostCommentReactions_PostComments_CommentId",
                table: "PostCommentReactions",
                column: "CommentId",
                principalTable: "PostComments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostReactions_Posts_PostId",
                table: "PostReactions",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubPostCommentReactions_SubPostComments_CommentId",
                table: "SubPostCommentReactions",
                column: "CommentId",
                principalTable: "SubPostComments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubPostReactions_SubPosts_PostId",
                table: "SubPostReactions",
                column: "PostId",
                principalTable: "SubPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
