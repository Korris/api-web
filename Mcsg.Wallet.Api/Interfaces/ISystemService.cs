namespace Mcsg.Wallet.Api.Interfaces;
public interface ISystemService
{
    Task<bool> SendAdminNoti(string action, string fromUser, string content);
}
