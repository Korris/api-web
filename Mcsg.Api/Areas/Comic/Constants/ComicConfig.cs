namespace Mcsg.Api.Areas.Comic.Constants;

/// <summary>
/// Static configuration holder for Comic area.
/// Values are initialized during application startup via DI extension methods.
/// </summary>
public static class ComicConfig
{
    /// <summary>
    /// Allowed media file extensions (comma-separated), e.g. "jpg,jpeg,png,mp4"
    /// </summary>
    public static string MediaExtensionAllow { get; set; } = string.Empty;
}
