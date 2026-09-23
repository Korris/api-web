namespace Mcsg.Api.Areas.TapShow.Constants;

/// <summary>
/// TapShow area runtime configuration (values assigned in Program.cs at startup) and fixed limits
/// </summary>
public static class TapShowConfig
{
    /// <summary>
    /// Allowed image extensions for uploads (comma separated), read by MediaOnlyAttribute.
    /// Program.cs reuses Minio.ComicMediaExtensionAllow.
    /// </summary>
    public static string MediaExtensionAllow { get; set; } = string.Empty;

    /// <summary>
    /// SystemSettings key holding the max image size in MB (shared with Comic thumbnails / covers)
    /// </summary>
    public const string ImageSizeSettingKey = "ThumbnailCoverSize";

    /// <summary>
    /// Images are re-encoded to JPEG, so the stored key always uses this extension
    /// </summary>
    public const string ImageExtension = ".jpg";

    /// <summary>
    /// Content type of the stored image
    /// </summary>
    public const string ImageContentType = "image/jpeg";

    /// <summary>
    /// Image folder under the user folder (thumbnails and segment images share it)
    /// </summary>
    public const string ImageFolder = "images";

    /// <summary>
    /// Allowed extensions for the segment voice-over upload (lower-case, no dot)
    /// </summary>
    public static readonly string[] AudioExtensionAllow = { "mp3", "m4a", "aac", "wav", "ogg" };

    /// <summary>
    /// Content type stored on the uploaded audio object, by extension
    /// </summary>
    public static readonly Dictionary<string, string> AudioContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["mp3"] = "audio/mpeg",
        ["m4a"] = "audio/mp4",
        ["aac"] = "audio/aac",
        ["wav"] = "audio/wav",
        ["ogg"] = "audio/ogg"
    };

    /// <summary>
    /// SystemSettings key holding the max audio size in MB (editable at runtime like GameFileSize)
    /// </summary>
    public const string AudioSizeSettingKey = "TapShowAudioSize";

    /// <summary>
    /// Fallback max audio size (MB) when the setting row is missing or 0
    /// </summary>
    public const double AudioDefaultMaxMb = 10;

    /// <summary>
    /// Hard ceiling (MB) for the audio setting; matches the Kestrel request cap below
    /// </summary>
    public const double AudioCeilingMb = 30;

    /// <summary>
    /// Kestrel request cap for upload-audio: ceiling + multipart overhead
    /// </summary>
    public const long AudioRequestLimitBytes = 31L * 1024 * 1024;

    /// <summary>
    /// Audio folder under the user folder
    /// </summary>
    public const string AudioFolder = "audios";

    /// <summary>
    /// Error message when the uploaded audio extension is not allowed
    /// </summary>
    public const string OnlyAudioFileMessage = "Only mp3, m4a, aac, wav, ogg files are allowed";

    /// <summary>
    /// Max outgoing choices of one segment
    /// </summary>
    public const int MaxChoicesPerSegment = 8;

    /// <summary>
    /// Max narration length of one segment
    /// </summary>
    public const int NarrationMaxLength = 4000;

    /// <summary>
    /// Max inline characters accepted in one POST / PUT api/tapshow/tapshow request
    /// </summary>
    public const int MaxCharactersPerPost = 50;

    /// <summary>
    /// Error message when the uploaded file extension is not allowed
    /// </summary>
    public const string OnlyMediaFileMessage = "Only media files are allowed.";

    /// <summary>
    /// Error message when a hashId is not a file this user uploaded through the TapShow upload API
    /// </summary>
    public const string InvalidResourceUrlMessage = "ThumbnailHashId / ImageHashId / AvatarHashId / AudioHashId must reference your own uploads of the right type from the tapshow upload API";

    /// <summary>
    /// Error message when a choice targets a segment outside the chapter, itself, or a duplicate target
    /// </summary>
    public const string InvalidChoiceTargetMessage = "Each choice must target a different existing segment of the same chapter (not itself)";

    /// <summary>
    /// Error message when a segment references a character that is not a live character of the same post
    /// </summary>
    public const string InvalidCharacterMessage = "CharacterId must be a character of the same post";

    /// <summary>
    /// Error message when IsEnding is set on a segment that still has choices
    /// </summary>
    public const string EndingWithChoicesMessage = "An ending segment cannot have choices; clear its choices first";

    /// <summary>
    /// Error message when a segment has neither image nor narration
    /// </summary>
    public const string EmptySegmentMessage = "A segment needs an image or a narration";
}
