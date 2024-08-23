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
    /// IosAndroid
    /// </summary>
    IosAndroid = Ios | Android,

    /// <summary>
    /// Web
    /// </summary>
    Web = 1 << 2,

    /// <summary>
    /// Ios and Web
    /// </summary>
    IosWeb = Ios | Web,

    /// <summary>
    /// Android and Web
    /// </summary>
    AndroidWeb = Android | Web,

    /// <summary>
    /// All
    /// </summary>
    All = Ios | Android | Web
}
