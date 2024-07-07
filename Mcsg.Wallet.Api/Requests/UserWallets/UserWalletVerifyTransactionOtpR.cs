namespace Mcsg.Wallet.Api.Requests;

public class UserWalletVerifyTransactionOtpR
{
    public Guid TransactionId { get; set; }
    public string Otp { get; set; }
    public string OtpToken { get; set; }
}
