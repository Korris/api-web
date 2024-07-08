#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Mcsg.Common.Core;

using Core.Constants;
using Core.Dtos;
using SeedWork.Dtos;
using SeedWork.Exceptions;

/// <summary>
/// Security token
/// </summary>
public class SecurityToken
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="setting">JWT setting</param>
    /// <param name="payload">Payload</param>
    public SecurityToken(JwtDto setting, PayloadDto payload)
    {
        if (setting == null)
        {
            setting = new JwtDto();
        }

        Secret = setting.Signing;
        Expires = setting.TimeAt;
        Issuer = setting.Issuer;
        Audience = setting.Audience;

        Payload = payload;
    }

    /// <summary>
    /// Validate
    /// </summary>
    /// <param name="auth">Bearer token</param>
    /// <param name="secret">Secret (if null or length less than 16 will error)</param>
    public static JwtSecurityToken? Validate(string? auth, string secret)
    {
        var token = auth?.Split(" ").LastOrDefault();
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var key = Encoding.UTF8.GetBytes(secret);
        var hmac = new HMACSHA512(key);
        var param = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(hmac.Key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero // tokens expire exactly at token expiration time (instead of 5 minutes later)
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(token, param, out Microsoft.IdentityModel.Tokens.SecurityToken jwt);
            return (JwtSecurityToken)jwt;
        }
        catch
        {
        }

        return null;
    }

    /// <summary>
    /// Get principal from token
    /// </summary>
    /// <param name="token">Token</param>
    /// <param name="secret">Secret (if null or length less than 16 will error)</param>
    /// <returns>Return the result</returns>
    /// <exception cref="ForbiddenAccessException"></exception>
    public static ClaimsPrincipal GetPrincipalFromToken(string token, string secret)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        var param = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero // tokens expire exactly at token expiration time (instead of 5 minutes later)
        };

        var handler = new JwtSecurityTokenHandler();
        var res = handler.ValidateToken(token, param, out Microsoft.IdentityModel.Tokens.SecurityToken jwt);
        if (jwt is not JwtSecurityToken security || !security.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new ForbiddenAccessException(SeedWork.Constants.Error.E200);
        }

        return res;
    }

    /// <summary>
    /// Generate token
    /// </summary>
    /// <param name="length">Length</param>
    /// <returns>Return the result</returns>
    public static string GenerateToken(int length = 64)
    {
        var randomNumber = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Read JWT
    /// </summary>
    /// <param name="auth">Bearer token</param>
    public static JwtSecurityToken? ReadJwt(string auth)
    {
        var token = auth?.Split(" ").Last();
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        var handler = new JwtSecurityTokenHandler();
        return handler.ReadJwtToken(token);
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Issuer
    /// </summary>
    public string Issuer { get; private set; }

    /// <summary>
    /// Audience
    /// </summary>
    public string Audience { get; private set; }

    /// <summary>
    /// Secret
    /// </summary>
    public string Secret { get; private set; }

    /// <summary>
    /// Payload
    /// </summary>
    public PayloadDto Payload { get; private set; }

    /// <summary>
    /// Expires (minute: 1 - 10000)
    /// </summary>
    public double Expires { get; private set; }

    /// <summary>
    /// Expired date
    /// </summary>
    public DateTime ExpiredDate { get; private set; }

    /// <summary>
    /// JSON web token
    /// </summary>
    public string Jwt
    {
        get
        {
            var now = DateTime.UtcNow;
            ExpiredDate = now.AddMinutes(Expires);

            var jti = Guid.NewGuid().ToString();
            var iat = EpochTime.GetIntDate(now).ToString(CultureInfo.InvariantCulture);

            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Jti, jti),
                new (JwtRegisteredClaimNames.Sid, Payload.SessionId.ToString()),
                new (JwtRegisteredClaimNames.Iat, iat, ClaimValueTypes.Integer64),
                new (ClaimTypes.Name, Payload.UserName)
            };

            foreach (var i in Payload.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, i));
            }

            var array = Encoding.UTF8.GetBytes(Secret);
            var key = new SymmetricSecurityKey(array);
            var signing = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var payload = new CustomJwtPayload(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                notBefore: now,
                expires: ExpiredDate);

            var header = new JwtHeader(signing);
            var token = new JwtSecurityToken(header, payload);
            token.Payload.Add(Setting.Payload, Payload);

            var res = new JwtSecurityTokenHandler().WriteToken(token);
            return res;
        }
    }

    #endregion

    #region -- Classes --

    /// <summary>
    /// Initializes a new instance of <see cref="CustomJwtPayload"/> which contains JSON objects representing the claims contained in the JWT. Each claim is a JSON object of the form { Name, Value }.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="CustomJwtPayload"/> class with claims added for each parameter specified. Default string comparer <see cref="StringComparer.Ordinal"/>. 
    /// </remarks>
    /// <param name="issuer">If this value is not null, a { iss, 'issuer' } claim will be added, overwriting any 'iss' claim in 'claims' if present.</param>
    /// <param name="audience">If this value is not null, a { aud, 'audience' } claim will be added, appending to any 'aud' claims in 'claims' if present.</param>
    /// <param name="claims">If this value is not null then for each <see cref="Claim"/> a { 'Claim.Type', 'Claim.Value' } is added. If duplicate claims are found then a { 'Claim.Type', List&lt;object&gt; } will be created to contain the duplicate values.</param>
    /// <param name="notBefore">If notbefore.HasValue a { nbf, 'value' } claim is added, overwriting any 'nbf' claim in 'claims' if present.</param>
    /// <param name="expires">If expires.HasValue a { exp, 'value' } claim is added, overwriting any 'exp' claim in 'claims' if present.</param>
    internal class CustomJwtPayload(string issuer, string audience, IEnumerable<Claim> claims, DateTime? notBefore, DateTime? expires) : JwtPayload(issuer, audience, claims, notBefore, expires)
    {
        /// <summary>
        /// Serializes this instance to JSON
        /// </summary>
        /// <returns>This instance as JSON</returns>
        public override string SerializeToJson()
        {
            var option = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return JsonSerializer.Serialize(this as IDictionary<string, object>, option);
        }
    }

    #endregion
}
