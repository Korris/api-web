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

    /// <summary>
    /// AppName
    /// </summary>
    string AppName { get; }

    /// <summary>
    /// AppVersion
    /// </summary>
    string AppVersion { get; }

    /// <summary>
    /// PoolSize
    /// </summary>
    int PoolSize { get; }

    /// <summary>
    /// Default connection
    /// </summary>
    string DefaultConnection { get; set; }

    #endregion
}
