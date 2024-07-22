using Microsoft.EntityFrameworkCore;

namespace Mcsg.Admin.Api.Services
{
    using Common.Domain.Entities;
    using Common.SeedWork.Exceptions;
    using Constants;
    using Dtos;
    using Lib.Common.Constants;
    using Lib.Common.Models;
    using Lib.Common.Web.RealTime.Services;
    using Lib.Common.Web.Security;
    using Lib.Data.Repositories;
    using Lib.Data.Wallet;
    using Lib.Data.Wallet.Entities;
    using Lib.Data.Wallet.Enums;
    using Requests;
    using Services.Interface;

    public partial class TransactionService : ITransactionService
    {
        private readonly IRepository<User> _userRepo;
        private readonly ILogger<UserService> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly WalletDbContext _dbContext;
        private readonly ISignalRService _signalRService;

        public TransactionService(IRepository<User> userRepo
            , ILogger<UserService> logger
            , WalletDbContext walletDbContext
            , ISignalRService signalRService
            , ICurrentUserService currentUserService)
        {
            _userRepo = userRepo;
            _logger = logger;
            _currentUserService = currentUserService;
            _signalRService = signalRService;
            _dbContext = walletDbContext;
        }

        public async Task<PaginatedList<UserWalletTransactionItemResp>> GetListAsync(UserWalletTransactionReq request)
        {
            var query = _dbContext.WalletTransactions
                .Include(x => x.SourceUserWallet)
                .Include(x => x.DestinationUserWallet)
                .Where(x =>
                    (
                        (request.FromDate == null || x.CreatedDate >= request.FromDate) &&
                        (request.ToDate == null || x.CreatedDate <= request.ToDate) &&
                        (request.Types == null || request.Types.Contains(x.Type)) &&
                        (request.Status == null || x.Status == request.Status) &&
                        (request.TransactionId == null || x.Id == request.TransactionId) &&
                        (string.IsNullOrEmpty(request.ReferenceNumber) || x.ReferenceNumber == request.ReferenceNumber) &&
                        (request.UserId == null ||
                            (x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == request.UserId)
                            || (x.SourceUserWallet != null && x.SourceUserWallet.UserId == request.UserId)))

                    )
                .Select(x => new UserWalletTransactionItemResp
                {
                    Amount = x.Amount,
                    AmountSign = (x.Type == TransactionType.WITHDRAW
                                || x.Type == TransactionType.REWARD
                                ) ? "-" :
                                    (x.Type == TransactionType.DEPOSIT
                                    ) ? "+" : " ",
                    Content = x.Content,
                    CreatedDate = x.CreatedDate,
                    FromAddress = x.IsFromSystem ? WalletConstants.FROM_SYSTEM : x.SourceUserWallet.Address,
                    ToAddress = x.DestinationUserWallet != null ? x.DestinationUserWallet.Address : string.Empty,
                    FromUser = x.IsFromSystem ? WalletConstants.FROM_SYSTEM : x.SourceUserWallet.ProfileName,
                    ReferenceNumber = x.ReferenceNumber,
                    ToUser = x.DestinationUserWallet != null ? x.DestinationUserWallet.ProfileName : string.Empty,
                    TransactionStatus = x.Status,
                    TransactionType = x.Type,
                    Id = x.Id,
                    SystemMessage = x.SystemMessage
                }).AsNoTracking();

            if (request.OrderByAsc)
            {
                query = query.OrderBy(p => EF.Property<object>(p, request.OrderBy));
            }
            else
            {
                query = query.OrderByDescending(p => EF.Property<object>(p, request.OrderBy));
            }

            var countQuery = _dbContext.WalletTransactions
                .Where(x =>
                    (
                        (request.FromDate == null || x.CreatedDate >= request.FromDate) &&
                        (request.ToDate == null || x.CreatedDate <= request.ToDate) &&
                        (request.Types == null || request.Types.Contains(x.Type)) &&
                        (request.Status == null || x.Status == request.Status) &&
                        (request.TransactionId == null || x.Id == request.TransactionId) &&
                        (string.IsNullOrEmpty(request.ReferenceNumber) || x.ReferenceNumber == request.ReferenceNumber) &&
                        (request.UserId == null ||
                            (x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == request.UserId)
                            || (x.SourceUserWallet != null && x.SourceUserWallet.UserId == request.UserId)))

                    )
                .AsNoTracking().Select(x => new UserWalletTransactionItemResp { Id = x.Id });

            return await PaginatedList<UserWalletTransactionItemResp>.CreateAsync(query, countQuery, request.PageNumber, request.PageSize);
        }

