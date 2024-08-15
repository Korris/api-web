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
/// Message
/// </summary>
public static class Message
{
    #region -- M0xx --

    /// <summary>
    /// Request error
    /// </summary>
    public const string M000 = "Request error";

    /// <summary>
    /// Create error
    /// </summary>
    public const string M001 = "Create error";

    /// <summary>
    /// Not found
    /// </summary>
    public const string M002 = "Not found";

    /// <summary>
    /// Data deleted
    /// </summary>
    public const string M003 = "Data has been deleted";

    /// <summary>
    /// Update error
    /// </summary>
    public const string M004 = "Update error";

    /// <summary>
    /// Delete error
    /// </summary>
    public const string M005 = "Delete error";

    #endregion

    #region -- M1xx --

    /// <summary>
    /// Password not set
    /// </summary>
    public const string M100 = "Please use the 'Forgot Password' function to set a new password.";

    /// <summary>
    /// Incorrect password
    /// </summary>
    public const string M101 = "Incorrect password";

    /// <summary>
    /// Verification failed
    /// </summary>
    public const string M102 = "Verification failed";

    /// <summary>
    /// No data response
    /// </summary>
    public const string M103 = "There is no data response";

    /// <summary>
    /// Add login error
    /// </summary>
    public const string M104 = "Add login error";

    /// <summary>
    /// User has no email
    /// </summary>
    public const string M105 = "The user has no email";

    /// <summary>
    /// Email confirmed
    /// </summary>
    public const string M106 = "The user's email has been confirmed";

    /// <summary>
    /// Data already exists
    /// </summary>
    public const string M107 = "This data already exists";

    /// <summary>
    /// Argument not null
    /// </summary>
    public const string M108 = "The argument must not be null";

    /// <summary>
    /// Token is expired
    /// </summary>
    public const string M109 = "Token is expired";

    /// <summary>
    /// Log in on the web
    /// </summary>
    public const string M110 = "This is an administrator. Please log in on the web.";

    /// <summary>
    /// Confirm email error
    /// </summary>
    public const string M111 = "Confirm email error";

    /// <summary>
    /// Invalid file
    /// </summary>
    public const string M112 = "Invalid file";

    /// <summary>
    /// Invalid parent folder
    /// </summary>
    public const string M113 = "Invalid parent folder";

    /// <summary>
    /// User can not follow yourself
    /// </summary>
    public const string M114 = "User can not follow yourself";

    /// <summary>
    /// User spam report
    /// </summary>
    public const string M115 = "User spam report";

    /// <summary>
    /// User enter referral code not existed
    /// </summary>
    public const string M116 = "Referral code is not found";

    /// <summary>
    /// Referral code only for social register
    /// </summary>
    public const string M117 = "Referral code only for social register";

    /// <summary>
    /// User already is Referee
    /// </summary>
    public const string M118 = "You have entered the referral code.";

    /// <summary>
    /// User not found
    /// </summary>
    public const string M119 = "User not found";

    /// <summary>
    /// User can not follow themselves
    /// </summary>
    public const string M120 = "User can not follow themselves";

    /// <summary>
    /// You are already following this user.
    /// </summary>
    public const string M121 = "You are already following this user.";

    /// <summary>
    /// Avatar image is not null
    /// </summary>
    public const string M122 = "Avatar image is not null";

    /// <summary>
    /// Avatar should be image
    /// </summary>
    public const string M123 = "File should be image";

    /// <summary>
    ///  Need Premium Account To Edit
    /// </summary>
    public const string M124 = "You need to upgrade to a premium account to edit username";

    /// <summary>
    ///  Duplicate username
    /// </summary>
    public const string M125 = "Duplicate username";

    /// <summary>
    ///  Cover photo image is not null
    /// </summary>
    public const string M126 = "Cover photo image is not null";

    /// <summary>
    ///  Profile name is empty
    /// </summary>
    public const string M127 = "Profile name is empty";

    /// <summary>
    ///  Buy rename package to edit username
    /// </summary>
    public const string M128 = "Buy rename package to edit username";

    /// <summary>
    ///  Wait time for edit username
    /// </summary>
    public const string M129 = "Wait time for edit username : ";

    #endregion

    #region -- M2xx --

    /// <summary>
    /// No file uploaded
    /// </summary>
    public const string M201 = "No file uploaded";

    /// <summary>
    /// Only image files are allowed
    /// </summary>
    public const string M202 = "Only image files are allowed";

    /// <summary>
    /// Post does not exist
    /// </summary>
    public const string M204 = "Post does not exist";

    /// <summary>
    /// This post has deleted
    /// </summary>
    public const string M205 = "This post has deleted";

    #endregion

    #region -- M3xx --

    /// <summary>
    /// Invalid access token
    /// </summary>
    public const string M300 = "Invalid access token";

    /// <summary>
    /// Token or OTP is incorrect
    /// </summary>
    public const string M301 = "Token or OTP is incorrect";

    /// <summary>
    /// Invalid refresh token
    /// </summary>
    public const string M302 = "Invalid refresh token";

    /// <summary>
    /// Account does not exist
    /// </summary>
    public const string M303 = "Account does not exist";

    /// <summary>
    /// Incorrect password
    /// </summary>
    public const string M304 = "Incorrect password";

    /// <summary>
    /// Account has been deleted
    /// </summary>
    public const string M305 = "Account has been deleted";

    /// <summary>
    /// Account has been logged into the social network
    /// </summary>
    public const string M306 = "Account has been logged into the social network";

    #endregion
}
