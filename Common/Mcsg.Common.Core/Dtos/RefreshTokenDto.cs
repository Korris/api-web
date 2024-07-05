namespace Mcsg.Common.Core.Dtos;

/// <summary>
/// RefreshToken data transfer object
/// </summary>
public class RefreshTokenDto
{
    #region -- Properties --

    /// <summary>
    /// Refresh token
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Refresh token expiry time
    /// </summary>
    public DateTime RefreshTokenExpiryTime { get; set; }

    #endregion
}
