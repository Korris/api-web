using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Wallet.Migrations._202402
{
    /// <inheritdoc />
    public partial class Updatepremiumpackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyValue: 1,
                column: "Name",
                value: "Premium package x 1 month");

            migrationBuilder.UpdateData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyValue: 2,
                column: "Name",
                value: "Premium package x 3 months");

            migrationBuilder.UpdateData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyValue: 3,
                column: "Name",
                value: "Premium package x 6 months");

            migrationBuilder.UpdateData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyValue: 4,
                column: "Name",
                value: "Premium package x 12 months");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyValue: 1,
                column: "Name",
                value: "1 month");

            migrationBuilder.UpdateData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyValue: 2,
                column: "Name",
                value: "3 months");

            migrationBuilder.UpdateData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyValue: 3,
                column: "Name",
                value: "6 months");

            migrationBuilder.UpdateData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyValue: 4,
                column: "Name",
                value: "12 months");
        }
    }
}
