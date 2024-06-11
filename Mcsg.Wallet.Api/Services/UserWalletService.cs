using HD.ZaloPay.Helper.Crypto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;

namespace Mcsg.Wallet.Api.Services
{
    using Api.Constants;
    using Api.Helpers;
    using Api.Interfaces;
    using Api.Models._3rdClass.ZaloPay.Request;
    using Api.Models._3rdClass.ZaloPay.Response;
    using Common.Core.Dtos;
    using Common.Core.Extensions;
    using Lib.Common.Enums;
    using Lib.Common.Exceptions;
    using Lib.Common.Helpers;
    using Lib.Common.Models;
    using Lib.Common.Web.RealTime.Services;
    using Lib.Common.Web.Security;
    using Lib.Data.Wallet;
    using Lib.Data.Wallet.Entities;
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
    public class UserWalletService : IUserWalletService
    {
        public UserWalletService(WalletDbContext walletDbContext,
            ICurrentUserService currentUserService,
            IOtpService otpService,
            IBankService bankService,
            ISystemService systemService,
            IZaloPayService zaloPayService,
            IConfiguration configuration,
            ISignalRService signalRService,
            IOptions<ZaloPaySetting> zaloPaySettingOptions,
            IServiceProvider serviceProvider,
            ILogger<UserWalletService> logger)
        {
            _otpService = otpService;
            _configuration = configuration;
            _bankService = bankService;
            _currentUserService = currentUserService;
            _systemService = systemService;
            _zaloPayService = zaloPayService;
            _signalRService = signalRService;
            _zaloPaySetting = zaloPaySettingOptions.Value;
            _dbContext = walletDbContext;
            _setting = serviceProvider.GetRequiredService<ISetting>();
            _logger = logger;
        }

        #region User info
        public async Task<IEnumerable<UserWalletResp>> GetUserWalletAsync()
        {
            var userId = _currentUserService?.Session?.UserId;
            if (userId == null)
                return new List<UserWalletResp>();
            var data = await _dbContext.UserWallets
                .Where(x => x.UserId == _currentUserService.Session.UserId)
                .AsNoTracking().FirstOrDefaultAsync();

            var user = new UserWalletResp
            {
                WalletAddress = data.Address,
                Point = data.Point,
                RewardPoint = data.RewardPoint,
                TotalPoint = data.Point + data.RewardPoint,
                Owner = _currentUserService.Session.ProfileName
            };
            //Select premium 
            var lastPackage = await _dbContext.UserPremiumPackages.Where(x => x.UserWalletId == data.Id).OrderByDescending(x => x.EndDate).FirstOrDefaultAsync();
            if (lastPackage != null)
            {
                user.PremiumDate = DateOnly.FromDateTime(lastPackage.EndDate);
            }

            return new List<UserWalletResp> { user };
        }

