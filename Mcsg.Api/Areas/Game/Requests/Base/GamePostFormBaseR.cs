namespace Mcsg.Api.Areas.Game.Requests;

using Common.Core.Enums;
using Common.Core.Requests;

/// <summary>
/// Shared fields for create / update game post (mirrors ComicPostFormBaseR without cover / chapters)
/// </summary>
public class GamePostFormBaseR : IdBaseR
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
    /// hashId returned by POST api/game/file/upload-thumbnail (same flow as Comic ThumbnailHashId)
    /// </summary>
    public string? ThumbnailHashId { get; set; }

    /// <summary>
    /// hashId returned by POST api/game/file/upload-game (the .html file)
    /// </summary>
    public string? GameHashId { get; set; }

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
    /// Permission (Public / Private / Premium)
    /// </summary>
    public PostPermission Permission { get; set; }

    /// <summary>
    /// Hashtags (with or without leading '#'), max Validator.Hashtag.MaxQuantity. PUT replaces the whole set; null / empty → no tags.
    /// </summary>
    public List<string>? Tags { get; set; }

    #endregion
}
