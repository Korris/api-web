namespace Mcsg.Wallet.Api.Requests;

public class UserWalletDonateR
{
    public Guid ToUserId { get; set; }
    public float Amount { get; set; }
    public string? Content { get; set; }
}
