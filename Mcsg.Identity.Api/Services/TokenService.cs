using Dapper;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Mcsg.Identity.Api.Services
{
    using DTOs;
    using DTOs.Response;
    using Helpers;
    using Interfaces;
    using Lib.Common.Constants;
    using Lib.Common.Exceptions;
    using Lib.Common.Extensions;
    using Lib.Common.Models;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Repositories;
    using Lib.Data.Repositories.Interface;

    public partial class TokenService : ITokenService
    {
        private readonly JwtSetting _jwtConfiguration;
        private readonly IRepository<UserRefreshToken> _userRefreshTokenRepository;
        public TokenService(
            IUnitOfWork unitOfWork,
            IOptions<JwtSetting> jwtConfiguration)
        {
            _jwtConfiguration = jwtConfiguration.Value;
            _userRefreshTokenRepository = unitOfWork.GetRepository<UserRefreshToken>();
        }

        public async Task<Guid> IsValidRefreshTokenAsync(string refreshToken)
        {
            var userRefreshToken = await _userRefreshTokenRepository
                        .Connection.QueryFirstOrDefaultAsync<UserRefreshToken>(GetByRefreshTokenQuery, new { RefreshToken = refreshToken });

            if (userRefreshToken == null)
            {
                return Guid.Empty;
            }

            if (userRefreshToken.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                await _userRefreshTokenRepository.DeleteAsync(userRefreshToken.Id);
                return Guid.Empty;
            }

            return userRefreshToken.UserId;
        }

        public Guid GetSessionIdFromToken(string accessToken)
        {
            ClaimsPrincipal principal = TokenHelper.GetPrincipalFromToken(accessToken, _jwtConfiguration.Key) ?? throw new ForbiddenAccessException(ErrorCodes.InvalidAccessToken);
            return principal.FindFirstValue(SecurityClaimTypes.SessionIdClaimName).ToGuid();
        }

        public async Task<RefreshTokenDto> AddUserRefreshTokenAsync(User user)
        {
            if (user != null)
            {
                var userRefreshTokens = await _userRefreshTokenRepository
                        .Connection.QueryAsync<UserRefreshToken>(GetRefreshTokenByUserIdQuery, new { UserId = user.Id });

                if (userRefreshTokens != null && userRefreshTokens.Any())
                {
                    var userRefreshToken = userRefreshTokens.FirstOrDefault();
                    userRefreshToken.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfiguration.RefreshTokenExpiredTimeInDay);
                    await _userRefreshTokenRepository.UpdateAsync(userRefreshToken);
                    return new RefreshTokenDto()
                    {
                        RefreshToken = userRefreshToken.RefreshToken,
                        RefreshTokenExpiryTime = userRefreshToken.RefreshTokenExpiryTime.Value
                    };
                }
                else
                {
                    var userRefreshToken = new UserRefreshToken()
                    {
                        UserId = user.Id,
                        RefreshToken = TokenHelper.GenerateToken(),
                        RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtConfiguration.RefreshTokenExpiredTimeInDay)
                    };
                    await _userRefreshTokenRepository.InsertAsync(userRefreshToken);
                    return new RefreshTokenDto()
                    {
                        RefreshToken = userRefreshToken.RefreshToken,
                        RefreshTokenExpiryTime = userRefreshToken.RefreshTokenExpiryTime.Value
                    };
                }
            }
            return null;
        }

        public TokenResponse GenerateAccessToken(Guid sessionId)
        {
            return TokenHelper.GenerateAccessToken(sessionId, _jwtConfiguration);
        }

        public async Task<bool> DeleteRefreshTokenAsync(Guid userId)
        {
            var iResult = await _userRefreshTokenRepository.Connection
                .ExecuteAsync(DeleteRefreshTokenByUserIdCommand, new
                {
                    UserId = userId
                });
            return iResult > 0;
        }
    }
}
