namespace Mcsg.Admin.Api.Interfaces;

using Common.SeedWork.Interfaces;
using static Common.SeedWork.Dtos.ConnectionDto;

/// <summary>
/// Interface setting
/// </summary>
public interface ISetting : ISettingBase
{
    #region -- Properties --

    /// <summary>
    /// Database Wallet
    /// </summary>
    DatabaseDto DbWallet { get; }

    #endregion
}
