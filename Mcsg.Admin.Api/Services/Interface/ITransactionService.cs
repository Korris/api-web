namespace Mcsg.Admin.Api.Services.Interface
{
    using Dtos;
    using Lib.Common.Models;
    using Requests;

    public interface ITransactionService
    {
        Task<PaginatedList<UserWalletTransactionItemResp>> GetListAsync(UserWalletTransactionReq request);
        Task<bool> ApproveTransaction(ApproveReq req);
        Task<bool> RejectTransaction(RejectReq req);
        Task<bool> ApproveRefTransaction(ApproveRefReq req);
        Task<bool> RejectRefTransaction(RejectRefReq req);
    }
}
