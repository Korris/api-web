namespace Mcsg.Api.Areas.Game.Constants;

/// <summary>
/// Game area runtime configuration (values assigned in Program.cs at startup)
/// </summary>
public static class GameConfig
{
    /// <summary>
    /// Allowed image extensions for thumbnail upload (comma separated), read by MediaOnlyAttribute.
    /// Program.cs reuses Minio.ComicMediaExtensionAllow.
    /// </summary>
    public static string MediaExtensionAllow { get; set; } = string.Empty;

    /// <summary>
    /// Only extension accepted for the game file upload
    /// </summary>
    public const string GameFileExtension = ".html";

    /// <summary>
    /// Content type stored on the uploaded game object so browsers render it inside the sandboxed iframe.
    /// Must stay parameter-free: Minio SDK 6.0.5 signs the raw value but cannot build a MediaTypeHeaderValue
    /// from "text/html; charset=utf-8", so it appends it next to the default "text/plain; charset=utf-8"
    /// and Minio answers SignatureDoesNotMatch. The uploaded HTML must declare its own &lt;meta charset&gt;.
    /// </summary>
    public const string GameFileContentType = "text/html";

    /// <summary>
    /// SystemSettings key holding the max game file size in MB (editable at runtime like ThumbnailCoverSize)
    /// </summary>
    public const string GameFileSizeSettingKey = "GameFileSize";

    /// <summary>
    /// Fallback max size (MB) when the setting row is missing or 0
    /// </summary>
    public const double GameFileDefaultMaxMb = 30;

    /// <summary>
    /// Hard ceiling (MB): the setting can never exceed this, it matches the Kestrel request cap below
    /// </summary>
    public const double GameFileCeilingMb = 50;

    /// <summary>
    /// Kestrel request cap for upload-game (compile-time constant): ceiling + multipart overhead
    /// </summary>
    public const long GameFileRequestLimitBytes = 51L * 1024 * 1024;

    /// <summary>
    /// Thumbnails are re-encoded to JPEG, so the stored key always uses this extension
    /// </summary>
    public const string ThumbnailExtension = ".jpg";

    /// <summary>
    /// Content type of the stored thumbnail
    /// </summary>
    public const string ThumbnailContentType = "image/jpeg";

    /// <summary>
    /// Thumbnail folder under the user folder
    /// </summary>
    public const string ThumbnailFolder = "thumbnails";

    /// <summary>
    /// Game file folder under the user folder
    /// </summary>
    public const string GameFolder = "games";

    /// <summary>
    /// Error message when the uploaded game file is not .html
    /// </summary>
    public const string OnlyHtmlFileMessage = "Only .html file is allowed";

    /// <summary>
    /// Error message when the thumbnail extension is not allowed
    /// </summary>
    public const string OnlyMediaFileMessage = "Only media files are allowed.";

    /// <summary>
    /// Error message when ThumbnailUrl/GameUrl is not a file this user uploaded through the game upload API
    /// </summary>
    public const string InvalidResourceUrlMessage = "ThumbnailHashId and GameHashId must reference your own uploads from the game upload API";
}
