namespace Mcsg.Wallet.Api.Requests;

using Common.Core.Requests;

public class UserWalletDonateR : BaseR
{
    public Guid ToUserId { get; set; }
    public float Amount { get; set; }
    public string? Content { get; set; }
}
