namespace Mcsg.Common.Core.Dtos;

/// <summary>
/// Token data transfer object
/// </summary>
public class TokenDto
{
    #region -- Properties --

    /// <summary>
    /// Access token
    /// </summary>
    public string AccessToken { get; set; } = default!;

    /// <summary>
    /// Refresh token
    /// </summary>
    public string RefreshToken { get; set; } = default!;

    /// <summary>
    /// Expired date
    /// </summary>
    public DateTime ExpiredDate { get; set; }

    /// <summary>
    /// Refresh token expired date
    /// </summary>
    public DateTime RefreshTokenExpiredDate { get; set; }

    /// <summary>
    /// Subscription key
    /// </summary>
    public string SubscriptionKey { get; set; } = default!;

    /// <summary>
    /// Roles
    /// </summary>
    public string Roles { get; set; } = default!;

    /// <summary>
    /// Check user is register by social account
    /// </summary>
    public bool IsFirstTimeLoginBySocial { get; set; }

    #endregion
}
