#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 07:03
 * Update       : 2024-Jan-21 07:03
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

namespace Mcsg.Common.SeedWork.Dtos;

/// <summary>
/// JWT data transfer object
/// </summary>
public class JwtDto
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public JwtDto()
    {
        Signing = string.Empty;
        Issuer = string.Empty;
        Audience = string.Empty;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// JWT signing
    /// </summary>
    public string Signing { get; set; }

    /// <summary>
    /// JWT issuer
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// JWT audience
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// Time to live of AccessToken in minutes [1 - 86400] (max 60 days)
    /// </summary>
    public int TimeAt
    {
        get
        {
            return _timeAt;
        }
        set
        {
            // Ensure the value is within the allowed range for AccessToken
            _timeAt = Math.Max(1, Math.Min(value, 86400));
        }
    }

    /// <summary>
    /// Time to live of RefreshToken in minutes [2 - 129600] (max 90 days)
    /// </summary>
    public int TimeRt
    {
        get
        {
            return _timeRt;
        }
        set
        {
            // Ensure the value is within the allowed range for RefreshToken
            _timeRt = Math.Max(2, Math.Min(value, 129600));
        }
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Time to live of AccessToken
    /// </summary>
    private int _timeAt;

    /// <summary>
    /// Time to live of RefreshToken
    /// </summary>
    private int _timeRt;

    #endregion
}
