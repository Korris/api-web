namespace Mcsg.Common.Core.Enums;

/// <summary>
/// User status
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// Active
    /// </summary>
    Active = 1,

    /// <summary>
    /// Suspended
    /// </summary>
    Suspended,

    /// <summary>
    /// Banned
    /// </summary>
    Banned,

    /// <summary>
    /// Deleted (cannot restore)
    /// </summary>
    Deleted
}
