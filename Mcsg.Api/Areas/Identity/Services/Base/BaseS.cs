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

namespace Mcsg.Api.Areas.Identity.Services;

using Common.Core.Interfaces;
using Common.Domain;
using Mcsg.Api.Areas.Identity.Interfaces;
using Mcsg.Api.Interfaces;

/// <summary>
/// Base service
/// </summary>
/// <remarks>
/// Initialize
/// </remarks>
/// <param name="context">DB context</param>
public abstract class BaseS(IMcsgContext context)
{
    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    protected readonly IMcsgContext _context = context;

    #endregion
}

/// <summary>
/// Base service
/// </summary>
/// <remarks>
/// Initialize
/// </remarks>
/// <param name="context">DB context</param>
/// <param name="setting">Setting</param>
public abstract class BaseSettingS(IMcsgContext context, ISetting setting) : BaseS(context)
{
    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    protected readonly ISetting _setting = setting;

    #endregion
}

/// <summary>
/// Base service
/// </summary>
/// <remarks>
/// Initialize
/// </remarks>
/// <param name="context">DB context</param>
/// <param name="setting">Setting</param>
/// <param name="sc">Storage client</param>
public abstract class BaseMinioS(IMcsgContext context, ISetting setting, IStorageClient sc) : BaseSettingS(context, setting)
{
    #region -- Fields --

    /// <summary>
    /// Storage client
    /// </summary>
    protected readonly IStorageClient _sc = sc;

    #endregion
}
