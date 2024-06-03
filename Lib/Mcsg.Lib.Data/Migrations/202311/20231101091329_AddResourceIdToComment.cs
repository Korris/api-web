using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202311
{
    /// <inheritdoc />
    public partial class AddResourceIdToComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ResourceId",
                table: "SubPostComments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationType",
                table: "Resources",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ResourceId",
                table: "PostComments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubPostComments_ResourceId",
                table: "SubPostComments",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_PostComments_ResourceId",
                table: "PostComments",
                column: "ResourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostComments_Resources_ResourceId",
                table: "PostComments",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubPostComments_Resources_ResourceId",
                table: "SubPostComments",
                column: "ResourceId",
                principalTable: "Resources",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostComments_Resources_ResourceId",
                table: "PostComments");

            migrationBuilder.DropForeignKey(
                name: "FK_SubPostComments_Resources_ResourceId",
                table: "SubPostComments");

            migrationBuilder.DropIndex(
                name: "IX_SubPostComments_ResourceId",
                table: "SubPostComments");

            migrationBuilder.DropIndex(
                name: "IX_PostComments_ResourceId",
                table: "PostComments");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "SubPostComments");

            migrationBuilder.DropColumn(
                name: "LocationType",
                table: "Resources");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                table: "PostComments");
        }
    }
}
