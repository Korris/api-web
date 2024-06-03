using Mcsg.Admin.Api.DTOs.Transactions;
using Mcsg.Lib.Common.Models;

namespace Mcsg.Admin.Api.Services.Interface
{
    public interface ITransactionService
    {
        Task<PaginatedList<UserWalletTransactionItemResp>> GetListAsync(UserWalletTransactionReq request);
        Task<bool> ApproveTransaction(ApproveReq req);
        Task<bool> RejectTransaction(RejectReq req);
        Task<bool> ApproveRefTransaction(ApproveRefReq req);
        Task<bool> RejectRefTransaction(RejectRefReq req);
    }
}
