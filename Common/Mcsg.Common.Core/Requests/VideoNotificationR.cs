namespace Mcsg.Common.Core.Requests;

using Enums;

/// <summary>
/// VideoNotification request
/// </summary>
public class VideoNotificationR
{
    #region -- Properties --

    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Action
    /// </summary>
    public NotificationAction Action { get; set; }

    /// <summary>
    /// HashId
    /// </summary>
    public string? HashId { get; set; }

    /// <summary>
    /// PostId
    /// </summary>
    public Guid? PostId { get; set; }

    /// <summary>
    /// PostHashId
    /// </summary>
    public string? PostHashId { get; set; }

    /// <summary>
    /// TargetType
    /// </summary>
    public string? TargetType { get; set; }

    /// <summary>
    /// AuthorId
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// AuthorName
    /// </summary>
    public string? AuthorName { get; set; }

    /// <summary>
    /// UserAvatar
    /// </summary>
    public string? UserAvatar { get; set; }

    #endregion
}
