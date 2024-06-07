#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 07:03
 * Update       : 2024-Jan-21 07:03
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using Microsoft.EntityFrameworkCore.Storage;

namespace Mcsg.Common.Core.Extensions;

/// <summary>
/// IDbContextTransaction extension for using [this IDbContextTransactionExtension] only
/// </summary>
public static class IDbContextTransactionExtension
{
    /// <summary>
    /// Do transaction
    /// </summary>
    /// <param name="transaction">IDbContextTransaction</param>
    /// <param name="isRollback">If true, rollback; else, commit</param>
    public static void DoTransaction(this IDbContextTransaction transaction, bool isRollback)
    {
        if (isRollback)
        {
            transaction.Rollback();
        }
        else
        {
            transaction.Commit();
        }
    }
}
