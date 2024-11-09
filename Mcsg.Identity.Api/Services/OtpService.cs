using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Identity.Api.Services;

using Common.Core.Constants;
using Common.Core.Distributor;
using Common.Core.Enums;
using Common.Domain;
using Common.Domain.Entities;
using Common.Interfaces;
using Common.Models;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Interfaces;
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
    /// <param name="unitOfWork"></param>
    /// <param name="distributeManager"></param>
    public OtpService(IMcsgContext context, ISetting setting, IUnitOfWork unitOfWork, DistributeManager distributeManager) : base(context, setting)
    {
        _distributeManager = distributeManager;
        _userOtpRepository = unitOfWork.GetRepository<UserOtp>();
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
            await _userOtpRepository.InsertAsync(userOtp);
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

    public async Task<bool> VerifyAsync(string otpToken, string otp, UserOtpType otpType)
    {
        try
        {
            var nowUtc = DateTime.UtcNow;
            var haveOtp = _context.UserOtps
                .Count(u => u.Code == otp && u.OtpType == otpType && u.ExpiryTime > nowUtc);
            var valid = haveOtp > 0;

            return valid;
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public async Task<UserOtp> GetAsync(string otpToken, UserOtpType otpType)
    {
        try
        {
            if (otpToken != null)
            {
                var dbOtp = await _userOtpRepository
                    .Connection.QueryFirstOrDefaultAsync<UserOtp>(GetOtpQuery, new { Token = otpToken, Type = otpType });

                return dbOtp;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public async Task<UserOtp> GetValidTokendAsync(string otpToken, UserOtpType otpType)
    {
        try
        {
            if (otpToken != null)
            {
                var dbOtp = await _userOtpRepository
                    .Connection.QueryFirstOrDefaultAsync<UserOtp>(GetValidOtpQuery, new { Token = otpToken, Type = otpType });

                return dbOtp;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public async Task<bool> ClearAllUserOtpAsync(Guid userId, UserOtpType otpType)
    {
        await _userOtpRepository.Connection.ExecuteAsync(DeleteUserOtpsQuery, new { UserId = userId, Type = otpType });
        return true;
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

    private readonly IRepository<UserOtp> _userOtpRepository;
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
