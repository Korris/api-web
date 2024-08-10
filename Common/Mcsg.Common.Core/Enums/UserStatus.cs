namespace Mcsg.Common.Core.Enums;

/// <summary>
/// Represents the different statuses a user can have.
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// The user is currently active.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The user is suspended and cannot use the service.
    /// </summary>
    Suspended,

    /// <summary>
    /// The user is banned and cannot access the service.
    /// </summary>
    Banned,

    /// <summary>
    /// The user is scheduled for deletion.
    /// </summary>
    WillDelete,

    /// <summary>
    /// The user is deleted and cannot be restored.
    /// </summary>
    Deleted
}
