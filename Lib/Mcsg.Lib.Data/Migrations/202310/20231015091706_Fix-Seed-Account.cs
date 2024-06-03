using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Migrations._202310
{
    /// <inheritdoc />
    public partial class FixSeedAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb"),
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "admin@angelpj.com", "ADMIN@ANGELPJ.COM", "ADMIN@ANGELPJ.COM", "admin@angelpj.com" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff5727ac-4b12-4e03-9e02-25bfb9086cbf"),
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { "sysadmin@angelpj.com", "SYSADMIN@ANGELPJ.COM", "SYSADMIN@ANGELPJ.COM", "sysadmin@angelpj.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff09e6eb-8ff5-4073-b8f7-a1bc7dc8d4cb"),
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { null, null, "admin", "admin" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("ff5727ac-4b12-4e03-9e02-25bfb9086cbf"),
                columns: new[] { "Email", "NormalizedEmail", "NormalizedUserName", "UserName" },
                values: new object[] { null, null, "sysadmin", "sysadmin" });
        }
    }
}
