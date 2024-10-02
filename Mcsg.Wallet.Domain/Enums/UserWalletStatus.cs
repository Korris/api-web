namespace Mcsg.Wallet.Domain.Enums;

/// <summary>
/// UserWalletStatus
/// </summary>
public enum UserWalletStatus
{
    /// <summary>
    /// Approved
    /// </summary>
    Approved,

    /// <summary>
    /// Blocked (for some reasons, the application can lock a user's wallet for investigation)
    /// </summary>
    Blocked
}
