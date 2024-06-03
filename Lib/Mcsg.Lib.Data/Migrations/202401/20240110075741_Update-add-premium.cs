using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202401
{
    /// <inheritdoc />
    public partial class Updateaddpremium : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "PremiumDate",
                table: "Users",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "PremiumDate",
                table: "Sessions",
                type: "date",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb"),
                column: "PremiumDate",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff5727ac-4b12-4e03-9e02-25bfb9086cbf"),
                column: "PremiumDate",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PremiumDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PremiumDate",
                table: "Sessions");
        }
    }
}
