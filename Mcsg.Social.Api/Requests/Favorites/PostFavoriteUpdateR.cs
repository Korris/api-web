namespace Mcsg.Social.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class PostFavoriteUpdateR : BaseR
{
    #region -- Properties --

    /// <summary>
    /// PostId
    /// </summary>
    public Guid PostId { get; set; }

    #endregion
}
