namespace Mcsg.Wallet.Api.Requests;

using Common.Core.Requests;

public class UserWalletDepositCancelR : BaseR
{
    public Guid TransactionId { get; set; }
}
