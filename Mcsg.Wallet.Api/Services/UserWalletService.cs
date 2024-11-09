using Grpc.Net.Client;
using HD.ZaloPay.Helper.Crypto;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace Mcsg.Wallet.Api.Services;

using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Core.Requests;
using Common.Helpers;
using Common.Models;
using Common.SeedWork;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Identity.Api.Protos;
using Interfaces;
using Models;
using Models._3rdClass.ZaloPay.Response;
using Requests;
using Validators;
using static Common.Core.Constants.Setting;
using static Common.SeedWork.Constants.Message;

public class UserWalletService : BaseRedisS, IUserWalletService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="setting"></param>
    /// <param name="rs"></param>
    /// <param name="otpService"></param>
    /// <param name="bankService"></param>
    /// <param name="systemService"></param>
    /// <param name="zaloPayService"></param>
    /// <param name="logger"></param>
    /// <param name="aes"></param>
    /// <param name="notificationService"></param>
    public UserWalletService(IWalletContext context, ISetting setting, IRedisStore rs, IOtpService otpService, IBankService bankService, ISystemService systemService, IZaloPayService zaloPayService, ILogger<UserWalletService> logger, ISecurityAes aes, INotificationService notificationService) : base(context, setting, rs)
    {
        _otpService = otpService;
        _bankService = bankService;
        _systemService = systemService;
        _zaloPayService = zaloPayService;
        _logger = logger;
        _aes = aes;
        _notificationService = notificationService;
    }

    #region User info
    public async Task<IEnumerable<UserWalletResp>> GetUserWalletAsync(BaseR req)
    {
        var userId = req.UserId;
        if (userId == null)
        {
            return [];
        }

        var data = await _context.UserWallets.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);
        if (data == null)
        {
            return [];
        }

        var user = new UserWalletResp
        {
            WalletAddress = data.Address,
            Point = data.Point,
            RewardPoint = data.RewardPoint,
            TotalPoint = data.Point + data.RewardPoint,
            Owner = req.ProfileName
        };

        // Select premium
        var lastPackage = await _context.UserPremiumPackages.Where(x => x.UserWalletId == data.Id).OrderByDescending(x => x.EndDate).FirstOrDefaultAsync();
        if (lastPackage != null)
        {
            user.PremiumDate = DateOnly.FromDateTime(lastPackage.EndDate);
        }

        return [user];
    }

    public async Task<UserWalletBasicResp> GetUserWalletByAddressAsync(UserWalletGetInfoByAddressR request)
    {
        var data = await _context.UserWallets
            .Where(x => x.Address == request.Address)
            .AsNoTracking()
            .Select(x => new UserWalletBasicResp
            {
                WalletAddress = x.Address + "",
                ProfileName = x.ProfileName + "",
                UserId = x.UserId
            }).FirstOrDefaultAsync();

        if (data == null)
        {
            throw new BadRequestException(ApiErrorCodes.WALLET_ADDRESS_NOT_FOUND, ApiErrorMessage.WALLET_ADDRESS_NOT_FOUND);
        }

        var userInfoList = await GetUserFromProto(new List<Guid?> { data.UserId, request.UserId });

        var walletUserInfo = userInfoList.GetValueOrDefault(data.UserId?.ToString());
        var myUserInfo = userInfoList.GetValueOrDefault(request.UserId.ToString());

        if (walletUserInfo != null)
        {
            data.Email = _aes.DecryptText(myUserInfo?.Email) + "";
            data.Avatar = walletUserInfo?.UserAvatar;
            data.ProfileName = walletUserInfo?.ProfileName + "";
        }
        return data;
    }

    public async Task<PaginatedList<UserWalletTransactionItemResp>> GetUserWalletTransactionsAsync(UserWalletTransactionSearchR request)
    {
        var vr = new UserWalletTransactionSearchV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(M109);
        }

        var userId = request.UserId;
        var tz = request.TimezoneOffset;
        var query = _context.WalletTransactions
            .Include(x => x.SourceUserWallet)
            .Include(x => x.DestinationUserWallet)
            .Where(x =>
                (x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == userId && x.Status != TransactionStatus.Pending && x.Status != TransactionStatus.Failed)
                || (x.SourceUserWallet != null && x.SourceUserWallet.UserId == userId)
            );

        if (!string.IsNullOrEmpty(request.ReferenceNumber))
        {
            query = query.Where(x => x.ReferenceNumber != null && x.ReferenceNumber.Contains(request.ReferenceNumber));
        }

        if (request.FromDate.HasValue)
        {
            var startOfDay = request.FromDate.Value.Date.StartOfDay();
            var startOfDayUtc = startOfDay.AddMinutes(tz);
            query = query.Where(x => x.CreatedOn >= startOfDayUtc);
        }

        if (request.ToDate.HasValue)
        {
            var endOfDay = request.ToDate.Value.Date.StartOfDay();
            var endOfDayUtc = endOfDay.AddMinutes(tz);
            query = query.Where(x => x.CreatedOn <= endOfDayUtc);
        }

        var listTransferTransactionTypes = new List<TransferTransactionType>();
        if (request.TransactionType != null && request.TransactionType.Count > 0)
        {
            foreach (var type in request.TransactionType)
            {
                var transactionStatus = type.ToEnum(TransferTransactionType.ReceiveBC);
                listTransferTransactionTypes.Add(transactionStatus);
            }
        }

        if (listTransferTransactionTypes.Count > 0)
        {
            query = query.Where(x =>
                (listTransferTransactionTypes.Contains(TransferTransactionType.ReceiveDonateBC) &&
                 x.Type == TransactionType.Donate && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == userId) ||

                (listTransferTransactionTypes.Contains(TransferTransactionType.DonateBC) &&
                 x.Type == TransactionType.Donate && x.SourceUserWallet != null && x.SourceUserWallet.UserId == userId) ||

                (listTransferTransactionTypes.Contains(TransferTransactionType.ReceiveBC) &&
                 x.Type == TransactionType.Transfer && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == userId) ||

                (listTransferTransactionTypes.Contains(TransferTransactionType.SendBC) &&
                 x.Type == TransactionType.Transfer && x.SourceUserWallet != null && x.SourceUserWallet.UserId == userId)
            );
        }

        if (request.TransactionStatus != null && request.TransactionStatus.Count > 0)
        {
            var listStatus = new List<TransactionStatus>();
            foreach (var status in request.TransactionStatus)
            {
                var transactionStatus = status.ToEnum(TransactionStatus.Success);
                listStatus.Add(transactionStatus);
            }
            query = query.Where(x => listStatus.Contains(x.Status));
        }

        var resultQuery = query
            .OrderByDescending(x => x.CreatedOn)
            .Select(x => new UserWalletTransactionItemResp
            {
                Amount = x.Amount,
                AmountSign = (x.Type == TransactionType.Deposit
                            || x.Type == TransactionType.Reward
                            || (x.Type == TransactionType.Donate && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == userId)
                            || (x.Type == TransactionType.Transfer && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == userId))
                            ? "+" : "-",
                Content = x.Content,
                CreatedOn = x.CreatedOn,
                FromAddress = x.IsFromSystem ? ApiMessages.FROM_SYSTEM : x.SourceUserWallet.Address,
                ToAddress = x.DestinationUserWallet != null ? x.DestinationUserWallet.Address : string.Empty,
                FromUser = x.IsFromSystem ? ApiMessages.FROM_SYSTEM : x.SourceUserWallet.ProfileName,
                ReferenceNumber = x.ReferenceNumber,
                ToUser = x.DestinationUserWallet != null ? x.DestinationUserWallet.ProfileName : string.Empty,
                ToUserId = x.DestinationUserWallet != null ? x.DestinationUserWallet.UserId : Guid.Empty,
                FromUserId = x.IsFromSystem ? Guid.Empty : (x.SourceUserWallet != null ? x.SourceUserWallet.UserId : Guid.Empty),
                TransactionStatus = x.Status,
                TransactionType = x.Type,
                Id = x.Id,
                SystemMessage = x.SystemMessage,
                TransactionFee = x.TransactionFee
            }).AsNoTracking();

        var items = await resultQuery.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();

        var userIds = items.SelectMany(p => new List<Guid?> { p.ToUserId, p.FromUserId })
                       .Where(id => id != null && id != Guid.Empty)
                       .Distinct()
                       .ToList();

        var userInfo = await GetUserFromProto(userIds);

        foreach (var item in items)
        {
            var toUserOfProto = item.ToUserId != null ? userInfo!.GetValueOrDefault(item.ToUserId.ToString()) : null;
            var fromUserOfProto = item.FromUserId != null ? userInfo!.GetValueOrDefault(item.FromUserId.ToString()) : null;
            item.ToUser = toUserOfProto?.ProfileName;
            item.FromUser = fromUserOfProto?.ProfileName;
            item.ToUserAvatar = toUserOfProto?.UserAvatar;
            item.ProfileName = toUserOfProto?.ProfileName + "";
            item.FromUserAvatar = fromUserOfProto?.UserAvatar;
        }

        return await PaginatedList<UserWalletTransactionItemResp>.CreateAsync(items, resultQuery, request.PageNumber, request.PageSize);
    }

    public async Task<UserWalletTransactionItemResp> GetUserWalletTransactionByRefNumberAsync(BaseR req, string referenceNumber)
    {
        var userId = req.UserId;
        var data = await _context.WalletTransactions
            .Include(x => x.SourceUserWallet)
            .Include(x => x.DestinationUserWallet)
            .Include(x => x.UserPaymentMethods).ThenInclude(x => x.PaymentMethod)
            .Where(x => x.ReferenceNumber == referenceNumber)
            .AsNoTracking()
            .Select(x => new UserWalletTransactionItemDetailResp
            {
                Amount = x.Amount,
                AmountSign = (x.Type == TransactionType.Deposit
                            || x.Type == TransactionType.Reward
                            || (x.Type == TransactionType.Donate && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == userId)
                            || (x.Type == TransactionType.Transfer && x.DestinationUserWallet != null && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == userId)
                            ) ? "+" : "-",
                Content = x.Content,
                CreatedOn = x.CreatedOn,
                FromAddress = x.IsFromSystem ? ApiMessages.FROM_SYSTEM : x.SourceUserWallet.Address,
                FromUserId = x.IsFromSystem ? Guid.Empty : x.SourceUserWallet.UserId,
                ToAddress = x.DestinationUserWallet != null ? x.DestinationUserWallet.Address : string.Empty,
                FromUser = x.IsFromSystem ? ApiMessages.FROM_SYSTEM : x.SourceUserWallet.ProfileName,
                PaymentMethodId = (x.UserPaymentMethodId != null && x.UserPaymentMethods != null) ? x.UserPaymentMethods.PaymentMethod.Id : null,
                PaymentMethod = (
                    x.UserPaymentMethods != null) ?
                    new PaymentMethodResp { Logo = x.UserPaymentMethods.PaymentMethod.Logo, Name = x.UserPaymentMethods.PaymentMethod.Name } :
                    (x.Type == TransactionType.Deposit && x.SystemMethod != null) ? new PaymentMethodResp
                    {
                        Name = x.SystemMethod == SystemPaymentMethod.Bank ? "Direct Banking" : x.SystemMethod == SystemPaymentMethod.ZaloPay ? "Zalo Pay" : ""
                    } : null,
                ReferenceNumber = x.ReferenceNumber,
                ToUser = x.DestinationUserWallet != null ? x.DestinationUserWallet.ProfileName : string.Empty,
                ToUserId = x.DestinationUserWallet != null ? x.DestinationUserWallet.UserId : ((x.Type == TransactionType.Deposit || x.Type == TransactionType.BuyPremium) ? Guid.Empty : null),
                TransactionStatus = x.Status,
                TransactionType = x.Type,
                Id = x.Id,
                SystemMessage = x.SystemMessage,
                TransactionFee = x.TransactionFee
            }).FirstOrDefaultAsync();

        if (data.ToUserId == null || data.ToUserId == Guid.Empty)
        {
            data.ToUserId = Guid.Empty;
            data.ToUser = ApiMessages.TO_SYSTEM;
        }

        if (data.FromUserId == null || data.FromUserId == Guid.Empty)
        {
            data.FromUserId = Guid.Empty;
            data.FromUser = ApiMessages.FROM_SYSTEM;
        }

        if (data == null)
        {
            throw new BadRequestException(ApiErrorCodes.TRANSACTION_NOT_FOUND, ApiErrorMessage.TRANSACTION_NOT_FOUND);
        }

        var userIds = new List<Guid?> { data.ToUserId, data.FromUserId }
               .Where(id => id != null && id != Guid.Empty)
               .Distinct()
               .ToList();

        var userInfo = await GetUserFromProto(userIds);

        if (userInfo != null)
        {
            var toUserOfProto = data.ToUserId != null ? userInfo.GetValueOrDefault(data.ToUserId.ToString()) : null;
            var fromUserOfProto = data.FromUserId != null ? userInfo.GetValueOrDefault(data.FromUserId.ToString()) : null;
            data.ToUser = toUserOfProto?.ProfileName;
            data.FromUser = fromUserOfProto?.ProfileName;
            data.ToUserAvatar = toUserOfProto?.UserAvatar;
            data.FromUserAvatar = fromUserOfProto?.UserAvatar;
        }

        return data;
    }
    private async Task<UserWalletTransactionWithDetailsResp> UpdateWalletInfor(WalletTransaction transaction)
    {
        CheckBalance(transaction.SourceUserWallet.Point, transaction.SourceUserWallet.RewardPoint, transaction.Amount);

        transaction.Status = TransactionStatus.Success;
        transaction.IsConfirmed = true;

        switch (transaction.Type)
        {
            case TransactionType.Deposit:
                {
                    transaction.DestinationUserWallet.Point += transaction.Amount;
                    break;
                }
            case TransactionType.Donate:
            case TransactionType.Transfer:
                {
                    //Source
                    if (transaction.SourceUserWallet.RewardPoint >= (transaction.Amount + (float)(_setting.TransactionChargeFee * transaction.Amount)))
                    {
                        transaction.SourceUserWallet.RewardPoint -= (transaction.Amount + (float)(_setting.TransactionChargeFee * transaction.Amount));
                    }
                    else
                    {
                        var remainingAmount = (transaction.Amount + (float)(_setting.TransactionChargeFee * transaction.Amount)) - transaction.SourceUserWallet.RewardPoint;
                        transaction.SourceUserWallet.RewardPoint = 0;
                        transaction.SourceUserWallet.Point -= remainingAmount;
                    }
                    transaction.TransactionFee = (float)(_setting.TransactionChargeFee * transaction.Amount);
                    transaction.DestinationUserWallet.Point += transaction.Amount;
                    break;
                }
        }
        _context.WalletTransactions.Update(transaction);
        await _context.SaveChangesAsync(default);
        await _notificationService.AddTransactionNotificationAsync(new Requests.Notification.TransactionNotificationReq
        {
            Amount = transaction.Amount,
            TransactionType = transaction.Type,
            Id = transaction.Id,
            AuthorId = transaction.SourceUserWallet.UserId,
            ReceiverId = transaction.DestinationUserWallet.UserId,
            ReferenceNumber = transaction.ReferenceNumber
        });

        var item = new UserWalletTransactionWithDetailsResp
        {
            Amount = transaction.Amount,
            AmountSign = (transaction.Type == TransactionType.Deposit
                        || transaction.Type == TransactionType.Reward
                        || (transaction.Type == TransactionType.Donate && transaction.DestinationUserWallet != null && transaction.DestinationUserWallet.UserId == transaction.SourceUserWallet.UserId)
                        || (transaction.Type == TransactionType.Transfer && transaction.DestinationUserWallet != null && transaction.DestinationUserWallet.UserId == transaction.SourceUserWallet.UserId)
                        ) ? "+" : "-",
            Content = transaction.Content + "",
            CreatedOn = transaction.CreatedOn,
            FromAddress = transaction.IsFromSystem ? ApiMessages.FROM_SYSTEM : transaction.SourceUserWallet.Address,
            ToAddress = transaction.DestinationUserWallet != null ? transaction.DestinationUserWallet.Address : string.Empty,
            FromUser = transaction.IsFromSystem ? ApiMessages.FROM_SYSTEM : transaction.SourceUserWallet.ProfileName,
            ReferenceNumber = transaction.ReferenceNumber + "",
            ToUser = transaction.DestinationUserWallet != null ? transaction.DestinationUserWallet.ProfileName : string.Empty,
            ToUserId = transaction.DestinationUserWallet != null ? transaction.DestinationUserWallet.UserId : Guid.Empty,
            FromUserId = transaction.IsFromSystem ? Guid.Empty : transaction.SourceUserWallet.UserId,
            TransactionStatus = transaction.Status,
            TransactionType = transaction.Type,
            Id = transaction.Id,
            SystemMessage = transaction.SystemMessage + "",
            TransactionFee = transaction.TransactionFee
        };

        var userIds = new List<Guid?> { item.ToUserId, item.FromUserId }
                  .Where(id => id != null && id != Guid.Empty)
                  .Distinct()
                  .ToList();

        var userInfo = await GetUserFromProto(userIds);

        if (userInfo != null)
        {
            var toUserOfProto = item.ToUserId != null ? userInfo.GetValueOrDefault(item.ToUserId.ToString()) : null;
            var fromUserOfProto = item.FromUserId != null ? userInfo.GetValueOrDefault(item.FromUserId.ToString()) : null;
            item.ToUser = toUserOfProto?.ProfileName;
            item.FromUser = fromUserOfProto?.ProfileName;
            item.ToUserAvatar = toUserOfProto?.UserAvatar;
            item.FromUserAvatar = fromUserOfProto?.UserAvatar;
        }
        item.IsValid = true;
        return item;

    }

    public async Task<UserWalletBasicResp> GetUserWalletAddress(UserWalletGetUserWalletAddressByUserIdR request)
    {
        var result = new UserWalletBasicResp();

        var userWallets = await _context.UserWallets.AsNoTracking()
            .Where(p => p.UserId == request.WalletOwnerId)
            .Select(p => new
            {
                p.Address,
                p.ProfileName,
                p.UserId
            }).FirstOrDefaultAsync();

        if (userWallets == null)
        {
            throw new BadRequestException(ApiErrorCodes.WALLET_ADDRESS_NOT_FOUND, ApiErrorMessage.WALLET_ADDRESS_NOT_FOUND);
        }

        var userInfoList = await GetUserFromProto(new List<Guid?> { userWallets.UserId, request.UserId });

        var walletUserInfo = userInfoList.GetValueOrDefault(userWallets.UserId.ToString());
        var myUserInfo = userInfoList.GetValueOrDefault(request.UserId.ToString());

        return result = new UserWalletBasicResp
        {
            Email = _aes.DecryptText(myUserInfo?.Email) + "",
            WalletAddress = userWallets?.Address + "",
            ProfileName = walletUserInfo?.ProfileName + "",
            Avatar = walletUserInfo?.UserAvatar,
            UserId = userWallets?.UserId
        };
    }

    #endregion

    #region User payment method
    public async Task<bool> RemoveUserPaymentMethod(Guid userId, Guid userPaymentMethodId)
    {
        var paymentMethod = _context.UserPaymentMethods.Where(
             x => x.UserWallet.UserId == userId && x.Id == userPaymentMethodId && x.IsDelete == false
            ).FirstOrDefault();

        if (paymentMethod != null)
        {
            paymentMethod.IsDelete = true;
            _context.UserPaymentMethods.Update(paymentMethod);
            await _context.SaveChangesAsync(default);
        }

        return true;
    }

    public async Task<IEnumerable<UserPaymentMethodResponse>> GetUserPaymentMethods(Guid userId)
    {
        string encryptKey = _setting.EncryptKey;

        var userPaymentMethods = await _context.UserPaymentMethods
            .Include(x => x.PaymentMethod)
            .Where(x => x.UserWallet.UserId == userId && !x.IsDelete)
            .AsNoTracking()
            .Select(x => new UserPaymentMethodResponse
            {
                Id = x.Id,
                UserWalletId = x.UserWalletId,
                PaymentMethodId = x.PaymentMethodId,
                Logo = x.PaymentMethod.Logo,
                BankCode = x.PaymentMethod.Code,
                AccountNumber = x.AccountNumber,
                BankName = x.PaymentMethod.Name,
                AccountName = x.AccountName
            })
            .ToListAsync();

        foreach (var userPaymentMethod in userPaymentMethods)
        {
            userPaymentMethod.AccountName = userPaymentMethod.AccountName != null ? CryptoHelper.Decrypt(userPaymentMethod.AccountName, encryptKey) : null;
            userPaymentMethod.AccountNumber = userPaymentMethod.AccountNumber != null ? CryptoHelper.Decrypt(userPaymentMethod.AccountNumber, encryptKey) : null;
        }


        return userPaymentMethods;
    }

    public async Task<AddUserPaymentMethodResp> AddUserPaymentMethod(UserWalletAddPaymentMethodR addUserPaymentMethodReq)
    {
        var userId = addUserPaymentMethodReq.UserId;
        var paymentMethod = await _context.PaymentMethods
                    .Where(x => x.Id == addUserPaymentMethodReq.PaymentMethodId)
                    .FirstOrDefaultAsync();
        var userWallet = await _context.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();

        if (paymentMethod == null || paymentMethod.AllowWithdrawal == false)
        {
            throw new BadRequestException(ApiErrorCodes.ERROR_PAYMENT_METHOD_TYPE, ApiErrorMessage.ERROR_PAYMENT_METHOD_TYPE);
        }
        string encryptKey = _setting.EncryptKey;
        var entity = new UserPaymentMethod
        {
            UserWalletId = userWallet.Id,
            PaymentMethod = paymentMethod,
            AccountNumber = addUserPaymentMethodReq.AccountNumber != null ? CryptoHelper.Encrypt(addUserPaymentMethodReq.AccountNumber, encryptKey) : null,
            AccountName = addUserPaymentMethodReq.AccountName != null ? CryptoHelper.Encrypt(addUserPaymentMethodReq.AccountName, encryptKey) : null
        };

        _context.UserPaymentMethods.Add(entity);
        await _context.SaveChangesAsync(default);

        return new AddUserPaymentMethodResp
        {
            Id = entity.Id,
            PaymentMethodId = paymentMethod.Id,
            AccountNumber = addUserPaymentMethodReq.AccountNumber,
            AccountName = addUserPaymentMethodReq.AccountName,
        };
    }

    public async Task<UpdateUserPaymentMethodResp> UpdateUserPaymentMethod(Guid userPaymentMethodId, UserWalletUpdatePaymentMethodR updateUserPaymentMethodReq)
    {
        var userId = updateUserPaymentMethodReq.UserId;
        var userPaymentMethod = await _context.UserPaymentMethods
            .Include(x => x.PaymentMethod)
                    .Where(x => x.Id == userPaymentMethodId)
                    .FirstOrDefaultAsync();
        var userWallet = await _context.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();

        if (userPaymentMethod == null)
        {
            throw new BadRequestException("NOT FOUND", ApiErrorMessage.ERROR_PAYMENT_METHOD_TYPE);
        }
        string encryptKey = _setting.EncryptKey;

        userPaymentMethod.AccountNumber = updateUserPaymentMethodReq.AccountNumber != null ? CryptoHelper.Encrypt(updateUserPaymentMethodReq.AccountNumber, encryptKey) : null;
        userPaymentMethod.AccountName = updateUserPaymentMethodReq.AccountName != null ? CryptoHelper.Encrypt(updateUserPaymentMethodReq.AccountName, encryptKey) : null;
        if (updateUserPaymentMethodReq?.PaymentMethodId != null && userPaymentMethod.PaymentMethodId != updateUserPaymentMethodReq?.PaymentMethodId)
        {
            var paymentMethod = await _context.PaymentMethods
                   .Where(x => x.Id == updateUserPaymentMethodReq.PaymentMethodId)
                   .FirstOrDefaultAsync();

            if (paymentMethod == null || paymentMethod.AllowWithdrawal == false)
            {
                throw new BadRequestException(ApiErrorCodes.ERROR_PAYMENT_METHOD_TYPE, ApiErrorMessage.ERROR_PAYMENT_METHOD_TYPE);
            }
            userPaymentMethod.PaymentMethod = paymentMethod;
        }

        _context.UserPaymentMethods.Update(userPaymentMethod);
        await _context.SaveChangesAsync(default);

        return new UpdateUserPaymentMethodResp
        {
            Id = userPaymentMethod.Id,
            PaymentMethodId = userPaymentMethod.PaymentMethodId,
            AccountNumber = updateUserPaymentMethodReq.AccountNumber,
            AccountName = updateUserPaymentMethodReq.AccountName,
        };
    }

    #endregion

    #region User OTP/transaction
    public async Task<UserWalletTransactionWithDetailsResp> VerifyTransactionOtpAsync(UserWalletVerifyTransactionOtpR req)
    {
        var userWalletResponse = new UserWalletTransactionWithDetailsResp();

        var transaction = await _context.WalletTransactions
             .Include(x => x.DestinationUserWallet)
             .Include(x => x.SourceUserWallet)
             .FirstOrDefaultAsync(p => p.Id == req.TransactionId);

        if (transaction == null)
        {
            throw new BadRequestException(ApiErrorCodes.TRANSACTION_NOT_FOUND, ApiErrorMessage.TRANSACTION_NOT_FOUND);
        }

        if (transaction.Status != TransactionStatus.Pending)
        {
            throw new BadRequestException(ApiErrorCodes.TRANSACTION_ALREADY_PROCESSED, ApiErrorMessage.TRANSACTION_ALREADY_PROCESSED);
        }


        var otpData = await _context.WalletTransactionOtps.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.TransactionId == req.TransactionId
                         && x.Otp == req.Otp && x.OtpToken == req.OtpToken);
        if (otpData == null)
        {
            var cacheKey = $"{req.TransactionId}";
            var attemptCountRedis = await _rs.RedisCache.StringGetAsync(cacheKey);

            int attemptCount = attemptCountRedis.HasValue ? (int)attemptCountRedis : MaxAttempts;

            if (attemptCount == MaxAttempts)
            {
                await _rs.RedisCache.StringSetAsync(cacheKey, attemptCount, TimeSpan.FromMinutes(60));
            }

            attemptCount--;
            var ttl = await _rs.RedisCache.KeyTimeToLiveAsync(cacheKey);
            await _rs.RedisCache.StringSetAsync(cacheKey, attemptCount, ttl.Value, when: StackExchange.Redis.When.Exists);

            if (attemptCount < 1)
            {
                if (transaction != null)
                {
                    transaction.Status = TransactionStatus.Failed;
                    transaction.IsConfirmed = true;
                    _context.WalletTransactions.Update(transaction);
                    await _context.SaveChangesAsync(default);
                    await _otpService.ClearAllTransactionOtpOtpAsync(req.TransactionId);
                }
                await _rs.RedisCache.KeyDeleteAsync(cacheKey);

                throw new BadRequestException(ApiErrorCodes.TRANSACTION_ALREADY_PROCESSED, ApiErrorMessage.TRANSACTION_ALREADY_PROCESSED);
            }
            userWalletResponse.RemainingAttempts = attemptCount;
            return userWalletResponse;
        }

        if (DateTime.UtcNow.Subtract(otpData.CreatedOn).TotalMinutes > _setting.Otp.OtpExpired)
            throw new BadRequestException(ApiErrorCodes.OTP_EXPIRED, ApiErrorMessage.OTP_EXPIRED);

        userWalletResponse = await UpdateWalletInfor(transaction);

        await _context.SaveChangesAsync(default);
        await _otpService.ClearAllTransactionOtpOtpAsync(req.TransactionId);

        return userWalletResponse;
    }
    public async Task<TransactionOtpInfoResp> ResentTransactionOtpAsync(Guid transactionId, TransactionOtpType otpType)
    {
        var transaction = await _context.WalletTransactions.AsNoTracking()
            .Include(x => x.DestinationUserWallet)
            .Include(x => x.SourceUserWallet)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == transactionId);
        if (transaction == null)
        {
            throw new BadRequestException(ApiErrorCodes.TRANSACTION_NOT_FOUND, ApiErrorMessage.TRANSACTION_NOT_FOUND);
        }
        return await _otpService.CreateAsync(transaction, otpType);
    }

    #endregion

    #region Donate
    public async Task<TransactionOtpInfoResp> DonateAsync(UserWalletDonateR req)
    {
        var userId = req.UserId;
        if (userId.Equals(req.ToUserId))
        {
            throw new BadRequestException(ApiErrorCodes.USER_AS_THE_SAME_DONOR, ApiErrorMessage.USER_AS_THE_SAME_DONOR);
        }

        if (req.Amount <= 0 || float.IsNaN(req.Amount))
        {
            throw new BadRequestException(ApiErrorCodes.INVALID_AMOUNT, ApiErrorMessage.INVALID_AMOUNT);
        }

        var userWallet = await _context.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
        var toUserWallet = await _context.UserWallets.Where(x => x.UserId == req.ToUserId).FirstOrDefaultAsync();
        if (toUserWallet != null)
        {
            if (string.IsNullOrEmpty(userWallet.Email)
                && string.IsNullOrEmpty(userWallet.PhoneNumber))
            {
                return null;
            }

            CheckBalance(userWallet.Point, userWallet.RewardPoint, req.Amount);

            var transaction = CreateTransaction(userId, userWallet.Id, toUserWallet.Id, req.Amount, TransactionType.Donate, req.Content, ApiMessages.DONATE_TO_USER);
            transaction.SystemMethod = SystemPaymentMethod.Point;
            await _context.WalletTransactions.AddAsync(transaction);

            var otpType = !string.IsNullOrEmpty(userWallet.Email) ? TransactionOtpType.Email : TransactionOtpType.Phone;

            var otpInfo = await _otpService.CreateAsync(transaction, otpType);
            otpInfo.ToProfileName = toUserWallet.ProfileName;

            await _context.SaveChangesAsync(default);

            return otpInfo;
        }
        else
        {
            throw new BadRequestException(ApiErrorCodes.USER_NOT_FOUND, ApiErrorMessage.USER_NOT_FOUND);
        }
    }
    #endregion

    #region Transfer
    public async Task<TransactionOtpInfoResp> TransferAsync(UserWalletTransferR req)
    {
        if (req.Amount <= 0 || float.IsNaN(req.Amount))
        {
            throw new BadRequestException(ApiErrorCodes.INVALID_AMOUNT, ApiErrorMessage.INVALID_AMOUNT);
        }

        var userId = req.UserId;
        var userWallet = await _context.UserWallets.Where(x => x.UserId == req.UserId)
                                                   .FirstOrDefaultAsync();

        var toUserWallet = await _context.UserWallets.Where(x => x.Address == req.ToAddress)
                                                     .FirstOrDefaultAsync();
        //Check permission

        if (userWallet == null || userWallet == null)
        {
            throw new ForbiddenAccessException(ApiErrorCodes.WALLET_ADDRESS_NOT_FOUND, ApiErrorMessage.WALLET_ADDRESS_NOT_FOUND);

        }

        if (userWallet.Address == toUserWallet.Address)
        {
            throw new ForbiddenAccessException(ApiErrorCodes.CAN_NOT_TRANSFER_THEMSELEVE, ApiErrorMessage.CAN_NOT_TRANSFER_THEMSELEVE);
        }

        if (userWallet?.UserId != userId || userId == null)
        {
            throw new ForbiddenAccessException(ApiErrorCodes.USER_NOT_PERMISSION, ApiErrorMessage.USER_NOT_PERMISSION);
        }

        if (userWallet != null && toUserWallet != null)
        {
            CheckBalance(userWallet.Point, userWallet.RewardPoint, req.Amount);

            var transaction = CreateTransaction(userId, userWallet.Id, toUserWallet.Id, req.Amount, TransactionType.Transfer, req.Content, ApiMessages.TRANSFER_TO_USER);
            transaction.SystemMethod = SystemPaymentMethod.Point;
            await _context.WalletTransactions.AddAsync(transaction);

            transaction.SourceUserWallet = userWallet;

            var otpType = !string.IsNullOrEmpty(userWallet.Email) ? TransactionOtpType.Email : TransactionOtpType.Phone;
            var otpInfo = await _otpService.CreateAsync(transaction, otpType);
            otpInfo.ToProfileName = toUserWallet.ProfileName;

            await _context.SaveChangesAsync(default);

            return otpInfo;
        }
        else
        {
            throw new BadRequestException(ApiErrorCodes.USER_NOT_FOUND, ApiErrorMessage.USER_NOT_FOUND);
        }
    }

    #endregion

    #region Withdraw
    public async Task<WithDrawPrepareResp> WithdrawPrepareAsync(Guid userId)
    {
        var result = new WithDrawPrepareResp();
        var userWallet = await _context.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
        result.UserPaymentMethods = _context.UserPaymentMethods.Where(x => x.UserWalletId == userWallet.Id && !x.IsDelete)
            .Include(x => x.PaymentMethod)
            .Select(y => new UserPaymentMethodResp
            {
                Id = y.Id,
                PaymentMethodId = y.PaymentMethodId,
                PaymentMethodName = y.PaymentMethod.Name,
                PaymentMethodType = y.PaymentMethod.Type
            })
            .ToList();
        result.CurrencyTypes = _bankService.GetCurrencyTypeRatios();
        result.MinPointCanWithDraw = Default.MinimumPointCanWithDraw;
        result.MaxPointCanWithDraw = userWallet.Point;
        return result;
    }

    public async Task<TransactionOtpInfoResp> WithdrawAsync(UserWalletWithdrawR req)
    {
        var userId = req.UserId;
        var userWallet = await _context.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
        if (userWallet == null)
        {
            throw new BadRequestException(ApiErrorCodes.USER_NOT_FOUND, ApiErrorMessage.USER_NOT_FOUND);
        }

        if (userWallet.Point < req.Amount)
        {
            throw new BadRequestException("Amount over", "");
        }
        if (req.Amount < Default.MinimumPointCanWithDraw)
        {
            throw new BadRequestException("MinimumPointCanWithDraw", "");
        }

        var transaction = CreateTransaction(userId, userWallet.Id, req.UserPaymentMethodId, req.Amount, TransactionType.Withdraw, req.Content, ApiMessages.WITHDRAW_MESSAGE);
        transaction.SystemMethod = SystemPaymentMethod.Bank;

        transaction.Content = string.Format(ApiMessages.WITHDRAW_CONTENT, req.Amount, transaction.ReferenceNumber);
        await _context.WalletTransactions.AddAsync(transaction);

        var otpInfo = await _otpService.CreateAsync(transaction, TransactionOtpType.Email);
        otpInfo.ToProfileName = userWallet.ProfileName;

        await _systemService.SendAdminNoti(nameof(TransactionType.Withdraw), req.ProfileId, transaction.Content);
        await _context.SaveChangesAsync(default);

        return otpInfo;
    }
    #endregion

    #region Deposit
    public async Task<DepositPrepareResp> DepositPrepareAsync(Guid userId)
    {
        var result = new DepositPrepareResp();
        var userWallet = await _context.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
        result.DepositMethods = _bankService.GetDepositMethods();
        result.CurrencyTypes = _bankService.GetCurrencyTypeRatios();
        result.MinPointCanDeposit = Default.MinimumPointCanDeposit;
        result.MaxPointCanDeposit = Default.MaximumPointCanDeposit;
        return result;
    }
    public async Task<DepositResp> DepositAsync(UserWalletDepositR req)
    {
        var userId = req.UserId;
        var userWallet = await _context.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
        if (userWallet == null)
        {
            throw new BadRequestException(ApiErrorCodes.USER_NOT_FOUND, ApiErrorMessage.USER_NOT_FOUND);
        }

        if (req.PointAmount < Default.MinimumPointCanDeposit)
        {
            throw new BadRequestException(ApiErrorCodes.MINIMUM_CAN_DEPOSIT, ApiErrorMessage.MINIMUM_CAN_DEPOSIT + Default.MinimumPointCanDeposit.ToString());
        }

        var transaction = CreateTransaction(userId, userWallet.Id, null, req.PointAmount, TransactionType.Deposit, "", ApiMessages.DEPOSIT_MESSAGE);
        var currencyRatio = _bankService.GetCurrencyTypeRatios().FirstOrDefault(x => x.Type == req.CurrencyType)?.Ratio ?? 0;
        var amountToDeposit = currencyRatio * req.PointAmount;
        transaction.SystemMessage = $"{transaction.SystemMessage} - RATIO: {currencyRatio} AMOUNT: {amountToDeposit}"; //TODO currencyTYPE
        transaction.Content = string.Format(ApiMessages.DEPOSIT_CONTENT, req.PointAmount, transaction.ReferenceNumber, req.DepositMethodName);
        transaction.SystemMethod = req.DepositMethodName == DepositMethods.BANK ?
            SystemPaymentMethod.Bank : req.DepositMethodName == DepositMethods.ZALO_PAY ?
            SystemPaymentMethod.ZaloPay : null;

        await _context.WalletTransactions.AddAsync(transaction);
        try
        {

            await _context.SaveChangesAsync(default);
        }
        catch (Exception e)
        {
            throw;
        }

        //Banking
        if (req.DepositMethodName == DepositMethods.BANK)
        {
            string bankAccount = "";

            string bankAccountName = "";
            string bankAccountBin = "";
            string bankName = (await _context.PaymentMethods.Where(x => x.Type == PaymentMethodType.Banking && x.Bin == bankAccountBin)
                .FirstOrDefaultAsync())?.Name;

            var depositResp = new DepositResp()
            {
                QRCodeUrl = CreateQRCode(amountToDeposit, transaction.ReferenceNumber),
                TransactionId = transaction.Id,
                Type = TransactionType.Deposit,
                ToBankName = bankName,
                ToAccountName = bankAccountName,
                ToAccountNumber = bankAccount,
            };

            await _systemService.SendAdminNoti(nameof(TransactionType.Deposit), req.ProfileId, transaction.Content);

            return depositResp;
        }
        else if (req.DepositMethodName == DepositMethods.ZALO_PAY)
        {
            var createOrderRes = await _zaloPayService.CreateOrderAsync(transaction.Id, transaction.Content, amountToDeposit, userId, req.RedirectUrl);

            if (createOrderRes != null && createOrderRes.ReturnCode == (int)ZaloPayReturnCode.SUCCESS)
            {
                transaction.ExternalId = createOrderRes.AppTransId;
                await _context.SaveChangesAsync(default);

                // Send queue to check zalo order
                var jobData = new PaymentTransData()
                {
                    Type = PaymentTransType.ZALO_PAY,
                    TransactionId = transaction.Id,
                    UserId = userId.Value,
                    Status = TransactionStatus.Pending
                };

                var msg = new QueueMessageDto(jobData);
                _setting.SendMessageToQueue(_setting.NotificationExchange, _setting.NotificationQueuePayment, msg);

                var depositResp = new DepositResp()
                {
                    QRCodeUrl = "",
                    TransactionId = transaction.Id,
                    Type = TransactionType.Deposit,
                    ToBankName = "",
                    ToAccountName = "",
                    ToAccountNumber = "",
                    GatewayUrl = createOrderRes.OrderUrl
                };
                return depositResp;
            }
            else
            {
                throw new BadRequestException(ApiErrorCodes.ZALO_PAY_DEPOSIT_FAIL, string.Format(ApiErrorMessage.ZALO_PAY_DEPOSIT_FAIL, createOrderRes != null ? createOrderRes.ReturnMessage : ""));
            }
        }
        else
        {
            throw new BadRequestException("Not implement", "");
        }

    }
    public async Task<CallBackZaloPayResponse> CallBackZaloPayAsync(UserWalletZaloPayCallBackR req)
    {
        try
        {
            _logger.LogInformation($"WalletService.CallBackZaloPayAsync - request: {JsonConvert.SerializeObject(req)}");
            if (req == null || req.Data == null)
            {
                return new CallBackZaloPayResponse()
                {
                    ReturnCode = (int)ZaloPayReturnCode.FAIL,
                    ReturnMessage = ApiErrorMessage.ZALO_PAY_CALLBACK_FAIL
                };
            }
            var dataStr = JsonConvert.SerializeObject(req.Data);
            var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, _setting.ZaloPay.Key2, dataStr);
            _logger.LogInformation($"WalletService.CallBackZaloPayAsync - dataStr: {dataStr} - mac: {mac}");
            if (!req.Mac.Equals(mac))
            {
                return new CallBackZaloPayResponse()
                {
                    ReturnCode = (int)ZaloPayReturnCode.FAIL,
                    ReturnMessage = ApiErrorMessage.ZALO_PAY_MAC_NOT_EQUAL
                };
            }
            else
            {
                // Order successful                    
                var embedData = req.Data.EmbedData;
                if (embedData != null)
                {
                    var transId = Guid.Parse(embedData.TransactionId);
                    var userId = Guid.Parse(embedData.UserId);
                    await _zaloPayService.CompleteTransactionAsync(transId, userId, TransactionStatus.Success);
                    _logger.LogInformation($"WalletService.CallBackZaloPayAsync - CompleteTransaction: {transId} - userId: {userId}");
                    return new CallBackZaloPayResponse()
                    {
                        ReturnCode = (int)ZaloPayReturnCode.SUCCESS,
                        ReturnMessage = "success"
                    };
                }
                else
                {
                    _logger.LogInformation($"WalletService.CallBackZaloPayAsync - TransactionFail embedData null");
                    return new CallBackZaloPayResponse()
                    {
                        ReturnCode = (int)ZaloPayReturnCode.FAIL,
                        ReturnMessage = ApiErrorMessage.ZALO_PAY_CALLBACK_FAIL
                    };
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"WalletService.CallBackZaloPayAsync - TransactionFail Error:{ex.Message}");
            return new CallBackZaloPayResponse()
            {
                ReturnCode = (int)ZaloPayReturnCode.FAIL,
                ReturnMessage = ex.Message
            };
        }
    }

    public async Task<bool> DepositCancelAsync(UserWalletDepositCancelR req)
    {
        var userId = req.UserId;
        var userWallet = await _context.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
        if (userWallet == null)
        {
            throw new BadRequestException(ApiErrorCodes.USER_NOT_FOUND, ApiErrorMessage.USER_NOT_FOUND);
        }

        var transaction = await _context.WalletTransactions.Where(x => x.Id == req.TransactionId
        && x.SourceUserWalletId == userWallet.Id && x.Type == TransactionType.Deposit).FirstOrDefaultAsync();
        if (transaction == null)
        {
            throw new BadRequestException(ApiErrorCodes.TRANSACTION_NOT_FOUND, ApiErrorMessage.TRANSACTION_NOT_FOUND);
        }
        transaction.Status = TransactionStatus.Canceled;

        _context.WalletTransactions.Update(transaction);
        await _context.SaveChangesAsync(default);

        return true;
    }

    public Dictionary<string, object> CallBackZaloPay(dynamic cbdata)
    {
        _logger.LogInformation($"WalletService.CallBackZaloPay - cbData: {JsonConvert.SerializeObject(cbdata)}");
        var result = new Dictionary<string, object>();
        try
        {
            if (cbdata == null)
            {
                return result;
            }
            var dataStr = Convert.ToString(cbdata["data"]);
            var reqMac = Convert.ToString(cbdata["mac"]);
            var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, _setting.ZaloPay.Key2, dataStr);

            _logger.LogInformation($"WalletService.CallBackZaloPay - data: {JsonConvert.SerializeObject(dataStr)} - reqMac: {JsonConvert.SerializeObject(reqMac)} - Mac: {JsonConvert.SerializeObject(mac)}");

            // kiểm tra callback hợp lệ (đến từ ZaloPay server)
            if (!reqMac.Equals(mac))
            {
                // callback không hợp lệ
                result["return_code"] = -1;
                result["return_message"] = "mac not equal";
            }
            else
            {
                // thanh toán thành công
                // merchant cập nhật trạng thái cho đơn hàng
                var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStr);
                _logger.LogInformation($"WalletService.CallBackZaloPay - update order's status = success where app_trans_id = {JsonConvert.SerializeObject(dataJson["app_trans_id"])}");

                result["return_code"] = 1;
                result["return_message"] = "success";
            }

            _logger.LogInformation($"WalletService.CallBackZaloPay - Result: {JsonConvert.SerializeObject(result)}");
            return result;
        }
        catch (Exception ex)
        {
            result["return_code"] = 0; // ZaloPay server sẽ callback lại (tối đa 3 lần)
            result["return_message"] = ex.Message;
            _logger.LogError($"WalletService.CallBackZaloPay - Error.Result: {JsonConvert.SerializeObject(result)}");
            return result;
        }
    }
    #endregion

    private WalletTransaction CreateTransaction(Guid? userId, Guid sourceId, Guid? destinationId, float amount, TransactionType type, string content, string systemMessage)
    {
        var transaction = new WalletTransaction
        {
            CreatedOn = DateTime.UtcNow,
            CreatedBy = userId,
            Id = Guid.NewGuid(),
            Amount = amount,
            IsFromSystem = false,
            Content = content,
            SystemMessage = systemMessage,
            ReferenceNumber = Default.ReferenceNumberLength.GetRandomString().ToLower(),
            ModifiedOn = DateTime.UtcNow,
            SourceUserWalletId = sourceId,
            Status = TransactionStatus.Pending,
            Type = type
        };
        if (type == TransactionType.Withdraw || type == TransactionType.Deposit)
        {
            transaction.UserPaymentMethodId = destinationId;
        }
        else
        {
            transaction.DestinationUserWalletId = destinationId;
        }
        return transaction;
    }

    private void CheckBalance(float inputPoint, float inputReward, float amount)
    {
        //Verify amount
        if ((inputPoint + inputReward) < amount)
        {
            throw new BadRequestException(ApiErrorCodes.BALANCE_NOT_ENOUGH, ApiErrorMessage.BALANCE_NOT_ENOUGH);
        }
    }

    //TODO
    private string CreateQRCode(float amount, string refCode)
    {
        string bankAccount = "";
        string bankAccountName = "";
        string bankAccountBin = "";

        return $"https://api.vietqr.io/image/{bankAccountBin}-{bankAccount}-PSYZ8LO.jpg?accountName={bankAccountName}&amount={amount}&addInfo={refCode}";
    }

    private async Task<Dictionary<string, UserProtoDto>> GetUserFromProto(List<Guid?> userIds)
    {
        var res = new Dictionary<string, UserProtoDto>();

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Web.Identity!);

            var client = new UserProto.UserProtoClient(channel);
            var request = new UserSearchReq
            {
                UserUid = string.Join(';', userIds.Where(id => id != null).Select(id => id.ToString()))
            };
            var rsp = await client.SearchAsync(request);
            return rsp.Items.ToDictionary(p => p.UserId, p => p);
        }
        catch (Exception ex)
        {
            ex.Message.LogError();
        }

        return res;
    }

    #endregion

    #region -- Fields --

    private readonly IBankService _bankService;
    private readonly IZaloPayService _zaloPayService;
    private readonly IOtpService _otpService;
    private readonly ISystemService _systemService;
    private readonly ILogger<UserWalletService> _logger;
    private readonly ISecurityAes _aes;
    private readonly INotificationService _notificationService;

    #endregion

    #region -- Constants --

    private const int MaxAttempts = 3;

    #endregion
}
