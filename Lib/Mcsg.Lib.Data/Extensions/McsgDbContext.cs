namespace Mcsg.Lib.Data;

using Domain.Entities;

/// <summary>
/// McsgDbContext
/// </summary>
partial class McsgDbContext
{
    #region -- Properties --

    /// <summary>
    /// UserAvailable
    /// </summary>
    public IQueryable<User> UserAvailable => Users.Where(p => !p.IsDelete);

    /// <summary>
    /// UserFollowAvailable
    /// </summary>
    public IQueryable<UserFollow> UserFollowAvailable => UserFollows.Where(p => !p.IsDelete);

    #endregion
}
