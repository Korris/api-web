namespace Mcsg.Analytic.Api;

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
    /// Database Analytic
    /// </summary>
    public DatabaseDto DbAnalytic { get; }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public Setting()
    {
        DbAnalytic = new DatabaseDto();
    }

    #endregion
}
