namespace Mcsg.Wallet.Api.Services;

using Domain;
using Interfaces;

/// <summary>
/// Base service
/// </summary>
/// <remarks>
/// Initialize
/// </remarks>
/// <param name="context">DB context</param>
public class BaseS(WalletContext context)
{
    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    protected readonly WalletContext _context = context;

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
public class BaseSettingS(WalletContext context, ISetting setting) : BaseS(context)
{
    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    protected readonly ISetting _setting = setting;

    #endregion
}
