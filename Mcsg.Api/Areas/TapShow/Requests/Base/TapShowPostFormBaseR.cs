namespace Mcsg.Api.Areas.TapShow.Requests;

using Common.Core.Enums;
using Common.Core.Requests;

/// <summary>
/// Shared fields for create / update TapShow post (mirrors GamePostFormBaseR without the game file)
/// </summary>
public class TapShowPostFormBaseR : IdBaseR
{
    #region -- Properties --

    /// <summary>
    /// Title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Summary / description (stored in Body)
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// hashId returned by POST api/tapshow/file/upload-media
    /// </summary>
    public string? ThumbnailHashId { get; set; }

    /// <summary>
    /// True when the logged-in user is the author, otherwise AuthorName is used
    /// </summary>
    public bool IsCurrentUserAuthor { get; set; }

    /// <summary>
    /// Author name when the current user is not the author
    /// </summary>
    public string? AuthorName { get; set; } = string.Empty;

    /// <summary>
    /// Mature content flag
    /// </summary>
    public bool IsMature { get; set; } = false;

    /// <summary>
    /// Story finished (no more chapters coming)
    /// </summary>
    public bool IsCompleted { get; set; } = false;

    /// <summary>
    /// Permission (Public / Private / Premium)
    /// </summary>
    public PostPermission Permission { get; set; }

    /// <summary>
    /// Hashtags (with or without leading '#'), max Validator.Hashtag.MaxQuantity. PUT replaces the whole set; null / empty → no tags.
    /// </summary>
    public List<string>? Tags { get; set; }

    #endregion
}
