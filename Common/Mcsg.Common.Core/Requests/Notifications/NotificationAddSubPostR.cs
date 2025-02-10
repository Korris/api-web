namespace Mcsg.Common.Core.Requests;

using Enums;

/// <summary>
/// Request
/// </summary>
public class NotificationAddSubPostR : BaseR
{
    /// <summary>
    /// FollowerUserIds
    /// </summary>
    public List<Guid>? FollowerUserIds { get; set; }

    /// <summary>
    /// PostType
    /// </summary>
    public PostType PostType { get; set; }

    /// <summary>
    /// AuthorId
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// SubPostId
    /// </summary>
    public Guid SubPostId { get; set; }

    /// <summary>
    /// PostId
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// PostName
    /// </summary>
    public string? PostName { get; set; }

    /// <summary>
    /// PostHashId
    /// </summary>
    public string? PostHashId { get; set; }

    /// <summary>
    /// Order
    /// </summary>
    public float Order { get; set; }

    /// <summary>
    /// PostThumbnailUrl
    /// </summary>
    public string? PostThumbnailUrl { get; set; }
}
