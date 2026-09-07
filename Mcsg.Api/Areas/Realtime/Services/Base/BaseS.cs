namespace Mcsg.Api.Areas.Realtime.Services;

using Common.Core.Interfaces;
using Common.Domain;
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
/// Base service with setting
/// </summary>
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
/// Base service with setting and storage
/// </summary>
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
