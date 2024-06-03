using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202403
{
    /// <inheritdoc />
    public partial class AddAuditForPostFavoriteAndTagFavorite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "TagFavorites",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "TagFavorites",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "TagFavorites",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "TagFavorites",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "TagFavorites",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "PostReports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PostReports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "PostReports",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "PostReports",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "PostReports",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "PostFavorites",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PostFavorites",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "PostFavorites",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "PostFavorites",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedDate",
                table: "PostFavorites",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TagFavorites");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "TagFavorites");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "TagFavorites");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "TagFavorites");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "TagFavorites");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PostReports");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PostReports");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "PostReports");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "PostReports");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "PostReports");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PostFavorites");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PostFavorites");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "PostFavorites");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "PostFavorites");

            migrationBuilder.DropColumn(
                name: "LastModifiedDate",
                table: "PostFavorites");
        }
    }
}
