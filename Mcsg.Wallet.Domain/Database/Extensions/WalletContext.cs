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

namespace Mcsg.Wallet.Domain;

using Common.SeedWork.Extensions;
using Domain.Entities;

partial class WalletContext
{
    #region -- Implements --

    /// <summary>
    /// Make serial number
    /// </summary>
    /// <param name="q">Queryable</param>
    /// <param name="sOrderBy">Selector for OrderBy statement</param>
    /// <param name="sSelect">Selector for Select statement</param>
    /// <param name="prefix">Prefix</param>
    /// <param name="useDateTime">Use DateTime</param>
    /// <returns>Return the result</returns>
    public string MakeNo<T>(IQueryable<T> q, Func<T, ulong> sOrderBy, Func<T, string> sSelect, string prefix, bool useDateTime = false)
    {
        // Prefix
        if (string.IsNullOrWhiteSpace(prefix))
        {
            prefix = "UN";
        }
        else
        {
            prefix = prefix.Trim();
        }

        if (useDateTime)
        {
            prefix += DateTime.Now.ToString("yyyyMM");
        }

        prefix += "-";
        var format = new string('0', 6);

        // First
        var m = q.OrderBy(sOrderBy).Select(sSelect).LastOrDefault();
        if (m == null)
        {
            return prefix + 1ul.ToString(format);
        }

        // Next
        var arr = m.Split('-').LastOrDefault();
        var num = arr == null ? "0" : arr.ToNumber();
        var seq = Convert.ToUInt64("0" + num) + 1;

        return prefix + seq.ToString(format);
    }

    #region -- IQueryable --
    public IQueryable<Job> JobAvailable => Jobs.Where(p => !p.IsDelete);
    public IQueryable<SystemSetting> SystemSettingAvailable => SystemSettings.Where(p => !p.IsDelete);
    #endregion

    #endregion
}
