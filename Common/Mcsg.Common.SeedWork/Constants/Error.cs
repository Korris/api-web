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
    public const string E000 = "Request error";

    /// <summary>
    /// Create error
    /// </summary>
    public const string E001 = "Create error";

    /// <summary>
    /// Not found
    /// </summary>
    public const string E002 = "Not found";

    /// <summary>
    /// Data has been deleted
    /// </summary>
    public const string E003 = "Data has been deleted";

    /// <summary>
    /// Update error
    /// </summary>
    public const string E004 = "Update error";

    /// <summary>
    /// Delete error
    /// </summary>
    public const string E005 = "Delete error";

    /// <summary>
    /// Proto problem
    /// </summary>
    public const string E006 = "There's a problem with the proto";

    #endregion

    #region -- E1xx --

    /// <summary>
    /// Please use the 'Forgot Password' function to set a new password.
    /// </summary>
    public const string E100 = "Please use the 'Forgot Password' function to set a new password.";

    /// <summary>
    /// Incorrect password
    /// </summary>
    public const string E101 = "Incorrect password";

    /// <summary>
    /// Verification failed
    /// </summary>
    public const string E102 = "Verification failed";

    /// <summary>
    /// There is no data response
    /// </summary>
    public const string E103 = "There is no data response";

    /// <summary>
    /// Add login error
    /// </summary>
    public const string E104 = "Add login error";

    /// <summary>
    /// The user has no email
    /// </summary>
    public const string E105 = "The user has no email";

    /// <summary>
    /// The user's email has been confirmed
    /// </summary>
    public const string E106 = "The user's email has been confirmed";

    /// <summary>
    /// This data already exists
    /// </summary>
    public const string E107 = "This data already exists";

    /// <summary>
    /// The argument must not be null
    /// </summary>
    public const string E108 = "The argument must not be null";

    /// <summary>
    /// Token is expired
    /// </summary>
    public const string E109 = "Token is expired";

    /// <summary>
    /// This is an administrator. Please log in on the web.
    /// </summary>
    public const string E110 = "This is an administrator. Please log in on the web.";

    /// <summary>
    /// Confirm email error
    /// </summary>
    public const string E111 = "Confirm email error";

    /// <summary>
    /// Invalid file
    /// </summary>
    public const string E112 = "Invalid file";

    /// <summary>
    /// Invalid parent folder
    /// </summary>
    public const string E113 = "Invalid parent folder";

    /// <summary>
    /// User can not follow yourself
    /// </summary>
    public const string E114 = "User can not follow yourself";

    /// <summary>
    /// User spam report
    /// </summary>
    public const string E115 = "User spam report";

    /// <summary>
    /// Referral code is not found
    /// </summary>
    public const string E116 = "Referral code is not found";

    /// <summary>
    /// Referral code only for social register
    /// </summary>
    public const string E117 = "Referral code only for social register";

    /// <summary>
    /// You have entered the referral code.
    /// </summary>
    public const string E118 = "You have entered the referral code.";

    /// <summary>
    /// User not found
    /// </summary>
    public const string E119 = "User not found";

    /// <summary>
    /// User can not follow themselves
    /// </summary>
    public const string E120 = "User can not follow themselves";

    /// <summary>
    /// You are already following this user.
    /// </summary>
    public const string E121 = "You are already following this user.";

    /// <summary>
    /// Avatar image is not null
    /// </summary>
    public const string E122 = "Avatar image is not null";

    /// <summary>
    /// File should be image
    /// </summary>
    public const string E123 = "File should be image";

    /// <summary>
    /// You need to upgrade to a premium account to edit username
    /// </summary>
    public const string E124 = "You need to upgrade to a premium account to edit username";

    /// <summary>
    /// User name matches current username
    /// </summary>
    public const string E125 = "User name matches current username";

    /// <summary>
    /// Cover photo image is not null
    /// </summary>
    public const string E126 = "Cover photo image is not null";

    /// <summary>
    /// Profile name is empty
    /// </summary>
    public const string E127 = "Profile name is empty";

    /// <summary>
    /// Wait time for edit username:
    /// </summary>
    public const string E128 = "Wait time for edit username: ";

    /// <summary>
    /// Has been blocked
    /// </summary>
    public const string E129 = "Has been blocked";

    /// <summary>
    /// Unable to report yourself
    /// </summary>
    public const string E130 = "Unable to report yourself";

    /// <summary>
    /// Report not found
    /// </summary>
    public const string E131 = "Report not found";

    /// <summary>
    /// Post not belong to you
    /// </summary>
    public const string E132 = "Post not belong to you";

    #endregion

    #region -- E2xx --

    /// <summary>
    /// No file uploaded
    /// </summary>
    public const string E201 = "No file uploaded";

    /// <summary>
    /// Only image files are allowed
    /// </summary>
    public const string E202 = "Only image files are allowed";

    /// <summary>
    /// Post does not exist
    /// </summary>
    public const string E204 = "Post does not exist";

    /// <summary>
    /// This post has deleted
    /// </summary>
    public const string E205 = "This post has deleted";

    /// <summary>
    /// This thumbnail not found
    /// </summary>
    public const string E206 = "This thumbnail not found";

    /// <summary>
    /// This cover not found
    /// </summary>
    public const string E207 = "This cover not found";

    /// <summary>
    /// Subpost does not exist
    /// </summary>
    public const string E208 = "Subpost does not exist";

    /// <summary>
    /// This subpost has deleted
    /// </summary>
    public const string E209 = "This subpost has deleted";

    /// <summary>
    /// Invalid upload data
    /// </summary>
    public const string E210 = "Invalid upload data";

    #endregion

    #region -- E3xx --

    /// <summary>
    /// Invalid access token
    /// </summary>
    public const string E300 = "Invalid access token";

    /// <summary>
    /// Token or OTP is incorrect
    /// </summary>
    public const string E301 = "Token or OTP is incorrect";

    /// <summary>
    /// Invalid refresh token
    /// </summary>
    public const string E302 = "Invalid refresh token";

    /// <summary>
    /// Account does not exist
    /// </summary>
    public const string E303 = "Account does not exist";

    /// <summary>
    /// Incorrect password
    /// </summary>
    public const string E304 = "Incorrect password";

    /// <summary>
    /// Account has been deleted
    /// </summary>
    public const string E305 = "Account has been deleted";

    /// <summary>
    /// Account has been logged into the social network
    /// </summary>
    public const string E306 = "Account has been logged into the social network";

    /// <summary>
    /// Email not confirmed
    /// </summary>
    public const string E307 = "Email not confirmed";

    /// <summary>
    /// Mobile not confirmed
    /// </summary>
    public const string E308 = "Mobile not confirmed";

    /// <summary>
    /// This user not permission to do this action
    /// </summary>
    public const string E309 = "This user not permission to do this action";

    /// <summary>
    /// This user has been suspeded
    /// </summary>
    public const string E310 = "This user has been suspended";

    /// <summary>
    /// This user has been banned
    /// </summary>
    public const string E311 = "This user has been banned";

    /// <summary>
    /// New password should different current
    /// </summary>
    public const string E312 = "New password should different current";

    /// <summary>
    /// Incorrect recovery code
    /// </summary>
    public const string E313 = "Incorrect recovery code";

    /// <summary>
    /// The recovery code has already been used
    /// </summary>
    public const string E314 = "The recovery code has already been used";

    /// <summary>
    /// OTP expired
    /// </summary>
    public const string E315 = "OTP expired";

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

    #region -- E6xx --

    /// <summary>
    /// Chapter not exits
    /// </summary>
    public const string E600 = "Chapter does not exist!";

    /// <summary>
    /// Need premium to read
    /// </summary>
    public const string E601 = "You need to upgrade to a premium account to view this content";

    #endregion

    #region -- E7xx --

    /// <summary>
    /// Can't donate
    /// </summary>
    public const string E700 = "User can not send donate to yourself";

    /// <summary>
    /// Can't transfer
    /// </summary>
    public const string E701 = "User can not send transfer to yourself";

    /// <summary>
    /// Can't swap
    /// </summary>
    public const string E702 = "User can not swap to other address";

    /// <summary>
    /// Not enough balance
    /// </summary>
    public const string E703 = "Not enough balance";

    /// <summary>
    /// Transaction not found
    /// </summary>
    public const string E704 = "Transaction not found";

    /// <summary>
    /// Minimum transaction amount
    /// </summary>
    public const string E705 = "The minimum transaction amount must be greater than or equal to";

    /// <summary>
    /// Transaction status
    /// </summary>
    public const string E706 = "Transaction status is incorrect";

    /// <summary>
    /// Invalid from date
    /// </summary>
    public const string E707 = "From date must be greater than utc now";

    /// <summary>
    /// Invalid fee
    /// </summary>
    public const string E708 = "The percentage-based fee must not exceed 100%";

    /// <summary>
    /// Cannot delete
    /// </summary>
    public const string E709 = "Cannot delete please create new data";

    /// <summary>
    /// Transaction already processed
    /// </summary>
    public const string E710 = "Transaction already processed";

    /// <summary>
    /// UserWallet not found
    /// </summary>
    public const string E711 = "UserWallet not found";

    /// <summary>
    /// ConversionRate not found
    /// </summary>
    public const string E712 = "ConversionRate not found";

    /// <summary>
    /// Coin not found
    /// </summary>
    public const string E713 = "Coin not found";

    #endregion

    #region -- E9xx --

    /// <summary>
    /// TODO
    /// </summary>
    public const string E900 = "E900";

    #endregion
}
