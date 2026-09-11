namespace Mcsg.Api.Areas.Document.Requests;

using Common.Core.Requests;

/// <summary>
/// PATCH v1/LoadFeed request (mobile feed of another user's / public posts).
/// Filter: PostFilter.Search (UserId | UserName, Keyword, Hashtag)
/// </summary>
public class PostLoadFeedR : PagingR
{
    #region -- Properties --

    /// <summary>
    /// Only posts the current user has favorited
    /// </summary>
    public bool IsFavorite { get; set; }

    #endregion
}
