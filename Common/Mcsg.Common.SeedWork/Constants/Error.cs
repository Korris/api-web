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
    /// Request error
    /// </summary>
    public const string E000 = "E000";

    /// <summary>
    /// Create error
    /// </summary>
    public const string E001 = "E001";

    /// <summary>
    /// Not found
    /// </summary>
    public const string E002 = "E002";

    /// <summary>
    /// Data deleted
    /// </summary>
    public const string E003 = "E003";

    /// <summary>
    /// Update error
    /// </summary>
    public const string E004 = "E004";

    /// <summary>
    /// Delete error
    /// </summary>
    public const string E005 = "E005";

    #endregion

    #region -- E1xx --

    /// <summary>
    /// Password not set
    /// </summary>
    public const string E100 = "E100";

    /// <summary>
    /// Incorrect password
    /// </summary>
    public const string E101 = "E101";

    /// <summary>
    /// Verification failed
    /// </summary>
    public const string E102 = "E102";

    /// <summary>
    /// No data response
    /// </summary>
    public const string E103 = "E103";

    /// <summary>
    /// Add login error
    /// </summary>
    public const string E104 = "E104";

    /// <summary>
    /// Email confirmed
    /// </summary>
    public const string E106 = "E106";

    /// <summary>
    /// This data already exists
    /// </summary>
    public const string E107 = "E107";

    /// <summary>
    /// Token is expired
    /// </summary>
    public const string E109 = "E109";

    /// <summary>
    /// Confirm email error
    /// </summary>
    public const string E111 = "E111";

    /// <summary>
    /// Invalid file
    /// </summary>
    public const string E112 = "E112";

    /// <summary>
    /// Invalid parent folder
    /// </summary>
    public const string E113 = "E113";

    /// <summary>
    /// User can not follow yourself
    /// </summary>
    public const string E114 = "E114";

    /// <summary>
    /// User spam report
    /// </summary>
    public const string E115 = "E115";

    /// <summary>
    /// Referral code is not existed
    /// </summary>
    public const string E116 = "E116";

    /// <summary>
    /// Referral only for social register account
    /// </summary>
    public const string E117 = "E117";

    /// <summary>
    /// User already is Referee
    /// </summary>
    public const string E118 = "E118";

    /// <summary>
    /// User not found
    /// </summary>
    public const string E119 = "E119";

    /// <summary>
    /// User can not follow themselves
    /// </summary>
    public const string E120 = "E120";

    /// <summary>
    /// You are already following this user.
    /// </summary>
    public const string E121 = "E121";

    /// <summary>
    /// Avatar image is not null
    /// </summary>
    public const string E122 = "E122";

    /// <summary>
    ///    File should be image
    /// </summary>
    public const string E123 = "E123";

    /// <summary>
    ///  Need Premium Account To Edit
    /// </summary>
    public const string E124 = "E124";

    /// <summary>
    ///  User name matches current username
    /// </summary>
    public const string E125 = "E125";

    /// <summary>
    ///  Cover photo image is not null
    /// </summary>
    public const string E126 = "E126";

    /// <summary>
    ///  Profile name is empty
    /// </summary>
    public const string E127 = "E127";

    /// <summary>
    ///  Wait time for edit username
    /// </summary>
    public const string E128 = "E128";

    /// <summary>
    ///  Must not be the same as the old username
    /// </summary>
    public const string E129 = "E129";

    #endregion

    #region -- E2xx --

    /// <summary>
    /// Not file upload
    /// </summary>
    public const string E201 = "E201";

    /// <summary>
    /// Only image file
    /// </summary>
    public const string E202 = "E202";

    /// <summary>
    /// Post does not exist
    /// </summary>
    public const string E204 = "E204";

    /// <summary>
    /// This post has deleted
    /// </summary>
    public const string E205 = "E205";

    /// <summary>
    /// Thumbnail not found
    /// </summary>
    public const string E206 = "E206";

    /// <summary>
    /// Cover not found
    /// </summary>
    public const string E207 = "E207";

    #endregion

    #region -- E3xx --

    /// <summary>
    /// Invalid access token
    /// </summary>
    public const string E300 = "E300";

    /// <summary>
    /// Token or OTP is incorrect
    /// </summary>
    public const string E301 = "E301";

    /// <summary>
    /// Invalid refresh token
    /// </summary>
    public const string E302 = "E302";

    /// <summary>
    /// Account does not exist
    /// </summary>
    public const string E303 = "E303";

    /// <summary>
    /// Incorrect password
    /// </summary>
    public const string E304 = "E304";

    /// <summary>
    /// Account has been deleted
    /// </summary>
    public const string E305 = "E305";

    /// <summary>
    /// Account has been logged into the social network
    /// </summary>
    public const string E306 = "E306";

    /// <summary>
    /// Email not confirmed
    /// </summary>
    public const string E307 = "E307";

    /// <summary>
    /// Mobile not confirmed
    /// </summary>
    public const string E308 = "E308";

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

    #region -- E9xx --

    /// <summary>
    /// TODO
    /// </summary>
    public const string E900 = "E900";

    #endregion
}
