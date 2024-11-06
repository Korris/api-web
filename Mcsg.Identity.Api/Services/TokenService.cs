using Microsoft.EntityFrameworkCore;

namespace Mcsg.Identity.Api.Services;

using Common.Core;
using Common.Core.Dtos;
using Common.Domain;
using Common.Domain.Entities;
using Interfaces;

/// <summary>
/// Token service
/// </summary>
public class TokenService : BaseSettingS, ITokenService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="userManager">User manager</param>
    public TokenService(IMcsgContext context, ISetting setting, ApplicationUserManager userManager) : base(context, setting)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// IsValid async
    /// </summary>
    /// <param name="rt">Refresh token</param>
    /// <returns>Returns the result</returns>
    public async Task<Guid?> IsValidAsync(string rt)
    {
        var ett = await _context.UserRefreshTokens.FirstOrDefaultAsync(p => p.RefreshToken == rt);
        if (ett == null)
        {
            return Guid.Empty;
        }

        if (ett.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            _context.UserRefreshTokens.Remove(ett);
            await _context.SaveChangesAsync(default);

            return Guid.Empty;
        }

        return ett.UserId;
    }

    /// <summary>
    /// Add async
    /// </summary>
    /// <param name="user">User</param>
    /// <returns>Returns the result</returns>
    public async Task<RefreshTokenDto?> AddAsync(User? user)
    {
        if (user == null)
        {
            return null;
        }

        var ett = await _context.UserRefreshTokens.OrderByDescending(p => p.RefreshTokenExpiryTime).FirstOrDefaultAsync(p => p.UserId == user.Id);
        if (ett != null)
        {
            ett.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_setting.Jwt.TimeRt);
        }
        else
        {
            ett = new UserRefreshToken
            {
                UserId = user.Id,
                RefreshToken = SecurityToken.GenerateToken(),
                RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_setting.Jwt.TimeRt)
            };
            await _context.UserRefreshTokens.AddAsync(ett);
        }

        await _context.SaveChangesAsync(default);

        return new RefreshTokenDto
        {
            RefreshToken = ett.RefreshToken,
            RefreshTokenExpiryTime = ett.RefreshTokenExpiryTime.Value
        };
    }

    /// <summary>
    /// Delete async
    /// </summary>
    /// <param name="userId">UserId</param>
    /// <returns>Returns the result</returns>
    public async Task<bool> DeleteAsync(Guid userId)
    {
        var etts = await _context.UserRefreshTokens.Where(p => p.UserId == userId).ToListAsync();
        _context.UserRefreshTokens.RemoveRange(etts);
        return await _context.SaveChangesAsync(default) > 0;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// User manager
    /// </summary>
    private readonly ApplicationUserManager _userManager;

    #endregion
}
