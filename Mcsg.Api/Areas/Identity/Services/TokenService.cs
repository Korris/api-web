using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Identity.Services;

using Common.Core;
using Common.Core.Dtos;
using Common.Domain;
using Common.Domain.Entities;
using Mcsg.Api.Areas.Identity.Interfaces;
using Mcsg.Api.Interfaces;

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
    public TokenService(IMcsgContext context, ISetting setting) : base(context, setting) { }

    /// <summary>
    /// IsValid async
    /// </summary>
    /// <param name="rt">Refresh token</param>
    /// <returns>Returns the result</returns>
    public async Task<Guid?> IsValidAsync(string? rt)
    {
        if (string.IsNullOrWhiteSpace(rt))
        {
            return null;
        }

        var ett = await _context.UserRefreshTokens.FirstOrDefaultAsync(p => p.RefreshToken == rt);
        if (ett == null)
        {
            return null;
        }

        if (ett.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            _context.UserRefreshTokens.Remove(ett);
            await _context.SaveChangesAsync(default);

            return null;
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

        var ett = new UserRefreshToken
        {
            UserId = user.Id,
            SessionId = user.SessionId,
            RefreshToken = SecurityToken.GenerateToken(),
            RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_setting.Jwt.TimeRt)
        };

        await _context.UserRefreshTokens.AddAsync(ett);
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
    /// <param name="rt">RefreshToken</param>
    /// <returns>Returns the result</returns>
    public async Task<bool> DeleteAsync(Guid? userId, string? rt)
    {
        if (userId == null)
        {
            return false;
        }

        var q = _context.UserRefreshTokens.Where(p => p.UserId == userId);
        if (!string.IsNullOrWhiteSpace(rt))
        {
            q = q.Where(p => p.RefreshToken != rt);
        }

        return await q.ExecuteDeleteAsync() > 0;
    }

    /// <summary>
    /// Delete async
    /// </summary>
    /// <param name="rt">RefreshToken</param>
    /// <returns>Returns the result</returns>
    public async Task<bool> DeleteAsync(string? rt)
    {
        if (string.IsNullOrWhiteSpace(rt))
        {
            return false;
        }

        var q = _context.UserRefreshTokens.Where(p => p.RefreshToken == rt);

        return await q.ExecuteDeleteAsync() > 0;
    }

    #endregion
}
