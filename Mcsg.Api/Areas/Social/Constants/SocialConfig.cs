namespace Mcsg.Api.Areas.Social.Constants;

/// <summary>
/// Static configuration holder for Social area.
/// Values are initialized during application startup via DI extension methods.
/// </summary>
public static class SocialConfig
{
    /// <summary>
    /// Allowed media file extensions (comma-separated), e.g. "jpg,jpeg,png,mp4"
    /// </summary>
    public static string MediaExtensionAllow { get; set; } = string.Empty;
}
