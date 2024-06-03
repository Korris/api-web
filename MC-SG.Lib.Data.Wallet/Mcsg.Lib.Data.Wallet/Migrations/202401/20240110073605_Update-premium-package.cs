using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mcsg.Lib.Data.Wallet.Migrations._202401
{
    /// <inheritdoc />
    public partial class Updatepremiumpackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PremiumPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<float>(type: "real", nullable: false),
                    LiveTimeDay = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremiumPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPremiumPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PremiumPackagId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PremiumPackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserWalletId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPremiumPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPremiumPackages_PremiumPackages_PremiumPackageId",
                        column: x => x.PremiumPackageId,
                        principalTable: "PremiumPackages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserPremiumPackages_UserWallets_UserWalletId",
                        column: x => x.UserWalletId,
                        principalTable: "UserWallets",
                        principalColumn: "Id");
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_UserPremiumPackages_UserWalletId",
                table: "UserPremiumPackages",
                column: "UserWalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPremiumPackages");

            migrationBuilder.DropTable(
                name: "PremiumPackages");
        }
    }
}
