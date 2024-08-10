using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mcsg.Identity.Api.Services;

using Common.Core;
using Common.Core.Dtos;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Interfaces;
using Lib.Common.Extensions;

public class TokenService : ITokenService
{
    public TokenService(IMcsgContext context, ISetting setting)
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
            await _context.SaveChangesAsync(default);

            return Guid.Empty;
        }

        return res.UserId;
    }

    public Guid GetSessionIdFromToken(string accessToken)
    {
        ClaimsPrincipal principal = SecurityToken.GetPrincipalFromToken(accessToken, _setting.Jwt.Signing) ?? throw new ForbiddenAccessException(Common.SeedWork.Constants.Error.E300);
        return principal.FindFirstValue(JwtRegisteredClaimNames.Sid).ToGuid();
    }

    public async Task<RefreshTokenDto?> AddUserRefreshTokenAsync(User user)
    {
        if (user == null)
        {
            return null;
        }

        var userRefreshToken = await _context.UserRefreshTokens.OrderByDescending(p => p.RefreshTokenExpiryTime).FirstOrDefaultAsync(p => p.UserId == user.Id);

        if (userRefreshToken != null)
        {
            userRefreshToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_setting.Jwt.TimeRt);
            await _context.SaveChangesAsync(default);

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
            await _context.SaveChangesAsync(default);

            return new RefreshTokenDto
            {
                RefreshToken = userRefreshToken.RefreshToken,
                RefreshTokenExpiryTime = userRefreshToken.RefreshTokenExpiryTime.Value
            };
        }
    }

    public TokenDto GenerateAccessToken(Guid sessionId, User user)
    {
        var payload = new PayloadDto
        {
            Id = user.Id,
            UserName = user.UserName + "",
            ProfileName = user.ProfileName + "",
            ProfileId = user.ProfileId + "",
            UserFolder = user.UserFolder,
            UserAvatar = user.Avatar + "",
            IsPremium = user.IsPremium,
            IsWalletShowing = user.IsWalletShowing,
            SessionId = sessionId
        };
        var st = new SecurityToken(_setting.Jwt, payload);

        return new TokenDto { AccessToken = st.Jwt, ExpiredDate = st.ExpiredDate };
    }

    public async Task<bool> DeleteRefreshTokenAsync(Guid userId)
    {
        var userRefreshTokens = await _context.UserRefreshTokens.Where(p => p.UserId == userId).ToListAsync();

        _context.UserRefreshTokens.RemoveRange(userRefreshTokens);
        var count = await _context.SaveChangesAsync(default);

        return count > 0;
    }

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
