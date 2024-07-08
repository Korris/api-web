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
    /// Time to live of AccessToken [1 - 1440] minutes
    /// </summary>
    public int TimeAt
    {
        get
        {
            return _timeAt;
        }
        set
        {
            if (value < 1)
            {
                value = 1;
            }

            if (value > 1440)
            {
                value = 1440;
            }

            _timeAt = value;
        }
    }

    /// <summary>
    /// Time to live of RefreshToken [2 - 43200] minutes
    /// </summary>
    public int TimeRt
    {
        get
        {
            return _timeRt;
        }
        set
        {
            if (value < 2)
            {
                value = 2;
            }

            if (value > 43200)
            {
                value = 43200;
            }

            _timeRt = value;
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
