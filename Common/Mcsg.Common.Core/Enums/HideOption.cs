namespace Mcsg.Common.Core.Enums;

/// <summary>
/// Hide option
/// </summary>
public enum HideOption
{
    /// <summary>
    /// None
    /// </summary>
    None,

    /// <summary>
    /// iOS
    /// </summary>
    Ios = 1 << 0,

    /// <summary>
    /// Android
    /// </summary>
    Android = 1 << 1,

    /// <summary>
    /// Web
    /// </summary>
    Web = 1 << 2,

    /// <summary>
    /// Mobile
    /// </summary>
    Mobile = Ios | Android,

    /// <summary>
    /// All
    /// </summary>
    All = Web | Mobile
}
