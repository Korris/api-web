namespace Mcsg.Api.Areas.Realtime.Commands;

using Common.Core.Interfaces;
using Common.Domain;
using Mcsg.Api.Interfaces;

/// <summary>
/// Base handler
/// </summary>
/// <param name="context">DB context</param>
public abstract class BaseH(IMcsgContext context)
{
    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    protected readonly IMcsgContext _context = context;

    #endregion
}

/// <summary>
/// Base handler with setting
/// </summary>
/// <param name="context">DB context</param>
/// <param name="setting">Setting</param>
public abstract class BaseSettingH(IMcsgContext context, ISetting setting) : BaseH(context)
{
    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    protected readonly ISetting _setting = setting;

    #endregion
}

/// <summary>
/// Base handler with setting and storage
/// </summary>
/// <param name="context">DB context</param>
/// <param name="setting">Setting</param>
/// <param name="sc">Storage client</param>
public abstract class BaseMinioH(IMcsgContext context, ISetting setting, IStorageClient sc) : BaseSettingH(context, setting)
{
    #region -- Fields --

    /// <summary>
    /// Storage client
    /// </summary>
    protected readonly IStorageClient _sc = sc;

    #endregion
}
