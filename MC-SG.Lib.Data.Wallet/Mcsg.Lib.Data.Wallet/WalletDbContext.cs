using Mcsg.Lib.Data.Wallet.Entities;
using Mcsg.Lib.Data.Wallet.Enums;
using Microsoft.EntityFrameworkCore;
using DataEntities = Mcsg.Lib.Data.Wallet.Entities;
namespace Mcsg.Lib.Data.Wallet
{
    public class WalletDbContext : DbContext
    {
        public WalletDbContext(DbContextOptions<WalletDbContext> dbContext)
        : base(dbContext)
        {

        }

        public virtual DbSet<DataEntities.UserWallet> UserWallets { get; set; }
        public virtual DbSet<DataEntities.PremiumPackage> PremiumPackages { get; set; }
        public virtual DbSet<DataEntities.UserPremiumPackage> UserPremiumPackages { get; set; }
        public virtual DbSet<DataEntities.WalletTransaction> WalletTransactions { get; set; }
        public virtual DbSet<DataEntities.WalletTransactionOtp> WalletTransactionOtps { get; set; }

        public virtual DbSet<DataEntities.PaymentMethod> PaymentMethods { get; set; }
        public virtual DbSet<DataEntities.WalletSetting> WalletSettings { get; set; }
        public virtual DbSet<DataEntities.WalletSettingDetail> WalletSettingDetails { get; set; }
        public virtual DbSet<DataEntities.UserPaymentMethod> UserPaymentMethods { get; set; }
        public virtual DbSet<DataEntities.UserPurchaseTransaction> UserPurchaseTransactions { get; set; }
        public virtual DbSet<DataEntities.EarningPeriod> EarningPeriods { get; set; }
        public virtual DbSet<DataEntities.EarningSummary> EarningSummaries { get; set; }
        public virtual DbSet<DataEntities.EarningSummaryDetail> EarningSummaryDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WalletSetting>().HasData(
                new WalletSetting
                {
                    Id = Guid.Parse("A2F9D301-B081-4CD8-850F-27BC996702E7"),
                    Name = "BL Coin",
                    Symbol = "BL",
                    CreatedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                    ModifiedDate = new DateTime(2023, 10, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984)
                });

            modelBuilder.Entity<WalletSettingDetail>().HasData(
                new WalletSettingDetail
                {
                    Id = Guid.Parse("5F65FED1-BDDC-4C7C-BA57-CB33A54542C8"),
                    Name = nameof(WalletSettingDetailType.WithDrawNotify),
                    Type = WalletSettingDetailType.WithDrawNotify,
                    Value = "dev@angelpj.com",
                    Description = "With draw notify list email",
                    CreatedDate = new DateTime(2023, 12, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984),
                    ModifiedDate = new DateTime(2023, 12, 11, 7, 33, 35, 791, DateTimeKind.Utc).AddTicks(4984)
                });


            modelBuilder.Entity<PremiumPackage>().HasData(
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
            modelBuilder.Entity<UserPremiumPackage>(entity =>
            {
                entity.HasOne(x => x.PremiumPackage)
                .WithMany()
                .HasForeignKey(x => x.PremiumPackageNo);

                entity.HasOne(x => x.UserWallet)
                .WithMany()
                .HasForeignKey(x => x.UserWalletId);
            });
            modelBuilder.Entity<WalletTransaction>(entity =>
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

            modelBuilder.Entity<WalletTransactionOtp>(entity =>
            {
                entity.HasOne(x => x.WalletTransaction)
                .WithMany(x => x.WalletTransactionOtps)
                .HasForeignKey(x => x.TransactionId);

                entity.HasIndex(x => x.Otp);
                entity.HasIndex(x => x.OtpToken);
            });

            modelBuilder.Entity<UserPurchaseTransaction>(entity =>
            {
                entity.HasOne(x => x.WalletTransaction)
                .WithMany(x => x.UserPurchaseTransactions)
                .HasForeignKey(x => x.WalletTransactionId);
                entity.HasIndex(x => new { x.CreatorUserId, x.CreatedDate });
            });

            modelBuilder.Entity<UserWallet>(entity =>
            {
                entity.HasIndex(x => x.Address).IsUnique();
            });

            modelBuilder.Entity<EarningPeriod>(entity =>
            {
                entity.HasIndex(x => new { x.Year, x.Order, x.FromDate, x.ToDate }).IsUnique();
            });
            modelBuilder.Entity<EarningSummary>(entity =>
            {
                entity.HasIndex(x => new { x.UserId, x.PeriodId }).IsUnique();
            });
            modelBuilder.Entity<EarningSummaryDetail>(entity =>
            {
                entity.HasIndex(x => x.EarningSummaryId);
                entity.HasOne(x => x.EarningSummary)
              .WithMany(x => x.SummaryDetails)
              .HasForeignKey(x => x.EarningSummaryId);

            });
        }
    }
}