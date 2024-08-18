namespace Mcsg.Social.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class PostSearchHashTagR : PagingR
{
    #region -- Properties --

    /// <summary>
    /// Tag
    /// </summary>
    public string Tag { get; set; } = default!;

    #endregion
}
