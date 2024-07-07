using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Mcsg.Identity.Api.Services;

using Common.Core;
using Common.Core.Dtos;
using Common.SeedWork.Constants;
using Common.SeedWork.Exceptions;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Extensions;
using Lib.Data;
using Lib.Data.Domain.Entities;

public class TokenService : ITokenService
{
    public TokenService(McsgDbContext context, ISetting setting)
    {
        _context = context;
        _setting = setting;
    }

    public async Task<Guid> IsValidRefreshTokenAsync(string refreshToken)
    {
        var res = await _context.UserRefreshTokens.FirstOrDefaultAsync(p => p.RefreshToken == refreshToken);

        if (res == null)
        {
            return Guid.Empty;
        }

        if (res.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            _context.UserRefreshTokens.Remove(res);
            await _context.SaveChangesAsync();

            return Guid.Empty;
        }

        return res.UserId;
    }

    public Guid GetSessionIdFromToken(string accessToken)
    {
        ClaimsPrincipal principal = SecurityToken.GetPrincipalFromToken(accessToken, _setting.Jwt.Signing) ?? throw new ForbiddenAccessException(Error.E200);
        return principal.FindFirstValue(SecurityClaimTypes.SessionIdClaimName).ToGuid();
    }

    public async Task<RefreshTokenDto?> AddUserRefreshTokenAsync(User user)
    {
        if (user != null)
        {
            var userRefreshToken = await _context.UserRefreshTokens.OrderByDescending(p => p.RefreshTokenExpiryTime).FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (userRefreshToken != null)
            {
                userRefreshToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_setting.Jwt.TimeRt);
                await _context.SaveChangesAsync();

                return new RefreshTokenDto
                {
                    RefreshToken = userRefreshToken.RefreshToken,
                    RefreshTokenExpiryTime = userRefreshToken.RefreshTokenExpiryTime.Value
                };
            }
            else
            {
                userRefreshToken = new UserRefreshToken
                {
                    UserId = user.Id,
                    RefreshToken = SecurityToken.GenerateToken(),
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_setting.Jwt.TimeRt)
                };
                await _context.UserRefreshTokens.AddAsync(userRefreshToken);

                return new RefreshTokenDto
                {
                    RefreshToken = userRefreshToken.RefreshToken,
                    RefreshTokenExpiryTime = userRefreshToken.RefreshTokenExpiryTime.Value
                };
            }
        }

        return null;
    }

    public TokenDto GenerateAccessToken(Guid sessionId)
    {
        return SecurityToken.GenerateAccessToken(sessionId, _setting.Jwt);
    }

    public async Task<bool> DeleteRefreshTokenAsync(Guid userId)
    {
        var userRefreshTokens = await _context.UserRefreshTokens.Where(p => p.UserId == userId).ToListAsync();

        _context.UserRefreshTokens.RemoveRange(userRefreshTokens);
        var count = await _context.SaveChangesAsync();

        return count > 0;
    }

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly McsgDbContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
