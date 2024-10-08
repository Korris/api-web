namespace Mcsg.Wallet.Api.Requests;

using Common.Core.Requests;

public class UserWalletGetInfoByAddressR : BaseR
{
    public string Address { get; set; }
}
