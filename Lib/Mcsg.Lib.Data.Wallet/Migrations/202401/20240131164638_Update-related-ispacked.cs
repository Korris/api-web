using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Wallet.Migrations._202401
{
    /// <inheritdoc />
    public partial class Updaterelatedispacked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPackage",
                table: "PremiumPackages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyValue: 1,
                column: "IsPackage",
                value: true);

            migrationBuilder.UpdateData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyValue: 2,
                column: "IsPackage",
                value: true);

            migrationBuilder.UpdateData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyValue: 3,
                column: "IsPackage",
                value: true);

            migrationBuilder.UpdateData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyValue: 4,
                column: "IsPackage",
                value: true);

            migrationBuilder.InsertData(
                table: "PremiumPackages",
                columns: new[] { "No", "Description", "FirstTimeDiscountPercent", "FirstTimePrice", "FirstTimePricePerMonth", "IsPackage", "LiveTimeDay", "Name", "Price", "PricePerMonth" },
                values: new object[] { 5, "Individual", 0, 2000f, 2000f, false, 0, "Individual", 2000f, 2000f });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "IsPackage",
                table: "PremiumPackages");
        }
    }
}
