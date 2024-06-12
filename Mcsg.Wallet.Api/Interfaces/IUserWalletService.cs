namespace Mcsg.Wallet.Api.Interfaces;

using Api.Models._3rdClass.ZaloPay.Request;
using Api.Models._3rdClass.ZaloPay.Response;
using Lib.Common.Models;
using Lib.Data.Wallet.Enums;
using Models;

public interface IUserWalletService
{
    Task<IEnumerable<UserWalletResp>> GetUserWalletAsync();
    Task<UserWalletBasicResp> GetUserWalletByAddressAsync(string address);
    Task<PaginatedList<UserWalletTransactionItemResp>> GetUserWalletTransactionsAsync(int page = 1, int pageSize = 10);
    Task<UserWalletTransactionItemResp> GetUserWalletTransactionByRefNumberAsync(string referenceNumber);
    Task<IEnumerable<UserPaymentMethodResponse>> GetUserPaymentMethods();
    Task<AddUserPaymentMethodResp> AddUserPaymentMethod(AddUserPaymentMethodReq addUserPaymentMethodReq);
    Task<UpdateUserPaymentMethodResp> UpdateUserPaymentMethod(Guid userPaymentMethodId, UpdateUserPaymentMethodReq updateUserPaymentMethodReq);
    Task<bool> RemoveUserPaymentMethod(Guid userPaymentMethodId);

    Task<bool> VerifyTransactionOtpAsync(VerifyTransactionOtpReq req);
    Task<TransactionOtpInfoResp> ResentTransactionOtpAsync(Guid transactionId, TransactionOtpType otpType);
    Task<TransactionOtpInfoResp> DonateAsync(DonateReq req);
    Task<TransactionOtpInfoResp> TransferAsync(TransferReq req);
    Task<WithDrawPrepareResp> WithdrawPrepareAsync();
    Task<TransactionOtpInfoResp> WithdrawAsync(WithdrawReq req);
    Task<DepositPrepareResp> DepositPrepareAsync();
    Task<DepositResp> DepositAsync(DepositReq req);
    Task<bool> DepositCancelAsync(DepositCancelReq req);
    Task<CallBackZaloPayResponse> CallBackZaloPayAsync(ZaloPayCallBackReq req);
    Dictionary<string, object> CallBackZaloPay(dynamic cbdata);
}
