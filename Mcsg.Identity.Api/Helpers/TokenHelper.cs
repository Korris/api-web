using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Mcsg.Identity.Api.Helpers;

using Common.SeedWork.Dtos;
using Lib.Common.Constants;
using Lib.Common.Exceptions;
using Response;

public static class TokenHelper
{
    public static string GenerateToken(int length = 64)
    {
        var randomNumber = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public static ClaimsPrincipal GetPrincipalFromToken(string token, string signingKey)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new ForbiddenAccessException(ErrorCodes.InvalidAccessToken);

        return principal;
    }

    public static TokenResponse GenerateAccessToken(Guid sessionId, JwtDto jwt)
    {
        Dictionary<string, object> claims = new()
        {
            { SecurityClaimTypes.SessionIdClaimName, sessionId.ToString() }
        };

        DateTime utcNow = DateTime.UtcNow;
        DateTime expiresAt = utcNow.AddMinutes(jwt.ExpiredTokenTimeInMinute);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            IssuedAt = utcNow,
            Claims = claims,
            Expires = expiresAt,
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Signing)), SecurityAlgorithms.HmacSha256),
            NotBefore = utcNow,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken jwtToken = tokenHandler.CreateToken(tokenDescriptor);
        string tokenString = tokenHandler.WriteToken(jwtToken);
        return new TokenResponse() { AccessToken = tokenString, ExpiredDate = expiresAt };
    }
}
