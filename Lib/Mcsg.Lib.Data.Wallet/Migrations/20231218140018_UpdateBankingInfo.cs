using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Wallet.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBankingInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CVVNumber",
                table: "UserPaymentMethods",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "BankBin",
                table: "UserPaymentMethods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankCode",
                table: "UserPaymentMethods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "UserPaymentMethods",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankBin",
                table: "UserPaymentMethods");

            migrationBuilder.DropColumn(
                name: "BankCode",
                table: "UserPaymentMethods");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "UserPaymentMethods");

            migrationBuilder.AlterColumn<int>(
                name: "CVVNumber",
                table: "UserPaymentMethods",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
