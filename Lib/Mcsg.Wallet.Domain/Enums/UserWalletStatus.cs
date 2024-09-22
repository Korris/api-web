namespace Mcsg.Wallet.Domain.Enums;

public enum UserWalletStatus
{
    APPROVED = 0,
    BLOCKED = -1 // For some reasons, the application can lock a user's wallet for investigation.
}
