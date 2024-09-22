namespace Mcsg.Wallet.Api.Interfaces;

using Domain.Entities;
using Domain.Enums;
using Models;

public interface IOtpService
{
    Task<TransactionOtpInfoResp> CreateAsync(WalletTransaction transaction, TransactionOtpType type, string otpToken = "");
    Task<bool> ClearAllTransactionOtpOtpAsync(Guid transactionId);
}
