namespace Mcsg.Api.Areas.Social.Models;

using Common.Core.Enums;

/// <summary>
/// A chapter (sub post) scheduled to be published in the future, with the parent post it belongs to.
/// Deliberately image-free: the "coming soon" list only needs titles and identifiers.
/// </summary>
public class UpcomingSubPostResponse
{
    /// <summary>
    /// Sub post id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Sub post hash id
    /// </summary>
    public string? HashId { get; set; }

    /// <summary>
    /// Sub post (chapter) title
    /// </summary>
    public string? SubPostTitle { get; set; }

    /// <summary>
    /// Parent post id
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// Parent post hash id
    /// </summary>
    public string? PostHashId { get; set; }

    /// <summary>
    /// Parent post (series) title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Story, Comic or Document
    /// </summary>
    public PostType Type { get; set; }

    /// <summary>
    /// When the sub post becomes visible (UTC)
    /// </summary>
    public DateTime PublishDate { get; set; }
}
