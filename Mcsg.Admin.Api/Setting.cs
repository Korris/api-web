namespace Mcsg.Admin.Api;

using Common.SeedWork;
using Interfaces;
using static Common.SeedWork.Dtos.ConnectionDto;

/// <summary>
/// Setting
/// </summary>
public class Setting : SettingBase, ISetting
{
    #region -- Implements --

    /// <summary>
    /// Database Wallet
    /// </summary>
    public DatabaseDto DbWallet { get; }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public Setting()
    {
        DbWallet = new DatabaseDto();
    }

    #endregion
}
