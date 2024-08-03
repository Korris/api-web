namespace Mcsg.Media.Tool;

using Common.SeedWork;
using Interfaces;

/// <summary>
/// Setting
/// </summary>
public class Setting : SettingBase, ISetting
{
    #region -- Implements --

    /// <summary>
    /// AppName
    /// </summary>
    public string AppName { get; }

    /// <summary>
    /// AppVersion
    /// </summary>
    public string AppVersion { get; }

    /// <summary>
    /// PoolSize
    /// </summary>
    public int PoolSize { get; }

    /// <summary>
    /// Default connection
    /// </summary>
    public string DefaultConnection { get; set; }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public Setting()
    {
        AppName = "Mcsg.Media.Tool";
        AppVersion = "1.0.0";
        PoolSize = 5;
        DefaultConnection = "Server={DbServer};Database={DbName};Port={DbPort};User Id={DbUser};Password={DbPassword};MaxPoolSize=100;MinPoolSize=10;ConnectionLifetime=300;";
    }

    #endregion
}
