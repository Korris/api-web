namespace Mcsg.Api.Areas.TapShow.Requests;

using Common.Core.Enums;
using Common.Core.Requests;

/// <summary>
/// Shared fields for create / update chapter
/// </summary>
public class ChapterFormBaseR : IdBaseR
{
    #region -- Properties --

    /// <summary>
    /// Chapter title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Position among the chapters of the post; null on create → appended at the end
    /// </summary>
    public float? Order { get; set; }

    /// <summary>
    /// Draft (owner only) or Public
    /// </summary>
    public PostStatus Status { get; set; } = PostStatus.Draft;

    /// <summary>
    /// hashId returned by POST api/tapshow/file/upload-media; null → no thumbnail.
    /// On update: same hashId keeps the current thumbnail, a new one replaces it, null removes it.
    /// </summary>
    public string? ThumbnailHashId { get; set; }

    #endregion
}
