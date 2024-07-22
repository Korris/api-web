using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Mcsg.Lib.Data.Wallet.Migrations
{
    /// <inheritdoc />
    public partial class InitData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EarningPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    FromDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ToDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EarningPeriods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PremiumPackages",
                columns: table => new
                {
                    No = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<float>(type: "real", nullable: false),
                    PricePerMonth = table.Column<float>(type: "real", nullable: false),
                    LiveTimeDay = table.Column<int>(type: "integer", nullable: false),
                    FirstTimeDiscountPercent = table.Column<int>(type: "integer", nullable: false),
                    FirstTimePricePerMonth = table.Column<float>(type: "real", nullable: false),
                    FirstTimePrice = table.Column<float>(type: "real", nullable: false),
                    IsPackage = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PremiumPackages", x => x.No);
                });

            migrationBuilder.CreateTable(
                name: "WalletSettingDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletSettingDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WalletSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Symbol = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    Logo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EarningSummaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PeriodId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    TotalAmount = table.Column<float>(type: "real", nullable: false),
                    EarningPeriodId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "UserWallets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Point = table.Column<float>(type: "real", nullable: false),
                    RewardPoint = table.Column<float>(type: "real", nullable: false),
                    WalletSettingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SystemMessage = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWallets_WalletSettings_WalletSettingId",
                        column: x => x.WalletSettingId,
                        principalTable: "WalletSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EarningSummaryDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EarningSummaryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    DataValue = table.Column<float>(type: "real", nullable: false),
                    EarningValue = table.Column<float>(type: "real", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Logo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AllowDeposit = table.Column<bool>(type: "boolean", nullable: false),
                    AllowWithdrawal = table.Column<bool>(type: "boolean", nullable: false),
                    SelfId = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Bin = table.Column<string>(type: "text", nullable: true),
                    ShortName = table.Column<string>(type: "text", nullable: true),
                    SwiftCode = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UserWalletId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_UserWallets_UserWalletId",
                        column: x => x.UserWalletId,
                        principalTable: "UserWallets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserPremiumPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PremiumPackageNo = table.Column<int>(type: "integer", nullable: true),
                    UserWalletId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPremiumPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPremiumPackages_PremiumPackages_PremiumPackageNo",
                        column: x => x.PremiumPackageNo,
                        principalTable: "PremiumPackages",
                        principalColumn: "No");
                    table.ForeignKey(
                        name: "FK_UserPremiumPackages_UserWallets_UserWalletId",
                        column: x => x.UserWalletId,
                        principalTable: "UserWallets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserPaymentMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserWalletId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentMethodId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountNumber = table.Column<string>(type: "text", nullable: true),
                    AccountName = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPaymentMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPaymentMethods_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPaymentMethods_UserWallets_UserWalletId",
                        column: x => x.UserWalletId,
                        principalTable: "UserWallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    SourceUserWalletId = table.Column<Guid>(type: "uuid", nullable: true),
                    DestinationUserWalletId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserPaymentMethodId = table.Column<Guid>(type: "uuid", nullable: true),
                    SystemMethod = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<float>(type: "real", nullable: false),
                    Content = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SystemMessage = table.Column<string>(type: "text", nullable: true),
                    IsFromSystem = table.Column<bool>(type: "boolean", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    RelatedId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExternalId = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_UserPaymentMethods_UserPaymentMethodId",
                        column: x => x.UserPaymentMethodId,
                        principalTable: "UserPaymentMethods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WalletTransactions_UserWallets_DestinationUserWalletId",
                        column: x => x.DestinationUserWalletId,
                        principalTable: "UserWallets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WalletTransactions_UserWallets_SourceUserWalletId",
                        column: x => x.SourceUserWalletId,
                        principalTable: "UserWallets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserPurchaseTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WalletTransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Thumbnail = table.Column<string>(type: "text", nullable: true),
                    Paymethod = table.Column<string>(type: "text", nullable: true),
                    AffiliateUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPurchaseTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPurchaseTransactions_WalletTransactions_WalletTransacti~",
                        column: x => x.WalletTransactionId,
                        principalTable: "WalletTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactionOtps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Otp = table.Column<string>(type: "text", nullable: true),
                    OtpToken = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    OtpType = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactionOtps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTransactionOtps_WalletTransactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "WalletTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PremiumPackages",
                columns: new[] { "No", "Description", "FirstTimeDiscountPercent", "FirstTimePrice", "FirstTimePricePerMonth", "IsPackage", "LiveTimeDay", "Name", "Price", "PricePerMonth" },
                values: new object[,]
                {
                    { 1, "1 month/ 30 days", 49, 29000f, 29000f, true, 30, "Premium package x 1 month", 59000f, 59000f },
                    { 2, "3 months/ 90 days", 49, 78000f, 26000f, true, 90, "Premium package x 3 months", 636000f, 53000f },
                    { 3, "6 months/ 182 days", 49, 150000f, 25000f, true, 182, "Premium package x 6 months", 300000f, 50000f },
                    { 4, "12 months/ 365 days", 49, 276000f, 23000f, true, 365, "Premium package x 12 months", 30000f, 47000f },
                    { 5, "Individual", 0, 2000f, 2000f, false, 0, "Individual", 2000f, 2000f }
                });

            migrationBuilder.InsertData(
                table: "WalletSettingDetails",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "Description", "IsDelete", "ModifiedBy", "ModifiedOn", "Name", "Type", "Value" },
                values: new object[] { new Guid("5f65fed1-bddc-4c7c-ba57-cb33a54542c8"), null, new DateTime(2023, 12, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), "With draw notify list email", false, null, new DateTime(2023, 12, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), "WithDrawNotify", 0, "dev@angelpj.com" });

            migrationBuilder.InsertData(
                table: "WalletSettings",
                columns: new[] { "Id", "CreatedBy", "CreatedOn", "IsDelete", "Logo", "ModifiedBy", "ModifiedOn", "Name", "Symbol" },
                values: new object[] { new Guid("a2f9d301-b081-4cd8-850f-27bc996702e7"), null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), false, null, null, new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984), "BL Coin", "BL" });

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

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_UserWalletId",
                table: "PaymentMethods",
                column: "UserWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPaymentMethods_PaymentMethodId",
                table: "UserPaymentMethods",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPaymentMethods_UserWalletId",
                table: "UserPaymentMethods",
                column: "UserWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPremiumPackages_PremiumPackageNo",
                table: "UserPremiumPackages",
                column: "PremiumPackageNo");

            migrationBuilder.CreateIndex(
                name: "IX_UserPremiumPackages_UserWalletId",
                table: "UserPremiumPackages",
                column: "UserWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPurchaseTransactions_CreatorUserId_CreatedOn",
                table: "UserPurchaseTransactions",
                columns: new[] { "CreatorUserId", "CreatedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPurchaseTransactions_WalletTransactionId",
                table: "UserPurchaseTransactions",
                column: "WalletTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWallets_Address",
                table: "UserWallets",
                column: "Address",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserWallets_WalletSettingId",
                table: "UserWallets",
                column: "WalletSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactionOtps_Otp",
                table: "WalletTransactionOtps",
                column: "Otp");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactionOtps_OtpToken",
                table: "WalletTransactionOtps",
                column: "OtpToken");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactionOtps_TransactionId",
                table: "WalletTransactionOtps",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_DestinationUserWalletId",
                table: "WalletTransactions",
                column: "DestinationUserWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_Id_Status_Type",
                table: "WalletTransactions",
                columns: new[] { "Id", "Status", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_ReferenceNumber",
                table: "WalletTransactions",
                column: "ReferenceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_SourceUserWalletId",
                table: "WalletTransactions",
                column: "SourceUserWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_UserPaymentMethodId",
                table: "WalletTransactions",
                column: "UserPaymentMethodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EarningSummaryDetails");

            migrationBuilder.DropTable(
                name: "UserPremiumPackages");

            migrationBuilder.DropTable(
                name: "UserPurchaseTransactions");

            migrationBuilder.DropTable(
                name: "WalletSettingDetails");

            migrationBuilder.DropTable(
                name: "WalletTransactionOtps");

            migrationBuilder.DropTable(
                name: "EarningSummaries");

            migrationBuilder.DropTable(
                name: "PremiumPackages");

            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DropTable(
                name: "EarningPeriods");

            migrationBuilder.DropTable(
                name: "UserPaymentMethods");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "UserWallets");

            migrationBuilder.DropTable(
                name: "WalletSettings");
        }
    }
}
