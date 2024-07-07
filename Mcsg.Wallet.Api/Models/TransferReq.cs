namespace Mcsg.Wallet.Api.Models;

public class TransferReq
{
    public string FromAddress { get; set; }
    public string ToAddress { get; set; }
    public float Amount { get; set; }
    public string Content { get; set; }
}
