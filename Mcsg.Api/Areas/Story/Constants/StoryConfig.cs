namespace Mcsg.Api.Areas.Story.Constants;

/// <summary>
/// Static configuration holder for Story area.
/// Values are initialized during application startup via DI extension methods.
/// </summary>
public static class StoryConfig
{
    /// <summary>
    /// Allowed media file extensions (comma-separated), e.g. "jpg,jpeg,png,mp4"
    /// </summary>
    public static string MediaExtensionAllow { get; set; } = string.Empty;
}
