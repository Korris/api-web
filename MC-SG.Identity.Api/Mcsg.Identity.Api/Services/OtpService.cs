using Dapper;
using Mcsg.Identity.Api.Models;
using Mcsg.Identity.Api.Services.Interfaces;
using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Distributor;
using Mcsg.Lib.Common.Exceptions;
using Mcsg.Lib.Common.Helpers;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Microsoft.Extensions.Options;

namespace Mcsg.Identity.Api.Services
{
    public partial class OtpService : IOtpService
    {
        private readonly IRepository<UserOtp> _userOtpRepository;
        private readonly OtpSetting _otpSetting;
        private readonly DistributeManager _distributeManager;
        public OtpService(IUnitOfWork unitOfWork
            , DistributeManager distributeManager
            , IOptions<OtpSetting> otpConfiguration)
        {
            _distributeManager = distributeManager;
            _userOtpRepository = unitOfWork.GetRepository<UserOtp>();
            _otpSetting = otpConfiguration.Value;
        }

        public async Task<UserOtp> CreateAsync(Guid userId, string to, UserOtpType type, string otpToken = "")
        {
            var id = Guid.NewGuid();
            var token = StringGenerator.GetRandomString(_otpSetting.OtpTokenLength);
            var otpCode = StringGenerator.GenerateOtp(_otpSetting.OtpLength);
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
                ExpiryTime = DateTime.UtcNow.AddMinutes(_otpSetting.ExpiryInMinutes),
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
                var valid = false;
                if (otpToken != null)
                {
                    var dbOtp = await _userOtpRepository
                        .Connection.QueryFirstOrDefaultAsync<UserOtp>(GetValidTokenQuery, new { Token = otpToken, Type = otpType });

                    if (dbOtp != null)
                    {
                        valid = dbOtp?.Code == otp;
                    }
                }
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
                JobType = OtpJobTypeMapper[type]
            };

            await _distributeManager.Deliver(emailJob);
        }
        private async Task CreateSmsOtpAsync(string to, UserOtpType type, UserOtp userOtp)
        {
            var smsJob = new SmsJobDistributeItem
            {
                Sms = new Sms { To = to, Body = userOtp.Code },
                JobType = OtpJobTypeMapper[type]
            };

            await _distributeManager.Deliver(smsJob);
        }

        private readonly IDictionary<UserOtpType, JobType> OtpJobTypeMapper =
            new Dictionary<UserOtpType, JobType>
        {
           {  UserOtpType.VerifyEmail, JobType.VerifyByEmailOtp },
           {  UserOtpType.ResetByEmail, JobType.ResetByEmailOtp },
           {  UserOtpType.ConfirmEmail, JobType.ConfirmEmailOtp },
           {  UserOtpType.VerifyPhone, JobType.SmsOtp },
           {  UserOtpType.ResetByPhone, JobType.SmsOtp },
           {  UserOtpType.ConfirmPhone, JobType.SmsOtp },
        };
    }
}
