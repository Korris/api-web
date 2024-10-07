#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-18 08:13
 * Update       : 2024-Jan-18 08:13
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Mcsg.Wallet.Domain.Interfaces;

using Domain.Entities;

/// <summary>
/// Interface WalletContext
/// </summary>
public interface IWalletContext
{
    #region -- Methods --

    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of state entries written to the database</returns>
    int SaveChanges();

    /// <summary>
    /// Make serial number
    /// </summary>
    /// <param name="q">Queryable</param>
    /// <param name="sOrderBy">Selector for OrderBy statement</param>
    /// <param name="sSelect">Selector for Select statement</param>
    /// <param name="prefix">Prefix</param>
    /// <param name="useDateTime">Use DateTime</param>
    /// <returns>Return the result</returns>
    string MakeNo<T>(IQueryable<T> q, Func<T, ulong> sOrderBy, Func<T, string> sSelect, string prefix, bool useDateTime = false);

    #endregion

    #region -- Properties --

    /// <summary>
    /// Database
    /// </summary>
    DatabaseFacade Database { get; }

    #region -- DbSet --
    DbSet<UserWallet> UserWallets { get; set; }
    DbSet<PremiumPackage> PremiumPackages { get; set; }
    DbSet<UserPremiumPackage> UserPremiumPackages { get; set; }
    DbSet<WalletTransaction> WalletTransactions { get; set; }
    DbSet<WalletTransactionOtp> WalletTransactionOtps { get; set; }
    DbSet<PaymentMethod> PaymentMethods { get; set; }
    DbSet<WalletSetting> WalletSettings { get; set; }
    DbSet<WalletSettingDetail> WalletSettingDetails { get; set; }
    DbSet<UserPaymentMethod> UserPaymentMethods { get; set; }
    DbSet<UserPurchaseTransaction> UserPurchaseTransactions { get; set; }
    DbSet<EarningPeriod> EarningPeriods { get; set; }
    DbSet<EarningSummary> EarningSummaries { get; set; }
    DbSet<EarningSummaryDetail> EarningSummaryDetails { get; set; }

    DbSet<Job> Jobs { get; set; }
    DbSet<SystemSetting> SystemSettings { get; set; }
    #endregion

    #region -- IQueryable --
    IQueryable<Job> JobAvailable { get; }
    IQueryable<SystemSetting> SystemSettingAvailable { get; }
    #endregion

    #endregion
}
