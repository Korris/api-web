using Mcsg.Common.Core.Enums;

namespace Mcsg.Common.Domain;

/// <summary>
/// StatusUtils
/// </summary>
public class StatusUtils
{
    #region -- Properties --

    /// <summary>
    /// PostStatusInt
    /// </summary>
    public static List<int> PostStatusInt => PostStatuses.Select(p => (int)p).ToList();

    #endregion

    #region -- Fields --

    /// <summary>
    /// PostStatuses
    /// </summary>
    public static List<PostStatus> PostStatuses = [PostStatus.Inactive, PostStatus.Public];

    #endregion
}
