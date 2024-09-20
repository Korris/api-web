namespace Mcsg.Wallet.Api.Interfaces;

using Common.Core.Enums;
using Lib.Common.Models;
using Models._3rdClass.ZaloPay.Response;

public interface IZaloPayService
{
    Task CompleteTransactionAsync(Guid transactionId, Guid userId, TransactionStatus status);
    Task<CreateOrderResponse> CreateOrderAsync(Guid transactionId, string content, float amount, Guid? userId, string redirectUrl);
    Task<QueryZalopayPayResponse> QueryOrderAsync(Guid transactionId);
}
