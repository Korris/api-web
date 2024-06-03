using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Wallet.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_ReferenceNumber",
                table: "WalletTransactions",
                column: "ReferenceNumber",
                unique: true,
                filter: "[ReferenceNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserWallets_Address",
                table: "UserWallets",
                column: "Address",
                unique: true,
                filter: "[Address] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_ReferenceNumber",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_UserWallets_Address",
                table: "UserWallets");
        }
    }
}
