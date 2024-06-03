using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Wallet.Migrations._202312
{
    /// <inheritdoc />
    public partial class AddWalletTransactionUserPaymentMethodId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserPaymentMethodId",
                table: "WalletTransactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_UserPaymentMethodId",
                table: "WalletTransactions",
                column: "UserPaymentMethodId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_UserPaymentMethods_UserPaymentMethodId",
                table: "WalletTransactions",
                column: "UserPaymentMethodId",
                principalTable: "UserPaymentMethods",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_UserPaymentMethods_UserPaymentMethodId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_UserPaymentMethodId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "UserPaymentMethodId",
                table: "WalletTransactions");
        }
    }
}
