namespace Mcsg.Wallet.Api.Interfaces;

using Mcsg.Lib.Data.Wallet.Entities;
using Mcsg.Lib.Data.Wallet.Enums;
using Models;

public interface IOtpService
{
    Task<TransactionOtpInfoResp> CreateAsync(WalletTransaction transaction, TransactionOtpType type, string otpToken = "");
    Task<bool> ClearAllTransactionOtpOtpAsync(Guid transactionId);
}
