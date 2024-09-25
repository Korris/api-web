using HD.ZaloPay.Helper.Crypto;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace Mcsg.Wallet.Api.Services;

using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Requests;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Interfaces;
using Lib.Common.Enums;
using Lib.Common.Helpers;
using Lib.Common.Models;
using Models;
using Models._3rdClass.ZaloPay.Response;
using Requests;
using static Common.Core.Constants.Setting;

public class UserWalletService : BaseSettingS, IUserWalletService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="setting"></param>
    /// <param name="otpService"></param>
    /// <param name="bankService"></param>
    /// <param name="systemService"></param>
    /// <param name="zaloPayService"></param>
    /// <param name="logger"></param>
    public UserWalletService(IWalletContext context, ISetting setting, IOtpService otpService, IBankService bankService, ISystemService systemService, IZaloPayService zaloPayService, ILogger<UserWalletService> logger) : base(context, setting)
    {
        _otpService = otpService;
        _bankService = bankService;
        _systemService = systemService;
        _zaloPayService = zaloPayService;
        _logger = logger;
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

    public async Task<UserWalletBasicResp> GetUserWalletByAddressAsync(string address)
    {
        var data = await _context.UserWallets
            .Where(x => x.Address == address)
            .AsNoTracking()
            .Select(x => new UserWalletBasicResp
            {
                WalletAddress = x.Address,
                ProfileName = x.ProfileName,
                Email = string.IsNullOrWhiteSpace(x.Email) ? x.ProfileName : x.Email.MaskDigits(3, 3)
            }).FirstOrDefaultAsync();

        if (data == null)
        {
            throw new BadRequestException(ApiErrorCodes.WALLET_ADDRESS_NOT_FOUND, ApiErrorMessage.WALLET_ADDRESS_NOT_FOUND);
        }

        return data;
    }

    public async Task<PaginatedList<UserWalletTransactionItemResp>> GetUserWalletTransactionsAsync(BaseR req, int page = 1, int pageSize = 10)
    {
        var userId = req.UserId;
        var data = new UserWalletTransactionResp();
        var query = _context.WalletTransactions
            .Include(x => x.SourceUserWallet)
            .Include(x => x.DestinationUserWallet)
            .Where(x =>
                (
                    (x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == userId)
                    || (x.SourceUserWallet != null && x.SourceUserWallet.UserId == userId))

                )
            .OrderByDescending(x => x.CreatedOn)
            .Select(x => new UserWalletTransactionItemResp
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
                ToAddress = x.DestinationUserWallet != null ? x.DestinationUserWallet.Address : string.Empty,
                FromUser = x.IsFromSystem ? ApiMessages.FROM_SYSTEM : x.SourceUserWallet.ProfileName,
                ReferenceNumber = x.ReferenceNumber,
                ToUser = x.DestinationUserWallet != null ? x.DestinationUserWallet.ProfileName : string.Empty,
                TransactionStatus = x.Status,
                TransactionType = x.Type,
                Id = x.Id,
                SystemMessage = x.SystemMessage
            }).AsNoTracking();


        var countQuery = _context.WalletTransactions
            .Where(x =>
            (x.DestinationUserWallet.UserId == userId
            || x.SourceUserWallet.UserId == userId))
            .AsNoTracking().Select(x => new UserWalletTransactionItemResp { Id = x.Id });

        return await PaginatedList<UserWalletTransactionItemResp>.CreateAsync(query, countQuery, page, pageSize);
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
                SystemMessage = x.SystemMessage
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

        return data;
    }
    private async Task UpdateWalletInfor(Guid transactionId)
    {
        var transaction = await _context.WalletTransactions.AsNoTracking()
            .Include(x => x.DestinationUserWallet)
            .Include(x => x.SourceUserWallet)
            .FirstOrDefaultAsync(x => x.Id == transactionId);
        if (transaction == null)
        {
            throw new BadRequestException(ApiErrorCodes.TRANSACTION_NOT_FOUND, ApiErrorMessage.TRANSACTION_NOT_FOUND);
        }
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
                    if (transaction.SourceUserWallet.RewardPoint >= transaction.Amount)
                    {
                        transaction.SourceUserWallet.RewardPoint -= transaction.Amount;
                    }
                    else
                    {
                        var remainingAmount = transaction.Amount - transaction.SourceUserWallet.RewardPoint;
                        transaction.SourceUserWallet.RewardPoint = 0;
                        transaction.SourceUserWallet.Point -= remainingAmount;
                    }

                    transaction.DestinationUserWallet.Point += transaction.Amount;
                    break;
                }
        }
        _context.WalletTransactions.Update(transaction);
        await _context.SaveChangesAsync(default);
    }

    public async Task<UserWalletBasicResp> GetUserWalletAddress(Guid userId)
    {
        var result = new UserWalletBasicResp();

        var userWallets = await _context.UserWallets.AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => new
            {
                p.Address,
                p.Email,
                p.ProfileName
            }).FirstOrDefaultAsync();

        return result = new UserWalletBasicResp
        {
            Email = userWallets?.Email + "",
            WalletAddress = userWallets?.Address + "",
            ProfileName = userWallets?.ProfileName + ""
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
    public async Task<bool> VerifyTransactionOtpAsync(UserWalletVerifyTransactionOtpR req)
    {
        var otpData = await _context.WalletTransactionOtps.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.TransactionId == req.TransactionId
                         && x.Otp == req.Otp && x.OtpToken == req.OtpToken);
        if (otpData == null)
            throw new BadRequestException(ApiErrorCodes.OTP_INVALID, ApiErrorMessage.OTP_INVALID);

        if (DateTime.UtcNow.Subtract(otpData.CreatedOn).TotalMinutes > _setting.Otp.OtpExpired)
            throw new BadRequestException(ApiErrorCodes.OTP_EXPIRED, ApiErrorMessage.OTP_EXPIRED);

        await UpdateWalletInfor(req.TransactionId);

        await _context.SaveChangesAsync(default);
        await _otpService.ClearAllTransactionOtpOtpAsync(req.TransactionId);

        return true;
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
        if (req.FromAddress == req.ToAddress)
        {
            throw new ForbiddenAccessException(ApiErrorCodes.CAN_NOT_TRANSFER_THEMSELEVE, ApiErrorMessage.CAN_NOT_TRANSFER_THEMSELEVE);
        }
        var userId = req.UserId;
        var userWallet = await _context.UserWallets.Where(x => x.Address == req.FromAddress).FirstOrDefaultAsync();
        var toUserWallet = await _context.UserWallets.Where(x => x.Address == req.ToAddress).FirstOrDefaultAsync();
        //Check permission
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

    #endregion

    #region -- Fields --

    private readonly IBankService _bankService;
    private readonly IZaloPayService _zaloPayService;
    private readonly IOtpService _otpService;
    private readonly ISystemService _systemService;
    private readonly ILogger<UserWalletService> _logger;

    #endregion
}
