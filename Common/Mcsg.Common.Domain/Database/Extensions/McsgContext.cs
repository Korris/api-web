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

namespace Mcsg.Common.Domain;

using Core.Enums;
using Entities;
using SeedWork;
using SeedWork.Extensions;

/// <summary>
/// McsgContext
/// </summary>
partial class McsgContext
{
    #region -- Implements --

    /// <summary>
    /// Get UserId
    /// </summary>
    /// <param name="userName">UserName</param>
    /// <returns>Return the UserId</returns>
    public async Task<Guid?> GetUserId(string? userName)
    {
        return await UserAvailable.Where(p => p.UserName == userName).Select(p => (Guid?)p.Id).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Make serial number
    /// </summary>
    /// <param name="q">Queryable</param>
    /// <param name="sOrderBy">Selector for OrderBy statement</param>
    /// <param name="sSelect">Selector for Select statement</param>
    /// <param name="prefix">Prefix</param>
    /// <returns>Return the result</returns>
    public string MakeNo<T>(IQueryable<T> q, Func<T, Guid> sOrderBy, Func<T, string> sSelect, string prefix)
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
        var ym = DateTime.Now.ToString("yyyyMM");
        prefix += ym + "-{0:0000#}";

        // First
        var m = q.OrderBy(sOrderBy).Select(sSelect).LastOrDefault();
        if (m == null)
        {
            return string.Format(prefix, 1);
        }

        // Next
        var arr = m.Split('-').LastOrDefault();
        var num = arr == null ? "0" : arr.ToNumber();
        var seq = Convert.ToUInt32(num) + 1;
        return string.Format(prefix, seq);
    }

    /// <summary>
    /// Set
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <returns>Return the result</returns>
    public new DbSet<T> Set<T>() where T : class
    {
        return base.Set<T>();
    }

    /// <summary>
    /// Available
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    /// <param name="isTracking">Is tracking</param>
    /// <returns>Return the result</returns>
    public IQueryable<T> Available<T>(bool isTracking = true) where T : AuditableEntity
    {
        var res = base.Set<T>().Where(p => !p.IsDelete);

        if (!isTracking)
        {
            res = res.AsNoTracking();
        }

        return res;
    }

    /// <summary>
    /// GetSettingDecimal
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>Return the result</returns>
    public async Task<decimal> GetSettingDecimal(string key)
    {
        var value = await Available<SystemSetting>().Where(p => p.Key == key).Select(p => p.Value).FirstOrDefaultAsync();
        return value.Cast<decimal?>("decimal") ?? 0;
    }

    /// <summary>
    /// GetSettingDouble
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>Return the result</returns>
    public async Task<double> GetSettingDouble(string key)
    {
        var value = await Available<SystemSetting>().Where(p => p.Key == key).Select(p => p.Value).FirstOrDefaultAsync();
        return value.Cast<double?>("double") ?? 0;
    }

    /// <summary>
    /// GetSettingInt
    /// </summary>
    /// <param name="key">Key</param>
    /// <returns>Return the result</returns>
    public async Task<int> GetSettingInt(string key)
    {
        var value = await Available<SystemSetting>().Where(p => p.Key == key).Select(p => p.Value).FirstOrDefaultAsync();
        return value.Cast<int?>("int") ?? 0;
    }

    #region -- IQueryable --
    public IQueryable<User> UserAvailable => Users.Where(p => p.Status != UserStatus.WillDelete && p.Status != UserStatus.Deleted);
    #endregion

    #endregion
}
