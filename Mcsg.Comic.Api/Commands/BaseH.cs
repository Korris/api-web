#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

namespace Mcsg.Comic.Api.Commands;

using Common.Core.Interfaces;
using Interfaces;
using Lib.Data;

/// <summary>
/// Base handler
/// </summary>
/// <remarks>
/// Initialize
/// </remarks>
/// <param name="context">DB context</param>
public abstract class BaseH(McsgDbContext context)
{
    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    protected readonly McsgDbContext _context = context;

    #endregion
}

/// <summary>
/// Base handler
/// </summary>
/// <remarks>
/// Initialize
/// </remarks>
/// <param name="context">DB context</param>
/// <param name="setting">Setting</param>
public abstract class BaseSettingH(McsgDbContext context, ISetting setting) : BaseH(context)
{
    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    protected readonly ISetting _setting = setting;

    #endregion
}

/// <summary>
/// Base handler
/// </summary>
/// <remarks>
/// Initialize
/// </remarks>
/// <param name="context">DB context</param>
/// <param name="setting">Setting</param>
/// <param name="sc">Storage client</param>
public abstract class BaseMinioH(McsgDbContext context, ISetting setting, IStorageClient sc) : BaseSettingH(context, setting)
{
    #region -- Fields --

    /// <summary>
    /// Storage client
    /// </summary>
    protected readonly IStorageClient _sc = sc;

    #endregion
}
