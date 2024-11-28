using Mcsg.Common.Core.Enums;

namespace Mcsg.Common.Domain;

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

    #endregion

    #region -- Fields --

    /// <summary>
    /// PostStatuses (Inactive, Public)
    /// </summary>
    public static List<PostStatus> PostStatuses = [PostStatus.Inactive, PostStatus.Public];

    #endregion
}
