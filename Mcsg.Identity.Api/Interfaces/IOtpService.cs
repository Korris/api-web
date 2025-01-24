namespace Mcsg.Identity.Api.Interfaces;

using Common.Core.Enums;
using Common.Domain.Entities;

public interface IOtpService
{
    Task<UserOtp> CreateAsync(Guid userId, string to, UserOtpType type, string otpToken = "");
    Task<bool> VerifyAsync(string token, string code, UserOtpType otpType);
    Task<UserOtp?> GetAsync(string token, UserOtpType otpType);
    Task<UserOtp?> GetAsync(string? token, string? code);
    Task<bool> ClearAllUserOtpAsync(Guid userId, UserOtpType otpType);
}
