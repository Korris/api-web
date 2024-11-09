namespace Mcsg.Wallet.Api.Interfaces;

using Api.Models._3rdClass.ZaloPay.Response;
using Common.Core.Requests;
using Common.Models;
using Domain.Enums;
using Models;
using Requests;

public interface IUserWalletService
{
    Task<IEnumerable<UserWalletResp>> GetUserWalletAsync(BaseR req);
    Task<UserWalletBasicResp> GetUserWalletByAddressAsync(UserWalletGetInfoByAddressR request);
    Task<PaginatedList<UserWalletTransactionItemResp>> GetUserWalletTransactionsAsync(UserWalletTransactionSearchR request);
    Task<UserWalletTransactionItemResp> GetUserWalletTransactionByRefNumberAsync(BaseR req, string referenceNumber);
    Task<IEnumerable<UserPaymentMethodResponse>> GetUserPaymentMethods(Guid userId);
    Task<AddUserPaymentMethodResp> AddUserPaymentMethod(UserWalletAddPaymentMethodR addUserPaymentMethodReq);
    Task<UpdateUserPaymentMethodResp> UpdateUserPaymentMethod(Guid userPaymentMethodId, UserWalletUpdatePaymentMethodR updateUserPaymentMethodReq);
    Task<bool> RemoveUserPaymentMethod(Guid userId, Guid userPaymentMethodId);
    Task<UserWalletBasicResp> GetUserWalletAddress(UserWalletGetUserWalletAddressByUserIdR request);

    Task<UserWalletTransactionWithDetailsResp> VerifyTransactionOtpAsync(UserWalletVerifyTransactionOtpR req);
    Task<TransactionOtpInfoResp> ResentTransactionOtpAsync(Guid transactionId, TransactionOtpType otpType);
    Task<TransactionOtpInfoResp> DonateAsync(UserWalletDonateR req);
    Task<TransactionOtpInfoResp> TransferAsync(UserWalletTransferR req);
    Task<WithDrawPrepareResp> WithdrawPrepareAsync(Guid userId);
    Task<TransactionOtpInfoResp> WithdrawAsync(UserWalletWithdrawR req);
    Task<DepositPrepareResp> DepositPrepareAsync(Guid userId);
    Task<DepositResp> DepositAsync(UserWalletDepositR req);
    Task<bool> DepositCancelAsync(UserWalletDepositCancelR req);
    Task<CallBackZaloPayResponse> CallBackZaloPayAsync(UserWalletZaloPayCallBackR req);
    Dictionary<string, object> CallBackZaloPay(dynamic cbdata);
}
