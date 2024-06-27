namespace Mcsg.Media.Tool;

using Common.Core.Dtos;
using Common.SeedWork;
using Interfaces;

/// <summary>
/// Setting
/// </summary>
public class Setting : SettingBase, ISetting
{
    #region -- Implements --

    /// <summary>
    /// API
    /// </summary>
    public ApiDto Api { get; }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public Setting()
    {
        Api = new ApiDto();
    }

    #endregion
}
