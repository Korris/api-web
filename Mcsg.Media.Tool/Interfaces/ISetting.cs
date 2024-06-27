namespace Mcsg.Media.Tool.Interfaces;

using Common.Core.Dtos;
using Common.SeedWork.Interfaces;

/// <summary>
/// Interface setting
/// </summary>
public interface ISetting : ISettingBase
{
    #region -- Properties --

    /// <summary>
    /// API
    /// </summary>
    ApiDto Api { get; }

    #endregion
}
