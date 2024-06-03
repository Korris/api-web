using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Wallet.Migrations
{
    /// <inheritdoc />
    public partial class ChangeWallerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_Wallets_UserWalletId",
                table: "PaymentMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPaymentMethods_Wallets_UserWalletId",
                table: "UserPaymentMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_WalletSettings_WalletSettingId",
                table: "Wallets");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Wallets_DestinationUserWalletId",
                table: "WalletTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Wallets_SourceUserWalletId",
                table: "WalletTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wallets",
                table: "Wallets");

            migrationBuilder.RenameTable(
                name: "Wallets",
                newName: "UserWallets");

            migrationBuilder.RenameIndex(
                name: "IX_Wallets_WalletSettingId",
                table: "UserWallets",
                newName: "IX_UserWallets_WalletSettingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserWallets",
                table: "UserWallets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_UserWallets_UserWalletId",
                table: "PaymentMethods",
                column: "UserWalletId",
                principalTable: "UserWallets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPaymentMethods_UserWallets_UserWalletId",
                table: "UserPaymentMethods",
                column: "UserWalletId",
                principalTable: "UserWallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserWallets_WalletSettings_WalletSettingId",
                table: "UserWallets",
                column: "WalletSettingId",
                principalTable: "WalletSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_UserWallets_DestinationUserWalletId",
                table: "WalletTransactions",
                column: "DestinationUserWalletId",
                principalTable: "UserWallets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_UserWallets_SourceUserWalletId",
                table: "WalletTransactions",
                column: "SourceUserWalletId",
                principalTable: "UserWallets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentMethods_UserWallets_UserWalletId",
                table: "PaymentMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPaymentMethods_UserWallets_UserWalletId",
                table: "UserPaymentMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_UserWallets_WalletSettings_WalletSettingId",
                table: "UserWallets");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_UserWallets_DestinationUserWalletId",
                table: "WalletTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_UserWallets_SourceUserWalletId",
                table: "WalletTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserWallets",
                table: "UserWallets");

            migrationBuilder.RenameTable(
                name: "UserWallets",
                newName: "Wallets");

            migrationBuilder.RenameIndex(
                name: "IX_UserWallets_WalletSettingId",
                table: "Wallets",
                newName: "IX_Wallets_WalletSettingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wallets",
                table: "Wallets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentMethods_Wallets_UserWalletId",
                table: "PaymentMethods",
                column: "UserWalletId",
                principalTable: "Wallets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPaymentMethods_Wallets_UserWalletId",
                table: "UserPaymentMethods",
                column: "UserWalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_WalletSettings_WalletSettingId",
                table: "Wallets",
                column: "WalletSettingId",
                principalTable: "WalletSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Wallets_DestinationUserWalletId",
                table: "WalletTransactions",
                column: "DestinationUserWalletId",
                principalTable: "Wallets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Wallets_SourceUserWalletId",
                table: "WalletTransactions",
                column: "SourceUserWalletId",
                principalTable: "Wallets",
                principalColumn: "Id");
        }
    }
}
