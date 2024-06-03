using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202310
{
    /// <inheritdoc />
    public partial class UserAddReasonStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActivedDate",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusReason",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb"),
                columns: new[] { "ActivedDate", "StatusReason" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff5727ac-4b12-4e03-9e02-25bfb9086cbf"),
                columns: new[] { "ActivedDate", "StatusReason" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivedDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StatusReason",
                table: "Users");
        }
    }
}
