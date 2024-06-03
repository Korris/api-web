using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Wallet.Migrations._202402
{
    /// <inheritdoc />
    public partial class Updateuserpurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPurchaseTransactions_WalletTransactions_WalletTransactionId",
                table: "UserPurchaseTransactions");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "UserPurchaseTransactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "WalletTransactionId",
                table: "UserPurchaseTransactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPurchaseTransactions_WalletTransactions_WalletTransactionId",
                table: "UserPurchaseTransactions",
                column: "WalletTransactionId",
                principalTable: "WalletTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPurchaseTransactions_WalletTransactions_WalletTransactionId",
                table: "UserPurchaseTransactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "WalletTransactionId",
                table: "UserPurchaseTransactions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                table: "UserPurchaseTransactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_UserPurchaseTransactions_WalletTransactions_WalletTransactionId",
                table: "UserPurchaseTransactions",
                column: "WalletTransactionId",
                principalTable: "WalletTransactions",
                principalColumn: "Id");
        }
    }
}
