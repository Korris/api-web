namespace Mcsg.Api.Areas.Game.Requests;

using Common.Core.Requests;

/// <summary>
/// Paged list request for game posts
/// </summary>
public class GamePostListR : PaginatedR
{
    #region -- Properties --

    /// <summary>
    /// Optional: only posts created by this profile name
    /// </summary>
    public string? ProfileName { get; set; }

    /// <summary>
    /// Optional: search in title
    /// </summary>
    public string? Keyword { get; set; }

    #endregion
}
