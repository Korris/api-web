using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Wallet.Migrations._202401
{
    /// <inheritdoc />
    public partial class Updatepaymentmethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankBin",
                table: "UserPaymentMethods");

            migrationBuilder.DropColumn(
                name: "BankBranch",
                table: "UserPaymentMethods");

            migrationBuilder.DropColumn(
                name: "BankCode",
                table: "UserPaymentMethods");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "UserPaymentMethods");

            migrationBuilder.DropColumn(
                name: "CVVNumber",
                table: "UserPaymentMethods");

            migrationBuilder.DropColumn(
                name: "ExpiredMonth",
                table: "UserPaymentMethods");

            migrationBuilder.DropColumn(
                name: "ExpiredYear",
                table: "UserPaymentMethods");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "UserPaymentMethods");

            migrationBuilder.DropColumn(
                name: "SwiftCode",
                table: "UserPaymentMethods");

            migrationBuilder.AddColumn<string>(
                name: "Bin",
                table: "PaymentMethods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "PaymentMethods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SelfId",
                table: "PaymentMethods",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "PaymentMethods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwiftCode",
                table: "PaymentMethods",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bin",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "SelfId",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "SwiftCode",
                table: "PaymentMethods");

            migrationBuilder.AddColumn<string>(
                name: "BankBin",
                table: "UserPaymentMethods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankBranch",
                table: "UserPaymentMethods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankCode",
                table: "UserPaymentMethods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "UserPaymentMethods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CVVNumber",
                table: "UserPaymentMethods",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpiredMonth",
                table: "UserPaymentMethods",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpiredYear",
                table: "UserPaymentMethods",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "UserPaymentMethods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SwiftCode",
                table: "UserPaymentMethods",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
