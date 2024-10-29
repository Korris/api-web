namespace Mcsg.Document.Api.Requests;

using Common.Core.Enums;

public class TrackingFollowReq
{
    #region -- Properties --

    /// <summary>
    /// MicroService
    /// </summary>
    public string? ServiceType { get; set; } = PostType.Document.ToString();

    /// <summary>
    /// PostId
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// FollowId
    /// </summary>
    public Guid FollowId { get; set; }

    /// <summary>
    /// SessionId
    /// </summary>
    public Guid? SessionId { get; set; }

    #endregion
}
