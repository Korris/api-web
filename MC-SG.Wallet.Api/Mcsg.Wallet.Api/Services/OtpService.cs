using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Distributor;
using Mcsg.Lib.Common.Exceptions;
using Mcsg.Lib.Common.Helpers;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Data.Wallet;
using Mcsg.Lib.Data.Wallet.Entities;
using Mcsg.Lib.Data.Wallet.Enums;
using Mcsg.Wallet.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Mcsg.Wallet.Api.Services
{
    public interface IOtpService
    {
        Task<TransactionOtpInfoResp> CreateAsync(WalletTransaction transaction, TransactionOtpType type, string otpToken = "");
        Task<bool> ClearAllTransactionOtpOtpAsync(Guid transactionId);
    }

    public class OtpService : IOtpService
    {
        private readonly WalletDbContext _walletDbContext;
        private readonly DistributeManager _distributeManager;
        private readonly OtpSetting _otpSetting;
        public OtpService(WalletDbContext walletDbContext,
            DistributeManager distributeManager,
            IOptions<OtpSetting> otpSettingoptions)
        {
            _otpSetting = otpSettingoptions.Value;
            _walletDbContext = walletDbContext;
            _distributeManager = distributeManager;
        }

        public async Task<bool> ClearAllTransactionOtpOtpAsync(Guid transactionId)
        {
            var removeItems = await _walletDbContext.WalletTransactionOtps
                .Where(x => x.TransactionId == transactionId).Select(x => new WalletTransactionOtp { Id = x.Id }).ToListAsync();
            _walletDbContext.RemoveRange(removeItems);
            await _walletDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<TransactionOtpInfoResp> CreateAsync(WalletTransaction transaction, TransactionOtpType type, string otpToken = "")
        {
            var id = Guid.NewGuid();
            var token = StringGenerator.GetRandomString(_otpSetting.OtpTokenLength);
            var otpCode = StringGenerator.GenerateOtp(_otpSetting.OtpLength);

            if (!string.IsNullOrEmpty(otpToken))
            {
                token = otpToken;
            }

            var otpInfo = new WalletTransactionOtp
            {
                Id = id,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                Otp = otpCode,
                OtpToken = token,
                OtpType = type,
                TransactionId = transaction.Id,
                Type = transaction.Type
            };
            try
            {
                await _walletDbContext.WalletTransactionOtps.AddAsync(otpInfo);
                await _walletDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ErrorCodes.OtpGenerateFail, ex.Message);
            }

            var otpReturn = new TransactionOtpInfoResp
            {
                TransactionId = otpInfo.TransactionId,
                Type = transaction.Type,
                OtpToken = token,
                OtpType = otpInfo.OtpType
            };

            if (type == TransactionOtpType.Email)
            {
                await CreateEmailOtpAsync(transaction.SourceUserWallet.Email,
                    type, otpInfo.Otp);
                otpReturn.Target = transaction.SourceUserWallet.Email;
            }
            else if (type == TransactionOtpType.Phone)
            {
                await CreateSmsOtpAsync(transaction.SourceUserWallet.PhoneNumber,
                    type, otpInfo.Otp);
                otpReturn.Target = transaction.SourceUserWallet.PhoneNumber;
            }

            return otpReturn;
        }

        private async Task CreateEmailOtpAsync(string to, TransactionOtpType type, string otpCode)
        {
            var emailJob = new EmailJobDistributeItem
            {
                Email = new Email { To = to, Body = otpCode },
                JobType = OtpJobTypeMapper[type]
            };

            await _distributeManager.Deliver(emailJob);
        }
        private async Task CreateSmsOtpAsync(string to, TransactionOtpType type, string otpCode)
        {
            var smsJob = new SmsJobDistributeItem
            {
                Sms = new Sms { To = to, Body = otpCode },
                JobType = OtpJobTypeMapper[type]
            };

            await _distributeManager.Deliver(smsJob);
        }

        private readonly IDictionary<TransactionOtpType, JobType> OtpJobTypeMapper =
            new Dictionary<TransactionOtpType, JobType>
        {
           {  TransactionOtpType.Email, JobType.ConfirmEmailOtp },
           {  TransactionOtpType.Phone, JobType.SmsOtp }
        };
    }
}
