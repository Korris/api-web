namespace Mcsg.Identity.Api.Interfaces;

using Lib.Data.Domain.Entities;
using Lib.Data.Enums;

public interface IOtpService
{
    Task<UserOtp> GetAsync(string otpToken, UserOtpType otpType);
    Task<bool> VerifyAsync(string otpToken, string otp, UserOtpType otpType);
    Task<UserOtp> CreateAsync(Guid userId, string to, UserOtpType type, string otpToken = "");
    Task<bool> ClearAllUserOtpAsync(Guid userId, UserOtpType otpType);
}
