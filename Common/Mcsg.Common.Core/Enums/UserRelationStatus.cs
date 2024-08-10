namespace Mcsg.Common.Core.Enums;

/// <summary>
/// Represents the status of the relationship between two users.
/// </summary>
public enum UserRelationStatus
{
    /// <summary>
    /// None -> (Pending | Blocked)
    /// </summary>
    None,

    /// <summary>
    /// Pending -> (Declined | Friend | Blocked | None)
    /// </summary>
    Pending,

    /// <summary>
    /// Declined -> (Pending | Blocked)
    /// </summary>
    Declined,

    /// <summary>
    /// Friend -> (None | Blocked | Muted | Family | Colleague | Acquaintance | CloseFriend)
    /// </summary>
    Friend,

    /// <summary>
    /// Blocked -> (All)
    /// </summary>
    Blocked,

    /// <summary>
    /// Muted -> (All)
    /// </summary>
    Muted,

    /// <summary>
    /// Family -> (None | Blocked | Friend | Colleague | Accquaintance | CloseFriend)
    /// </summary>
    Family,

    /// <summary>
    /// Colleague -> (None | Blocked | Friend | Family | Accquaintance | CloseFriend)
    /// </summary>
    Colleague,

    /// <summary>
    /// Acquaintance -> (None | Blocked | Friend | Family | Colleague | CloseFriend)
    /// </summary>
    Acquaintance,

    /// <summary>
    /// CloseFriend -> (None | Blocked | Friend | Family | Colleague | Acquaintance)
    /// </summary>
    CloseFriend
}
