using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Wallet.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSystemColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFromSystem",
                table: "WalletTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFromSystem",
                table: "WalletTransactions");
        }
    }
}
