namespace Mcsg.Analytic.Api.Interfaces;

using Common.SeedWork.Interfaces;
using static Common.SeedWork.Dtos.ConnectionDto;

/// <summary>
/// Interface setting
/// </summary>
public interface ISetting : ISettingBase
{
    #region -- Properties --

    /// <summary>
    /// Database Analytic
    /// </summary>
    DatabaseDto DbAnalytic { get; }

    #endregion
}
