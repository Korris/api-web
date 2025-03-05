namespace Mcsg.Common.Domain;

using Core.Enums;

/// <summary>
/// StatusUtils
/// </summary>
public class StatusUtils
{
    #region -- Properties --

    /// <summary>
    /// PostStatusInt (Inactive, Public)
    /// </summary>
    public static List<int> PostStatusInt => PostStatuses.Select(p => (int)p).ToList();

    /// <summary>
    /// PostStatusIntPublic
    /// </summary>
    public static List<int> PostStatusIntPublic = [(int)PostStatus.Public];

    /// <summary>
    /// PostStatusesForAuthorInt (Inactive, Public, Draft)
    /// </summary>
    public static List<int> PostStatusForAuthorInt => PostStatusesForAuthor.Select(p => (int)p).ToList();

    #endregion

    #region -- Fields --

    /// <summary>
    /// PostStatuses (Inactive, Public)
    /// </summary>
    public static List<PostStatus> PostStatuses = [PostStatus.Inactive, PostStatus.Public];

    /// <summary>
    /// PostStatusesForAuthor (Inactive, Public, Draft)
    /// </summary>
    public static List<PostStatus> PostStatusesForAuthor = [PostStatus.Inactive, PostStatus.Public, PostStatus.Draft];

    #endregion
}