        public async Task<UserWalletBasicResp> GetUserWalletByAddressAsync(string address)
        {
            var data = await _dbContext.UserWallets
                .Where(x => x.Address == address)
                .AsNoTracking()
                .Select(x => new UserWalletBasicResp
                {
                    WalletAddress = x.Address,
                    ProfileName = x.ProfileName,
                    Email = string.IsNullOrWhiteSpace(x.Email) ? x.ProfileName : StringHelper.MaskDigits(x.Email, 3, 3)
                }).FirstOrDefaultAsync();

            if (data == null)
            {
                throw new BadRequestException(ApiErrorCodes.WALLET_ADDRESS_NOT_FOUND, ApiErrorMessage.WALLET_ADDRESS_NOT_FOUND);
            }

            return data;
        }
        public async Task<PaginatedList<UserWalletTransactionItemResp>> GetUserWalletTransactionsAsync(int page = 1, int pageSize = 10)
        {
            var data = new UserWalletTransactionResp();
            var query = _dbContext.WalletTransactions
                .Include(x => x.SourceUserWallet)
                .Include(x => x.DestinationUserWallet)
                .Where(x =>
                    (
                        (x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == _currentUserService.Session.UserId)
                        || (x.SourceUserWallet != null && x.SourceUserWallet.UserId == _currentUserService.Session.UserId))

                    )
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new UserWalletTransactionItemResp
                {
                    Amount = x.Amount,
                    AmountSign = (x.Type == TransactionType.DEPOSIT
                                || x.Type == TransactionType.REWARD
                                || (x.Type == TransactionType.DONATE && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == _currentUserService.Session.UserId)
                                || (x.Type == TransactionType.TRANSFER && x.DestinationUserWallet != null && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == _currentUserService.Session.UserId)
                                ) ? "+" : "-",
                    Content = x.Content,
                    CreatedDate = x.CreatedDate,
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


            var countQuery = _dbContext.WalletTransactions
                .Where(x =>
                (x.DestinationUserWallet.UserId == _currentUserService.Session.UserId
                || x.SourceUserWallet.UserId == _currentUserService.Session.UserId))
                .AsNoTracking().Select(x => new UserWalletTransactionItemResp { Id = x.Id });

            return await PaginatedList<UserWalletTransactionItemResp>.CreateAsync(query, countQuery, page, pageSize);
        }
        public async Task<UserWalletTransactionItemResp> GetUserWalletTransactionByRefNumberAsync(string referenceNumber)
        {
            var data = await _dbContext.WalletTransactions
                .Include(x => x.SourceUserWallet)
                .Include(x => x.DestinationUserWallet)
                .Include(x => x.UserPaymentMethods).ThenInclude(x => x.PaymentMethod)
                .Where(x => x.ReferenceNumber == referenceNumber)
                .AsNoTracking()
                .Select(x => new UserWalletTransactionItemDetailResp
                {
                    Amount = x.Amount,
                    AmountSign = (x.Type == TransactionType.DEPOSIT
                                || x.Type == TransactionType.REWARD
                                || (x.Type == TransactionType.DONATE && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == _currentUserService.Session.UserId)
                                || (x.Type == TransactionType.TRANSFER && x.DestinationUserWallet != null && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == _currentUserService.Session.UserId)
                                ) ? "+" : "-",
                    Content = x.Content,
                    CreatedDate = x.CreatedDate,
                    FromAddress = x.IsFromSystem ? ApiMessages.FROM_SYSTEM : x.SourceUserWallet.Address,
                    FromUserId = x.IsFromSystem ? Guid.Empty : x.SourceUserWallet.UserId,
                    ToAddress = x.DestinationUserWallet != null ? x.DestinationUserWallet.Address : string.Empty,
                    FromUser = x.IsFromSystem ? ApiMessages.FROM_SYSTEM : x.SourceUserWallet.ProfileName,
                    PaymentMethodId = (x.UserPaymentMethodId != null && x.UserPaymentMethods != null) ? x.UserPaymentMethods.PaymentMethod.Id : null,
                    PaymentMethod = (
                        x.UserPaymentMethods != null) ?
                        new PaymentMethodResp { Logo = x.UserPaymentMethods.PaymentMethod.Logo, Name = x.UserPaymentMethods.PaymentMethod.Name } :
                        (x.Type == TransactionType.DEPOSIT && x.SystemMethod != null) ? new PaymentMethodResp
                        {
                            Name = x.SystemMethod == SystemPaymentMethod.Bank ? "Direct Banking" : x.SystemMethod == SystemPaymentMethod.ZaloPay ? "Zalo Pay" : ""
                        } : null,
                    ReferenceNumber = x.ReferenceNumber,
                    ToUser = x.DestinationUserWallet != null ? x.DestinationUserWallet.ProfileName : string.Empty,
                    ToUserId = x.DestinationUserWallet != null ? x.DestinationUserWallet.UserId : ((x.Type == TransactionType.DEPOSIT || x.Type == TransactionType.BUY_PREMIUM) ? Guid.Empty : null),
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
            var transaction = await _dbContext.WalletTransactions.AsNoTracking()
                .Include(x => x.DestinationUserWallet)
                .Include(x => x.SourceUserWallet)
                .FirstOrDefaultAsync(x => x.Id == transactionId);
            if (transaction == null)
            {
                throw new BadRequestException(ApiErrorCodes.TRANSACTION_NOT_FOUND, ApiErrorMessage.TRANSACTION_NOT_FOUND);
            }
            CheckBalance(transaction.SourceUserWallet.Point, transaction.SourceUserWallet.RewardPoint, transaction.Amount);

            transaction.Status = TransactionStatus.SUCCESS;
            transaction.IsConfirmed = true;

            switch (transaction.Type)
            {
                case TransactionType.DEPOSIT:
                    {
                        transaction.DestinationUserWallet.Point += transaction.Amount;
                        break;
                    }
                case TransactionType.DONATE:
                case TransactionType.TRANSFER:
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
            _dbContext.Update(transaction);
        }

        #endregion

        #region User payment method
        public async Task<bool> RemoveUserPaymentMethod(Guid userPaymentMethodId)
        {
            var paymentMethod = _dbContext.UserPaymentMethods.Where(
                 x => x.UserWallet.UserId == _currentUserService.Session.UserId && x.Id == userPaymentMethodId && x.IsDelete == false
                ).FirstOrDefault();

            if (paymentMethod != null)
            {
                paymentMethod.IsDelete = true;
                _dbContext.UserPaymentMethods.Update(paymentMethod);
                await _dbContext.SaveChangesAsync();
            }

            return true;
        }

        public async Task<IEnumerable<UserPaymentMethodResponse>> GetUserPaymentMethods()
        {
            string encryptKey = _configuration[SystemSettings.CONST_PAYMENT_ENCRYPTION_KEY];

            var userPaymentMethods = await _dbContext.UserPaymentMethods
                .Include(x => x.PaymentMethod)
                .Where(x => x.UserWallet.UserId == _currentUserService.Session.UserId && !x.IsDelete)
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

        public async Task<AddUserPaymentMethodResp> AddUserPaymentMethod(AddUserPaymentMethodReq addUserPaymentMethodReq)
        {
            var paymentMethod = await _dbContext.PaymentMethods
                        .Where(x => x.Id == addUserPaymentMethodReq.PaymentMethodId)
                        .FirstOrDefaultAsync();
            var userWallet = await _dbContext.UserWallets.Where(x => x.UserId == _currentUserService.Session.UserId).FirstOrDefaultAsync();

            if (paymentMethod == null || paymentMethod.AllowWithdrawal == false)
            {
                throw new BadRequestException(ApiErrorCodes.ERROR_PAYMENT_METHOD_TYPE, ApiErrorMessage.ERROR_PAYMENT_METHOD_TYPE);
            }
            string encryptKey = _configuration[SystemSettings.CONST_PAYMENT_ENCRYPTION_KEY];
            var entity = new UserPaymentMethod
            {
                UserWalletId = userWallet.Id,
                PaymentMethod = paymentMethod,
                AccountNumber = addUserPaymentMethodReq.AccountNumber != null ? CryptoHelper.Encrypt(addUserPaymentMethodReq.AccountNumber, encryptKey) : null,
                AccountName = addUserPaymentMethodReq.AccountName != null ? CryptoHelper.Encrypt(addUserPaymentMethodReq.AccountName, encryptKey) : null
            };

            _dbContext.UserPaymentMethods.Add(entity);
            await _dbContext.SaveChangesAsync();

            return new AddUserPaymentMethodResp
            {
                Id = entity.Id,
                PaymentMethodId = paymentMethod.Id,
                AccountNumber = addUserPaymentMethodReq.AccountNumber,
                AccountName = addUserPaymentMethodReq.AccountName,
            };
        }
        public async Task<UpdateUserPaymentMethodResp> UpdateUserPaymentMethod(Guid userPaymentMethodId, UpdateUserPaymentMethodReq updateUserPaymentMethodReq)
        {
            var userPaymentMethod = await _dbContext.UserPaymentMethods
                .Include(x => x.PaymentMethod)
                        .Where(x => x.Id == userPaymentMethodId)
                        .FirstOrDefaultAsync();
            var userWallet = await _dbContext.UserWallets.Where(x => x.UserId == _currentUserService.Session.UserId).FirstOrDefaultAsync();

            if (userPaymentMethod == null)
            {
                throw new BadRequestException("NOT FOUND", ApiErrorMessage.ERROR_PAYMENT_METHOD_TYPE);
            }
            string encryptKey = _configuration[SystemSettings.CONST_PAYMENT_ENCRYPTION_KEY];

            userPaymentMethod.AccountNumber = updateUserPaymentMethodReq.AccountNumber != null ? CryptoHelper.Encrypt(updateUserPaymentMethodReq.AccountNumber, encryptKey) : null;
            userPaymentMethod.AccountName = updateUserPaymentMethodReq.AccountName != null ? CryptoHelper.Encrypt(updateUserPaymentMethodReq.AccountName, encryptKey) : null;
            if (updateUserPaymentMethodReq?.PaymentMethodId != null && userPaymentMethod.PaymentMethodId != updateUserPaymentMethodReq?.PaymentMethodId)
            {
                var paymentMethod = await _dbContext.PaymentMethods
                       .Where(x => x.Id == updateUserPaymentMethodReq.PaymentMethodId)
                       .FirstOrDefaultAsync();

                if (paymentMethod == null || paymentMethod.AllowWithdrawal == false)
                {
                    throw new BadRequestException(ApiErrorCodes.ERROR_PAYMENT_METHOD_TYPE, ApiErrorMessage.ERROR_PAYMENT_METHOD_TYPE);
                }
                userPaymentMethod.PaymentMethod = paymentMethod;
            }

            _dbContext.UserPaymentMethods.Update(userPaymentMethod);
            await _dbContext.SaveChangesAsync();

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
        public async Task<bool> VerifyTransactionOtpAsync(VerifyTransactionOtpReq req)
        {
            var otpData = await _dbContext.WalletTransactionOtps.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.TransactionId == req.TransactionId
                             && x.Otp == req.Otp && x.OtpToken == req.OtpToken);
            if (otpData == null)
                throw new BadRequestException(ApiErrorCodes.OTP_INVALID, ApiErrorMessage.OTP_INVALID);

            if (DateTime.UtcNow.Subtract(otpData.CreatedDate).TotalMinutes > _configuration.OtpExpired())
                throw new BadRequestException(ApiErrorCodes.OTP_EXPIRED, ApiErrorMessage.OTP_EXPIRED);

            await UpdateWalletInfor(req.TransactionId);

            await _dbContext.SaveChangesAsync();
            await _otpService.ClearAllTransactionOtpOtpAsync(req.TransactionId);

            return true;
        }
        public async Task<TransactionOtpInfoResp> ResentTransactionOtpAsync(Guid transactionId, TransactionOtpType otpType)
        {
            var transaction = await _dbContext.WalletTransactions.AsNoTracking()
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
        public async Task<TransactionOtpInfoResp> DonateAsync(DonateReq req)
        {
            var userId = _currentUserService?.Session?.UserId;
            var userWallet = await _dbContext.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            var toUserWallet = await _dbContext.UserWallets.Where(x => x.UserId == req.ToUserId).FirstOrDefaultAsync();
            if (toUserWallet != null)
            {
                if (string.IsNullOrEmpty(userWallet.Email)
                    && string.IsNullOrEmpty(userWallet.PhoneNumber))
                {
                    return null;
                }

                CheckBalance(userWallet.Point, userWallet.RewardPoint, req.Amount);

                var transaction = CreateTransaction(
                    userWallet.Id,
                    toUserWallet.Id,
                    req.Amount,
                    TransactionType.DONATE,
                    req.Content,
                    ApiMessages.DONATE_TO_USER);
                transaction.SystemMethod = SystemPaymentMethod.Point;
                await _dbContext.WalletTransactions.AddAsync(transaction);

                var otpType = !string.IsNullOrEmpty(userWallet.Email) ? TransactionOtpType.Email : TransactionOtpType.Phone;

                var otpInfo = await _otpService.CreateAsync(transaction, otpType);
                otpInfo.ToProfileName = toUserWallet.ProfileName;

                await _dbContext.SaveChangesAsync();

                return otpInfo;
            }
            else
            {
                throw new BadRequestException(ApiErrorCodes.USER_NOT_FOUND, ApiErrorMessage.USER_NOT_FOUND);
            }
        }
        #endregion

        #region Transfer
        public async Task<TransactionOtpInfoResp> TransferAsync(TransferReq req)
        {
            if (req.FromAddress == req.ToAddress)
            {
                throw new ForbiddenAccessException(ApiErrorCodes.CAN_NOT_TRANSFER_THEMSELEVE, ApiErrorMessage.CAN_NOT_TRANSFER_THEMSELEVE);
            }
            var userId = _currentUserService?.Session?.UserId;
            var userWallet = await _dbContext.UserWallets.Where(x => x.Address == req.FromAddress).FirstOrDefaultAsync();
            var toUserWallet = await _dbContext.UserWallets.Where(x => x.Address == req.ToAddress).FirstOrDefaultAsync();
            //Check permission
            if (userWallet?.UserId != userId || userId == null)
            {
                throw new ForbiddenAccessException(ApiErrorCodes.USER_NOT_PERMISSION, ApiErrorMessage.USER_NOT_PERMISSION);
            }

            if (userWallet != null && toUserWallet != null)
            {
                CheckBalance(userWallet.Point, userWallet.RewardPoint, req.Amount);

                var transaction = CreateTransaction(
                    userWallet.Id,
                    toUserWallet.Id,
                    req.Amount,
                    TransactionType.TRANSFER,
                    req.Content,
                    ApiMessages.TRANSFER_TO_USER);
                transaction.SystemMethod = SystemPaymentMethod.Point;
                await _dbContext.WalletTransactions.AddAsync(transaction);

                transaction.SourceUserWallet = userWallet;

                var otpType = !string.IsNullOrEmpty(userWallet.Email) ? TransactionOtpType.Email : TransactionOtpType.Phone;
                var otpInfo = await _otpService.CreateAsync(transaction, otpType);
                otpInfo.ToProfileName = toUserWallet.ProfileName;

                await _dbContext.SaveChangesAsync();

                return otpInfo;
            }
            else
            {
                throw new BadRequestException(ApiErrorCodes.USER_NOT_FOUND, ApiErrorMessage.USER_NOT_FOUND);
            }
        }

        #endregion

        #region Withdraw
        public async Task<WithDrawPrepareResp> WithdrawPrepareAsync()
        {
            var result = new WithDrawPrepareResp();
            var userId = _currentUserService?.Session?.UserId;
            var userWallet = await _dbContext.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            result.UserPaymentMethods = _dbContext.UserPaymentMethods.Where(x => x.UserWalletId == userWallet.Id && !x.IsDelete)
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
            result.MinPointCanWithDraw = SystemConfig.MinimumPointCanWithDraw;
            result.MaxPointCanWithDraw = userWallet.Point;
            return result;
        }

        public async Task<TransactionOtpInfoResp> WithdrawAsync(WithdrawReq req)
        {
            var userId = _currentUserService?.Session?.UserId;
            var userWallet = await _dbContext.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if (userWallet == null)
            {
                throw new BadRequestException(ApiErrorCodes.USER_NOT_FOUND, ApiErrorMessage.USER_NOT_FOUND);
            }

            if (userWallet.Point < req.Amount)
            {
                throw new BadRequestException("Amount over", "");
            }
            if (req.Amount < SystemConfig.MinimumPointCanWithDraw)
            {
                throw new BadRequestException("MinimumPointCanWithDraw", "");
            }

            var transaction = CreateTransaction(
                    userWallet.Id,
                    req.UserPaymentMethodId,
                    req.Amount,
                    TransactionType.WITHDRAW,
                    req.Content,
                    ApiMessages.WITHDRAW_MESSAGE);
            transaction.SystemMethod = SystemPaymentMethod.Bank;

            transaction.Content = string.Format(ApiMessages.WITHDRAW_CONTENT, req.Amount, transaction.ReferenceNumber);
            await _dbContext.WalletTransactions.AddAsync(transaction);

            var otpInfo = await _otpService.CreateAsync(transaction, TransactionOtpType.Email);
            otpInfo.ToProfileName = userWallet.ProfileName;

            await _systemService.SendAdminNoti(nameof(TransactionType.WITHDRAW), _currentUserService?.Session?.ProfileId, transaction.Content);
            await _dbContext.SaveChangesAsync();

            return otpInfo;
        }
        #endregion

        #region Deposit
        public async Task<DepositPrepareResp> DepositPrepareAsync()
        {
            var result = new DepositPrepareResp();
            var userId = _currentUserService?.Session?.UserId;
            var userWallet = await _dbContext.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            result.DepositMethods = _bankService.GetDepositMethods();
            result.CurrencyTypes = _bankService.GetCurrencyTypeRatios();
            result.MinPointCanDeposit = SystemConfig.MinimumPointCanDeposit;
            result.MaxPointCanDeposit = 999999;
            return result;
        }
        public async Task<DepositResp> DepositAsync(DepositReq req)
        {
            var userId = _currentUserService?.Session?.UserId;
            var userWallet = await _dbContext.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if (userWallet == null)
            {
                throw new BadRequestException(ApiErrorCodes.USER_NOT_FOUND, ApiErrorMessage.USER_NOT_FOUND);
            }

            if (req.PointAmount < SystemConfig.MinimumPointCanDeposit)
            {
                throw new BadRequestException(ApiErrorCodes.MINIMUM_CAN_DEPOSIT, ApiErrorMessage.MINIMUM_CAN_DEPOSIT + SystemConfig.MinimumPointCanDeposit.ToString());
            }

            var transaction = CreateTransaction(
                    userWallet.Id,
                    null,
                    req.PointAmount,
                    TransactionType.DEPOSIT,
                    "",
                    ApiMessages.DEPOSIT_MESSAGE);
            var currencyRatio = _bankService.GetCurrencyTypeRatios().FirstOrDefault(x => x.Type == req.CurrencyType)?.Ratio ?? 0;
            var amountToDeposit = currencyRatio * req.PointAmount;
            transaction.SystemMessage = $"{transaction.SystemMessage} - RATIO: {currencyRatio} AMOUNT: {amountToDeposit}"; //TODO currencyTYPE
            transaction.Content = string.Format(ApiMessages.DEPOSIT_CONTENT, req.PointAmount, transaction.ReferenceNumber, req.DepositMethodName);
            transaction.SystemMethod = req.DepositMethodName == DepositMethods.BANK ?
                SystemPaymentMethod.Bank : req.DepositMethodName == DepositMethods.ZALO_PAY ?
                SystemPaymentMethod.ZaloPay : null;

            await _dbContext.WalletTransactions.AddAsync(transaction);
            try
            {

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw;
            }

            //Banking
            if (req.DepositMethodName == DepositMethods.BANK)
            {
                string bankAccount = _configuration["Bank:AdminBankAccount"];

                string bankAccountName = _configuration["Bank:AdminBankName"];
                string bankAccountBin = _configuration["Bank:AdminBankBin"];
                string bankName = (await _dbContext.PaymentMethods.Where(x => x.Type == Lib.Data.Wallet.Enums.PaymentMethodType.BANKING && x.Bin == bankAccountBin)
                    .FirstOrDefaultAsync())?.Name;

                var depositResp = new DepositResp()
                {
                    QRCodeUrl = CreateQRCode(amountToDeposit, transaction.ReferenceNumber),
                    TransactionId = transaction.Id,
                    Type = TransactionType.DEPOSIT,
                    ToBankName = bankName,
                    ToAccountName = bankAccountName,
                    ToAccountNumber = bankAccount,
                };

                await _systemService.SendAdminNoti(nameof(TransactionType.DEPOSIT), _currentUserService?.Session?.ProfileId, transaction.Content);

                return depositResp;
            }
            else if (req.DepositMethodName == DepositMethods.ZALO_PAY)
            {
                var createOrderRes = await _zaloPayService.CreateOrderAsync(transaction.Id, transaction.Content, amountToDeposit, userId, req.RedirectUrl);

                if (createOrderRes != null && createOrderRes.ReturnCode == (int)ZaloPayReturnCode.SUCCESS)
                {
                    transaction.ExternalId = createOrderRes.AppTransId;
                    await _dbContext.SaveChangesAsync();

                    // Send queue to check zalo order
                    var jobData = new PaymentTransData()
                    {
                        Type = PaymentTransType.ZALO_PAY,
                        TransactionId = transaction.Id,
                        UserId = userId.Value,
                        Status = TransactionStatus.PENDING
                    };

                    var msg = new QueueMessageDto(jobData);
                    _setting.SendMessageToQueue(_setting.NotificationExchange, _setting.NotificationQueuePayment, msg);

                    var depositResp = new DepositResp()
                    {
                        QRCodeUrl = "",
                        TransactionId = transaction.Id,
                        Type = TransactionType.DEPOSIT,
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
        public async Task<CallBackZaloPayResponse> CallBackZaloPayAsync(ZaloPayCallBackReq req)
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
                var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, _zaloPaySetting.Key2, dataStr);
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
                        await _zaloPayService.CompleteTransactionAsync(transId, userId, TransactionStatus.SUCCESS);
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

        public async Task<bool> DepositCancelAsync(DepositCancelReq req)
        {
            var userId = _currentUserService?.Session?.UserId;
            var userWallet = await _dbContext.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if (userWallet == null)
            {
                throw new BadRequestException(ApiErrorCodes.USER_NOT_FOUND, ApiErrorMessage.USER_NOT_FOUND);
            }

            var transaction = await _dbContext.WalletTransactions.Where(x => x.Id == req.TransactionId
            && x.SourceUserWalletId == userWallet.Id && x.Type == TransactionType.DEPOSIT).FirstOrDefaultAsync();
            if (transaction == null)
            {
                throw new BadRequestException(ApiErrorCodes.TRANSACTION_NOT_FOUND, ApiErrorMessage.TRANSACTION_NOT_FOUND);
            }
            transaction.Status = TransactionStatus.CANCELED;

            _dbContext.WalletTransactions.Update(transaction);
            await _dbContext.SaveChangesAsync();

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
                var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, _zaloPaySetting.Key2, dataStr);

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

        #region Private Method
        private WalletTransaction CreateTransaction(Guid sourceId,
            Guid? destinationId,
            float amount,
            TransactionType type,
            string content,
            string systemMessage)
        {
            var transaction = new WalletTransaction
            {
                CreatedDate = DateTime.UtcNow,
                CreatedBy = _currentUserService?.Session?.UserId,
                Id = Guid.NewGuid(),
                Amount = amount,
                IsFromSystem = false,
                Content = content,
                SystemMessage = systemMessage,
                ReferenceNumber = StringHelper.GetRandomString(SystemConfig.ReferenceNumberLength).ToLower(),
                ModifiedDate = DateTime.UtcNow,
                SourceUserWalletId = sourceId,
                Status = TransactionStatus.PENDING,
                Type = type
            };
            if (type == TransactionType.WITHDRAW || type == TransactionType.DEPOSIT)
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
            string bankAccount = _configuration["Bank:AdminBankAccount"];
            string bankAccountName = _configuration["Bank:AdminBankName"];
            string bankAccountBin = _configuration["Bank:AdminBankBin"];

            return $"https://api.vietqr.io/image/{bankAccountBin}-{bankAccount}-PSYZ8LO.jpg?accountName={bankAccountName}&amount={amount}&addInfo={refCode}";
        }
        #endregion

        #region -- Fields --

        private readonly WalletDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IConfiguration _configuration;
        private readonly IBankService _bankService;
        private readonly IZaloPayService _zaloPayService;
        private readonly IOtpService _otpService;
        private readonly ISystemService _systemService;
        private readonly ZaloPaySetting _zaloPaySetting;
        private readonly ISignalRService _signalRService;
        private readonly ILogger<UserWalletService> _logger;

        /// <summary>
        /// Setting
        /// </summary>
        private readonly ISetting _setting;

        #endregion
    }
}