        public async Task<bool> ApproveTransaction(ApproveReq req)
        {
            var transaction = await _dbContext.WalletTransactions
                .Include(x => x.DestinationUserWallet)
                .Include(x => x.SourceUserWallet)
                .Where(x => x.Id == req.TransactionId).FirstOrDefaultAsync();

            var action = await ActionApproveTransaction(transaction);

            var transactionTypeStr = transaction.Type == TransactionType.DEPOSIT ? "deposit" : "withdraw";
            await _signalRService.SendToUser(RealTimeTopic.ReceiveTransactionUpdate, transaction.SourceUserWallet.UserId.ToString(), $"You transaction {transactionTypeStr} {transaction.Amount} point has successfully");

            return action;
        }

        public async Task<bool> RejectTransaction(RejectReq req)
        {
            var transaction = await _dbContext.WalletTransactions
                .Where(x => x.Id == req.TransactionId).FirstOrDefaultAsync();
            return await ActionRejectTransaction(transaction, req.Reason);
        }

        private async Task<bool> ActionApproveTransaction(WalletTransaction transaction)
        {
            if (transaction == null)
            {
                throw new BadRequestException("TRANSACTION_NOT_FOUND", "TRANSACTION_NOT_FOUND");
            }
            switch (transaction.Type)
            {
                case TransactionType.DEPOSIT:
                    {
                        transaction.SourceUserWallet.Point += transaction.Amount;
                        break;
                    }
                case TransactionType.WITHDRAW:
                    {
                        transaction.SourceUserWallet.Point -= transaction.Amount;
                        break;
                    }
                default:
                    throw new BadRequestException("TransactionType NOT SUPPORT", "TransactionType NOT SUPPORT");

            }
            transaction.IsConfirmed = true;
            transaction.ModifiedDate = DateTime.UtcNow;
            transaction.ModifiedBy = _currentUserService?.Session?.UserId;
            transaction.Status = Lib.Data.Wallet.Enums.TransactionStatus.SUCCESS;
            _dbContext.SaveChanges();
            return true;
        }
        private async Task<bool> ActionRejectTransaction(WalletTransaction transaction, string reason)
        {
            if (transaction == null)
            {
                throw new BadRequestException("TRANSACTION_NOT_FOUND", "TRANSACTION_NOT_FOUND");
            }
            switch (transaction.Type)
            {
                case TransactionType.DEPOSIT:
                case TransactionType.WITHDRAW:
                    {
                        break;
                    }
                default:
                    throw new BadRequestException("TransactionType NOT SUPPORT", "TransactionType NOT SUPPORT");

            }
            transaction.IsConfirmed = true;
            transaction.SystemMessage = "REJECT BY ADMIN: Reason:" + reason;
            transaction.Status = TransactionStatus.FAILED;
            transaction.ModifiedDate = DateTime.UtcNow;
            transaction.ModifiedBy = _currentUserService?.Session?.UserId;

            _dbContext.SaveChanges();
            return true;
        }

        public async Task<bool> ApproveRefTransaction(ApproveRefReq req)
        {
            var transaction = await _dbContext.WalletTransactions
                .Include(x => x.DestinationUserWallet)
                .Include(x => x.SourceUserWallet)
                .Where(x => x.ReferenceNumber == req.ReferenceNumber).FirstOrDefaultAsync();

            var action = await ActionApproveTransaction(transaction);
            var transactionTypeStr = transaction.Type == TransactionType.DEPOSIT ? "deposit" : "withdraw";
            await _signalRService.SendToUser(RealTimeTopic.ReceiveTransactionUpdate, transaction.SourceUserWallet.UserId.ToString(), $"You transaction {transactionTypeStr} {transaction.Amount} point has successfully");
            return action;
        }

        public async Task<bool> RejectRefTransaction(RejectRefReq req)
        {
            var transaction = await _dbContext.WalletTransactions
                .Where(x => x.ReferenceNumber == req.ReferenceNumber).FirstOrDefaultAsync();
            return await ActionRejectTransaction(transaction, req.Reason);
        }
    }
}
