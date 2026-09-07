namespace Mcsg.Api.Areas.Comic.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class CommentCheckPostExistedR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// PostId
    /// </summary>
    public Guid? PostId { get; set; }

    /// <summary>
    /// IsSubPost
    /// </summary>
    public bool IsSubPost { get; set; }

    #endregion
}
