using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mcsg.Lib.Data.Wallet.Migrations._202404
{
    /// <inheritdoc />
    public partial class AddEarningSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EarningPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EarningPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EarningSummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    TotalAmount = table.Column<float>(type: "real", nullable: false),
                    EarningPeriodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EarningSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EarningSummaries_EarningPeriods_EarningPeriodId",
                        column: x => x.EarningPeriodId,
                        principalTable: "EarningPeriods",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EarningSummaryDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EarningSummaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    DataValue = table.Column<float>(type: "real", nullable: false),
                    EarningValue = table.Column<float>(type: "real", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EarningSummaryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EarningSummaryDetails_EarningSummaries_EarningSummaryId",
                        column: x => x.EarningSummaryId,
                        principalTable: "EarningSummaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_Id_Status_Type",
                table: "WalletTransactions",
                columns: new[] { "Id", "Status", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPurchaseTransactions_CreatorUserId_CreatedDate",
                table: "UserPurchaseTransactions",
                columns: new[] { "CreatorUserId", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EarningPeriods_Year_Order_FromDate_ToDate",
                table: "EarningPeriods",
                columns: new[] { "Year", "Order", "FromDate", "ToDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EarningSummaries_EarningPeriodId",
                table: "EarningSummaries",
                column: "EarningPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_EarningSummaries_UserId_PeriodId",
                table: "EarningSummaries",
                columns: new[] { "UserId", "PeriodId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EarningSummaryDetails_EarningSummaryId",
                table: "EarningSummaryDetails",
                column: "EarningSummaryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EarningSummaryDetails");

            migrationBuilder.DropTable(
                name: "EarningSummaries");

            migrationBuilder.DropTable(
                name: "EarningPeriods");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_Id_Status_Type",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_UserPurchaseTransactions_CreatorUserId_CreatedDate",
                table: "UserPurchaseTransactions");
        }
    }
}
