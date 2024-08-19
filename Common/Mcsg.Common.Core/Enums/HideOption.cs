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
    iOS,

    /// <summary>
    /// Android
    /// </summary>
    Android,

    /// <summary>
    /// Web
    /// </summary>
    Web,

    /// <summary>
    /// Mobile
    /// </summary>
    Mobile = iOS | Android,

    /// <summary>
    /// All
    /// </summary>
    All = Web | Mobile
}
