namespace Mcsg.Wallet.Api.Requests;

using Common.Core.Requests;

public class UserWalletGetUserWalletAddressByUserIdR : BaseR
{
    public Guid WalletOwnerId { get; set; }
}
