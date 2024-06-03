using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Wallet.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "WalletSettings",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "IsDelete", "Logo", "ModifiedBy", "ModifiedDate", "Name", "Symbol" },
                values: new object[] { new Guid("a2f9d301-b081-4cd8-850f-27bc996702e7"), null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), "BL Coin", "BL" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WalletSettings",
                keyColumn: "Id",
                keyValue: new Guid("a2f9d301-b081-4cd8-850f-27bc996702e7"));
        }
    }
}
