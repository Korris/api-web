namespace Mcsg.Identity.Api.Interfaces;

using Common.Core.Enums;
using Common.Domain.Entities;

public interface IOtpService
{
    Task<UserOtp> GetAsync(string otpToken, UserOtpType otpType);
    Task<bool> VerifyAsync(string otpToken, string otp, UserOtpType otpType);
    Task<UserOtp> CreateAsync(Guid userId, string to, UserOtpType type, string otpToken = "");
    Task<bool> ClearAllUserOtpAsync(Guid userId, UserOtpType otpType);
}
