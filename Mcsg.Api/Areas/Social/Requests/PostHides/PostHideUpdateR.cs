namespace Mcsg.Api.Areas.Social.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class PostHideUpdateR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// PostId
    /// </summary>
    public Guid PostId { get; set; }

    #endregion
}
