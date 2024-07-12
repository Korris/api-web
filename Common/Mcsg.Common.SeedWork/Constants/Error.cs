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

namespace Mcsg.Common.SeedWork.Constants;

/// <summary>
/// Error code
/// </summary>
public static class Error
{
    #region -- E0xx --

    /// <summary>
    /// Not found
    /// </summary>
    public const string E002 = "E002";

    #endregion

    #region -- E1xx --

    /// <summary>
    /// Password not set
    /// </summary>
    public const string E100 = "E100";

    /// <summary>
    /// Token is expired
    /// </summary>
    public const string E109 = "E109";

    /// <summary>
    /// Invalid file
    /// </summary>
    public const string E112 = "E112";

    /// <summary>
    /// Invalid parent folder
    /// </summary>
    public const string E113 = "E113";

    #endregion

    #region -- E2xx --

    /// <summary>
    /// Invalid access token
    /// </summary>
    public const string E200 = "ERR_AUTH_00003";

    /// <summary>
    /// Not file upload
    /// </summary>
    public const string E201 = "ERR_API_000002";

    /// <summary>
    /// Only image file
    /// </summary>
    public const string E202 = "ERR_API_000006";

    /// <summary>
    /// Not existed user
    /// </summary>
    public const string E203 = "ERR_AUTH_00011";

    /// <summary>
    /// Post does not exist
    /// </summary>
    public const string E204 = "ERR_API_200001";

    /// <summary>
    /// This post has deleted
    /// </summary>
    public const string E205 = "ERR_API_200003";

    #endregion

    #region -- E4xx --

    /// <summary>
    /// Bad request code
    /// </summary>
    public const string E400 = "400";

    /// <summary>
    /// Unauthorize access code
    /// </summary>
    public const string E401 = "401";

    /// <summary>
    /// Forbidden access code
    /// </summary>
    public const string E403 = "403";

    /// <summary>
    /// Not found code
    /// </summary>
    public const string E404 = "404";

    #endregion

    #region -- E5xx --

    /// <summary>
    /// API error code
    /// </summary>
    public const string E500 = "500";

    #endregion
}
