using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202402
{
    /// <inheritdoc />
    public partial class AddIsActiveEarning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActiveEarning",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "IsActiveEarning",
                value: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("13778acb-926e-4a46-b395-620a60c767e4"),
                column: "IsActiveEarning",
                value: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("14a17a27-b240-4cc8-98e7-419eaa1498e2"),
                column: "IsActiveEarning",
                value: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("23df53e6-5096-4d3c-b248-6139f6b06e65"),
                column: "IsActiveEarning",
                value: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("295389fb-d4b6-47e6-bf89-365db61d890f"),
                column: "IsActiveEarning",
                value: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("6fd356db-5a1d-4260-9f0d-29ca702d8b02"),
                column: "IsActiveEarning",
                value: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c3313143-39ad-492e-8bac-28be0acc4fed"),
                column: "IsActiveEarning",
                value: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ee856925-7f86-4770-b2db-18ff794aafae"),
                column: "IsActiveEarning",
                value: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb"),
                column: "IsActiveEarning",
                value: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff5727ac-4b12-4e03-9e02-25bfb9086cbf"),
                column: "IsActiveEarning",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActiveEarning",
                table: "Users");
        }
    }
}
