namespace Mcsg.Lib.Data.Repositories;

using Domain.Entities;
using Enums;

public interface IUserOtpRepository : IRepository<UserOtp>
{
    Task<IEnumerable<UserOtp>> GetByUserIdAsync(Guid userId);

    Task<IEnumerable<UserOtp>> GetByOtpTypeAsync(UserOtpType otpType);

    Task<IEnumerable<UserOtp>> GetByTokenAsync(string token, UserOtpType type);

    Task<IEnumerable<UserOtp>> GetByCodeAsync(string code);

    Task<bool> ClearAllOtp(Guid userId, UserOtpType otpType);
}