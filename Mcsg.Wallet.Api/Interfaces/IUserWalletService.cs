namespace Mcsg.Wallet.Api.Interfaces;

using Api.Models._3rdClass.ZaloPay.Response;
using Domain.Enums;
using Lib.Common.Models;
using Models;
using Requests;

public interface IUserWalletService
{
    Task<IEnumerable<UserWalletResp>> GetUserWalletAsync();
    Task<UserWalletBasicResp> GetUserWalletByAddressAsync(string address);
    Task<PaginatedList<UserWalletTransactionItemResp>> GetUserWalletTransactionsAsync(int page = 1, int pageSize = 10);
    Task<UserWalletTransactionItemResp> GetUserWalletTransactionByRefNumberAsync(string referenceNumber);
    Task<IEnumerable<UserPaymentMethodResponse>> GetUserPaymentMethods();
    Task<AddUserPaymentMethodResp> AddUserPaymentMethod(UserWalletAddPaymentMethodR addUserPaymentMethodReq);
    Task<UpdateUserPaymentMethodResp> UpdateUserPaymentMethod(Guid userPaymentMethodId, UserWalletUpdatePaymentMethodR updateUserPaymentMethodReq);
    Task<bool> RemoveUserPaymentMethod(Guid userPaymentMethodId);
    Task<UserWalletBasicResp> GetUserWalletAddress(Guid userId);

    Task<bool> VerifyTransactionOtpAsync(UserWalletVerifyTransactionOtpR req);
    Task<TransactionOtpInfoResp> ResentTransactionOtpAsync(Guid transactionId, TransactionOtpType otpType);
    Task<TransactionOtpInfoResp> DonateAsync(UserWalletDonateR req);
    Task<TransactionOtpInfoResp> TransferAsync(UserWalletTransferR req);
    Task<WithDrawPrepareResp> WithdrawPrepareAsync();
    Task<TransactionOtpInfoResp> WithdrawAsync(UserWalletWithdrawR req);
    Task<DepositPrepareResp> DepositPrepareAsync();
    Task<DepositResp> DepositAsync(UserWalletDepositR req);
    Task<bool> DepositCancelAsync(UserWalletDepositCancelR req);
    Task<CallBackZaloPayResponse> CallBackZaloPayAsync(UserWalletZaloPayCallBackR req);
    Dictionary<string, object> CallBackZaloPay(dynamic cbdata);
}
