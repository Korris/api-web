namespace Mcsg.Wallet.Api.Interfaces;

using Lib.Data.Wallet.Entities;
using Lib.Data.Wallet.Enums;
using Models;

public interface IOtpService
{
    Task<TransactionOtpInfoResp> CreateAsync(WalletTransaction transaction, TransactionOtpType type, string otpToken = "");
    Task<bool> ClearAllTransactionOtpOtpAsync(Guid transactionId);
}
