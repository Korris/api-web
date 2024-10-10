namespace Mcsg.Wallet.Api.Interfaces;

using Requests.Notification;

public interface INotificationService
{
    Task AddTransactionNotificationAsync(TransactionNotificationReq req);
}
