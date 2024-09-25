using Microsoft.EntityFrameworkCore;

namespace Mcsg.Wallet.Api.Services;

using Common.Core.Distributor;
using Common.Core.Enums;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Models;
using Models;

public class OtpService : BaseSettingS, IOtpService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="setting"></param>
    /// <param name="distributeManager"></param>
    public OtpService(IWalletContext context, ISetting setting, DistributeManager distributeManager) : base(context, setting)
    {
        _distributeManager = distributeManager;
    }

    public async Task<bool> ClearAllTransactionOtpOtpAsync(Guid transactionId)
    {
        var removeItems = await _context.WalletTransactionOtps
            .Where(x => x.TransactionId == transactionId).Select(x => new WalletTransactionOtp { Id = x.Id }).ToListAsync();
        _context.WalletTransactionOtps.RemoveRange(removeItems);
        await _context.SaveChangesAsync(default);
        return true;
    }

    public async Task<TransactionOtpInfoResp> CreateAsync(WalletTransaction transaction, TransactionOtpType type, string otpToken = "")
    {
        var id = Guid.NewGuid();
        var token = _setting.Otp.OtpTokenLength.GetRandomString();
        var otpCode = _setting.Otp.OtpLength.GenerateOtp();

        if (!string.IsNullOrEmpty(otpToken))
        {
            token = otpToken;
        }

        var otpInfo = new WalletTransactionOtp
        {
            Id = id,
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow,
            Otp = otpCode,
            OtpToken = token,
            OtpType = type,
            TransactionId = transaction.Id,
            Type = transaction.Type
        };
        try
        {
            await _context.WalletTransactionOtps.AddAsync(otpInfo);
            await _context.SaveChangesAsync(default);
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

    #endregion

    #region -- Fields --

    private readonly DistributeManager _distributeManager;

    #endregion
}
