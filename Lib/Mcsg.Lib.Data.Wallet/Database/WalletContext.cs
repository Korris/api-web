using Microsoft.EntityFrameworkCore;

namespace Mcsg.Lib.Data.Wallet;

using Entities;
using Enums;

/// <summary>
/// WalletDbContext
/// </summary>
public class WalletContext : DbContext
{
    #region -- Overrides --

    /// <summary>
    /// On model creating
    /// </summary>
    /// <param name="builder">Builder</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<WalletSetting>().HasData(
            new WalletSetting
            {
                Id = Guid.Parse("A2F9D301-B081-4CD8-850F-27BC996702E7"),
                Name = "BL Coin",
                Symbol = "BL",
                CreatedOn = new DateTime(2024, 08, 08, 18, 45, 04),
                ModifiedOn = new DateTime(2024, 08, 08, 18, 45, 04)
            });

        builder.Entity<WalletSettingDetail>().HasData(
            new WalletSettingDetail
            {
                Id = Guid.Parse("5F65FED1-BDDC-4C7C-BA57-CB33A54542C8"),
                Name = nameof(WalletSettingDetailType.WithDrawNotify),
                Type = WalletSettingDetailType.WithDrawNotify,
                Value = "info@bumcheo.vn",
                Description = "With draw notify list email",
                CreatedOn = new DateTime(2024, 08, 08, 18, 45, 04),
                ModifiedOn = new DateTime(2024, 08, 08, 18, 45, 04)
            });

        builder.Entity<PremiumPackage>().HasData(
            new PremiumPackage
            {
                No = 1,
                Name = "Premium package x 1 month",
                Description = "1 month/ 30 days",
                Price = 59000,
                PricePerMonth = 59000,
                LiveTimeDay = 30,
                FirstTimeDiscountPercent = 49,
                FirstTimePrice = 29000,
                FirstTimePricePerMonth = 29000,
                IsPackage = true
            },
            new PremiumPackage
            {
                No = 2,
                Name = "Premium package x 3 months",
                Description = "3 months/ 90 days",
                Price = 636000,
                PricePerMonth = 53000,
                LiveTimeDay = 90,
                FirstTimeDiscountPercent = 49,
                FirstTimePrice = 78000,
                FirstTimePricePerMonth = 26000,
                IsPackage = true
            },
            new PremiumPackage
            {
                No = 3,
                Name = "Premium package x 6 months",
                Description = "6 months/ 182 days",
                Price = 300000,
                PricePerMonth = 50000,
                LiveTimeDay = 182,
                FirstTimeDiscountPercent = 49,
                FirstTimePrice = 150000,
                FirstTimePricePerMonth = 25000,
                IsPackage = true
            },
            new PremiumPackage
            {
                No = 4,
                Name = "Premium package x 12 months",
                Description = "12 months/ 365 days",
                Price = 30000,
                PricePerMonth = 47000,
                LiveTimeDay = 365,
                FirstTimeDiscountPercent = 49,
                FirstTimePrice = 276000,
                FirstTimePricePerMonth = 23000,
                IsPackage = true
            },
             new PremiumPackage
             {
                 No = 5,
                 Name = "Individual",
                 Description = "Individual",
                 Price = 2000,
                 PricePerMonth = 2000,
                 LiveTimeDay = 0,
                 FirstTimeDiscountPercent = 0,
                 FirstTimePrice = 2000,
                 FirstTimePricePerMonth = 2000,
                 IsPackage = false
             }
            );

        builder.Entity<UserPremiumPackage>(entity =>
        {
            entity.HasOne(x => x.PremiumPackage)
            .WithMany()
            .HasForeignKey(x => x.PremiumPackageNo);

            entity.HasOne(x => x.UserWallet)
            .WithMany()
            .HasForeignKey(x => x.UserWalletId);
        });
        builder.Entity<WalletTransaction>(entity =>
        {
            entity.HasOne(x => x.DestinationUserWallet)
            .WithMany(x => x.DestinationUserWalletTransactions)
            .HasForeignKey(x => x.DestinationUserWalletId);

            entity.HasOne(x => x.SourceUserWallet)
            .WithMany(x => x.SourceUserWalletTransactions)
            .HasForeignKey(x => x.SourceUserWalletId);

            entity.HasIndex(x => x.ReferenceNumber).IsUnique();
            entity.HasIndex(x => new { x.Id, x.Status, x.Type }).IsUnique();
        });

        builder.Entity<WalletTransactionOtp>(entity =>
        {
            entity.HasOne(x => x.WalletTransaction)
            .WithMany(x => x.WalletTransactionOtps)
            .HasForeignKey(x => x.TransactionId);

            entity.HasIndex(x => x.Otp);
            entity.HasIndex(x => x.OtpToken);
        });

        builder.Entity<UserPurchaseTransaction>(entity =>
        {
            entity.HasOne(x => x.WalletTransaction)
            .WithMany(x => x.UserPurchaseTransactions)
            .HasForeignKey(x => x.WalletTransactionId);
            entity.HasIndex(x => new { x.CreatorUserId, x.CreatedOn });
        });

        builder.Entity<UserWallet>(entity =>
        {
            entity.HasIndex(x => x.Address).IsUnique();
        });

        builder.Entity<EarningPeriod>(entity =>
        {
            entity.HasIndex(x => new { x.Year, x.Order, x.FromDate, x.ToDate }).IsUnique();
        });
        builder.Entity<EarningSummary>(entity =>
        {
            entity.HasIndex(x => new { x.UserId, x.PeriodId }).IsUnique();
        });
        builder.Entity<EarningSummaryDetail>(entity =>
        {
            entity.HasIndex(x => x.EarningSummaryId);
            entity.HasOne(x => x.EarningSummary)
          .WithMany(x => x.SummaryDetails)
          .HasForeignKey(x => x.EarningSummaryId);
        });
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="options">Options</param>
    public WalletContext(DbContextOptions<WalletContext> options) : base(options) { }

    #endregion

    #region -- Properties --

    public virtual DbSet<UserWallet> UserWallets { get; set; }
    public virtual DbSet<PremiumPackage> PremiumPackages { get; set; }
    public virtual DbSet<UserPremiumPackage> UserPremiumPackages { get; set; }
    public virtual DbSet<WalletTransaction> WalletTransactions { get; set; }
    public virtual DbSet<WalletTransactionOtp> WalletTransactionOtps { get; set; }
    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
    public virtual DbSet<WalletSetting> WalletSettings { get; set; }
    public virtual DbSet<WalletSettingDetail> WalletSettingDetails { get; set; }
    public virtual DbSet<UserPaymentMethod> UserPaymentMethods { get; set; }
    public virtual DbSet<UserPurchaseTransaction> UserPurchaseTransactions { get; set; }
    public virtual DbSet<EarningPeriod> EarningPeriods { get; set; }
    public virtual DbSet<EarningSummary> EarningSummaries { get; set; }
    public virtual DbSet<EarningSummaryDetail> EarningSummaryDetails { get; set; }

    #endregion
}
