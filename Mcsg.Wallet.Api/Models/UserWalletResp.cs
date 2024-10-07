namespace Mcsg.Wallet.Api.Models;

public class UserWalletResp
{
    public string WalletAddress { get; set; }
    public float Point { get; set; }
    public DateOnly? PremiumDate { get; set; }
    public float RewardPoint { get; set; }
    public float TotalPoint { get; set; }
    public string Owner { get; set; }
}
public class UserWalletBasicResp
{
    public string WalletAddress { get; set; }
    public string ProfileName { get; set; }
    public string Email { get; set; }
    public string? Avatar { get; set; }
    public Guid? UserId { get; set; }
}
