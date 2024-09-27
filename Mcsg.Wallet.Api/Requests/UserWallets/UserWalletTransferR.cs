namespace Mcsg.Wallet.Api.Requests;

using Common.Core.Requests;

public class UserWalletTransferR : BaseR
{
    public string ToAddress { get; set; }
    public float Amount { get; set; }
    public string Content { get; set; }
}
