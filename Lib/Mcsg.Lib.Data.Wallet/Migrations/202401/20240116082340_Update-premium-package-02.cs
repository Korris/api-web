using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mcsg.Lib.Data.Wallet.Migrations._202401
{
    /// <inheritdoc />
    public partial class Updatepremiumpackage02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPremiumPackages_PremiumPackages_PremiumPackageId",
                table: "UserPremiumPackages");

            migrationBuilder.DropIndex(
                name: "IX_UserPremiumPackages_PremiumPackageId",
                table: "UserPremiumPackages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PremiumPackages",
                table: "PremiumPackages");

            migrationBuilder.DeleteData(
                table: "PremiumPackages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("7b0691e1-a239-4b1c-b7ee-a007a3e4a832"));

            migrationBuilder.DeleteData(
                table: "PremiumPackages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("91042678-dcc6-46e1-bc13-549ff569bd98"));

            migrationBuilder.DeleteData(
                table: "PremiumPackages",
                keyColumn: "Id",
                keyColumnType: "uniqueidentifier",
                keyValue: new Guid("f2fab8b0-0924-498a-96dd-b2be6d9b4551"));

            migrationBuilder.DropColumn(
                name: "PremiumPackagId",
                table: "UserPremiumPackages");

            migrationBuilder.DropColumn(
                name: "PremiumPackageId",
                table: "UserPremiumPackages");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PremiumPackages");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PremiumPackages");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PremiumPackages");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "PremiumPackages");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "PremiumPackages");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "PremiumPackages");

            migrationBuilder.AddColumn<int>(
                name: "PremiumPackageNo",
                table: "UserPremiumPackages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "No",
                table: "PremiumPackages",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "FirstTimeDiscountPercent",
                table: "PremiumPackages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "FirstTimePrice",
                table: "PremiumPackages",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "FirstTimePricePerMonth",
                table: "PremiumPackages",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PricePerMonth",
                table: "PremiumPackages",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PremiumPackages",
                table: "PremiumPackages",
                column: "No");

            migrationBuilder.InsertData(
                table: "PremiumPackages",
                columns: new[] { "No", "Description", "FirstTimeDiscountPercent", "FirstTimePrice", "FirstTimePricePerMonth", "LiveTimeDay", "Name", "Price", "PricePerMonth" },
                values: new object[,]
                {
                    { 1, "1 month/ 30 days", 49, 29000f, 29000f, 30, "1 month", 59000f, 59000f },
                    { 2, "3 months/ 90 days", 49, 78000f, 26000f, 90, "3 months", 636000f, 53000f },
                    { 3, "6 months/ 182 days", 49, 150000f, 25000f, 182, "6 months", 300000f, 50000f },
                    { 4, "12 months/ 365 days", 49, 276000f, 23000f, 365, "12 months", 30000f, 47000f }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPremiumPackages_PremiumPackageNo",
                table: "UserPremiumPackages",
                column: "PremiumPackageNo");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPremiumPackages_PremiumPackages_PremiumPackageNo",
                table: "UserPremiumPackages",
                column: "PremiumPackageNo",
                principalTable: "PremiumPackages",
                principalColumn: "No");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPremiumPackages_PremiumPackages_PremiumPackageNo",
                table: "UserPremiumPackages");

            migrationBuilder.DropIndex(
                name: "IX_UserPremiumPackages_PremiumPackageNo",
                table: "UserPremiumPackages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PremiumPackages",
                table: "PremiumPackages");

            migrationBuilder.DeleteData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyColumnType: "int",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyColumnType: "int",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyColumnType: "int",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PremiumPackages",
                keyColumn: "No",
                keyColumnType: "int",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "PremiumPackageNo",
                table: "UserPremiumPackages");

            migrationBuilder.DropColumn(
                name: "No",
                table: "PremiumPackages");

            migrationBuilder.DropColumn(
                name: "FirstTimeDiscountPercent",
                table: "PremiumPackages");

            migrationBuilder.DropColumn(
                name: "FirstTimePrice",
                table: "PremiumPackages");

            migrationBuilder.DropColumn(
                name: "FirstTimePricePerMonth",
                table: "PremiumPackages");

            migrationBuilder.DropColumn(
                name: "PricePerMonth",
                table: "PremiumPackages");

            migrationBuilder.AddColumn<Guid>(
                name: "PremiumPackagId",
                table: "UserPremiumPackages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PremiumPackageId",
                table: "UserPremiumPackages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "PremiumPackages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "PremiumPackages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PremiumPackages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "PremiumPackages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModifiedBy",
                table: "PremiumPackages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "PremiumPackages",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_PremiumPackages",
                table: "PremiumPackages",
                column: "Id");

            migrationBuilder.InsertData(
                table: "PremiumPackages",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "Description", "IsDelete", "LiveTimeDay", "ModifiedBy", "ModifiedDate", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("7b0691e1-a239-4b1c-b7ee-a007a3e4a832"), null, new DateTime(2023, 12, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), "3 months/ 91 days", false, 91, null, new DateTime(2023, 12, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), "3 months", 30f },
                    { new Guid("91042678-dcc6-46e1-bc13-549ff569bd98"), null, new DateTime(2023, 12, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), "6 months/ 182 days", false, 182, null, new DateTime(2023, 12, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), "6 months", 30f },
                    { new Guid("f2fab8b0-0924-498a-96dd-b2be6d9b4551"), null, new DateTime(2023, 12, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), "12 months/ 365 days", false, 365, null, new DateTime(2023, 12, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), "12 months", 30f }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPremiumPackages_PremiumPackageId",
                table: "UserPremiumPackages",
                column: "PremiumPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPremiumPackages_PremiumPackages_PremiumPackageId",
                table: "UserPremiumPackages",
                column: "PremiumPackageId",
                principalTable: "PremiumPackages",
                principalColumn: "Id");
        }
    }
}
