using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Identity.Services;

using Common.Core.Constants;
using Common.Core.Distributor;
using Common.Core.Enums;
using Common.Domain;
using Common.Domain.Entities;
using Common.Models;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Mcsg.Api.Areas.Identity.Interfaces;
using Mcsg.Api.Interfaces;
using Models;

/// <summary>
/// OtpService
/// </summary>
public partial class OtpService : BaseSettingS, IOtpService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="setting"></param>
    /// <param name="distributeManager"></param>
    public OtpService(IMcsgContext context, ISetting setting, DistributeManager distributeManager) : base(context, setting)
    {
        _distributeManager = distributeManager;
    }

    public async Task<UserOtp> CreateAsync(Guid userId, string to, UserOtpType type, string otpToken = "")
    {
        var id = Guid.NewGuid();
        var token = _setting.Otp.OtpTokenLength.GetRandomString();
        var otpCode = _setting.Otp.OtpLength.GenerateOtp();
        if (!string.IsNullOrEmpty(otpToken))
        {
            token = otpToken;
        }
        var userOtp = new UserOtp
        {
            Id = id,
            UserId = userId,
            Code = otpCode,
            Destination = to,
            Token = token,
            OtpType = type,
            ExpiryTime = DateTime.UtcNow.AddMinutes(_setting.Otp.ExpiryInMinutes),
        };

        try
        {
            await _context.UserOtps.AddAsync(userOtp);
            await _context.SaveChangesAsync(default);
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.OtpGenerateFail, ex.Message);
        }

        if (type == UserOtpType.VerifyEmail
            || type == UserOtpType.ResetByEmail
            || type == UserOtpType.ConfirmEmail)
        {
            await CreateEmailOtpAsync(to, type, userOtp);
        }

        if (type == UserOtpType.VerifyPhone
            || type == UserOtpType.ResetByPhone
            || type == UserOtpType.ConfirmPhone)
        {
            await CreateSmsOtpAsync(to, type, userOtp);
        }

        return userOtp;
    }

    public async Task<bool> VerifyAsync(string token, string code, UserOtpType otpType)
    {
        if (string.IsNullOrWhiteSpace(token) && string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        var utc = DateTime.UtcNow;
        return await _context.UserOtps.AnyAsync(p => p.Token == token && p.Code == code && p.OtpType == otpType && p.ExpiryTime > utc);
    }

    public async Task<UserOtp?> GetAsync(string token, UserOtpType otpType)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        return await _context.UserOtps.FirstOrDefaultAsync(p => p.Token == token && p.OtpType == otpType);
    }

    public async Task<UserOtp?> GetAsync(string? token, string? code)
    {
        if (string.IsNullOrWhiteSpace(token) && string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        return await _context.UserOtps.FirstOrDefaultAsync(p => p.Token == token && p.Code == code);
    }

    public async Task<bool> ClearAllUserOtpAsync(Guid userId, UserOtpType otpType)
    {
        return await _context.UserOtps.Where(p => p.UserId == userId && p.OtpType == otpType).ExecuteDeleteAsync() > 0;
    }

    private async Task CreateEmailOtpAsync(string to, UserOtpType type, UserOtp userOtp)
    {
        var emailJob = new EmailJobDistributeItem
        {
            Email = new Email { To = to, Body = userOtp.Code },
            JobType = _otpJobTypeMapper[type]
        };

        await _distributeManager.Deliver(emailJob);
    }

    private async Task CreateSmsOtpAsync(string to, UserOtpType type, UserOtp userOtp)
    {
        var smsJob = new SmsJobDistributeItem
        {
            Sms = new Sms { To = to, Body = userOtp.Code },
            JobType = _otpJobTypeMapper[type]
        };

        await _distributeManager.Deliver(smsJob);
    }

    #endregion

    #region -- Fields --

    private readonly DistributeManager _distributeManager;

    private readonly IDictionary<UserOtpType, JobType> _otpJobTypeMapper = new Dictionary<UserOtpType, JobType> {
        { UserOtpType.VerifyEmail, JobType.VerifyByEmailOtp },
        { UserOtpType.ResetByEmail, JobType.ResetByEmailOtp },
        { UserOtpType.ConfirmEmail, JobType.ConfirmEmailOtp },
        { UserOtpType.VerifyPhone, JobType.SmsOtp },
        { UserOtpType.ResetByPhone, JobType.SmsOtp },
        { UserOtpType.ConfirmPhone, JobType.SmsOtp }
    };

    #endregion
}
