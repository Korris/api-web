namespace Mcsg.Wallet.Api.Requests;

public class UserWalletTransferR
{
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }
    public float Amount { get; set; }
    public string Content { get; set; }
}
