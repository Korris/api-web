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
/// Validator
/// </summary>
public class Validator
{
    /// <summary>
    /// NotEmpty message
    /// </summary>
    public const string NotEmpty = "must be specifed";

    /// <summary>
    /// MinimumLength message
    /// </summary>
    public const string MinimumLength = "does not exceed the authorized size";

    /// <summary>
    /// MaximumLength message
    /// </summary>
    public const string MaximumLength = "exceeds the authorized size";

    /// <summary>
    /// MinimumValue message
    /// </summary>
    public const string MinimumValue = "values should be within the range of";

    /// <summary>
    /// MaximumValue message
    /// </summary>
    public const string MaximumValue = "to";

    /// <summary>
    /// EmailAddress message
    /// </summary>
    public const string EmailAddress = "is not a valid email address";

    /// <summary>
    /// PhoneNumber message
    /// </summary>
    public const string PhoneNumber = "is not a valid phone number";

    /// <summary>
    /// Tag
    /// </summary>
    public const string Tag = "Invalid hashtag format. Hashtags must start with #, contain no spaces or special characters and be 1-33 characters long.";

    /// <summary>
    /// Equal message
    /// </summary>
    public const string Equal = "does not match";

    /// <summary>
    /// User name
    /// </summary>
    public class UserName
    {
        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 30;
    }

    /// <summary>
    /// User name free
    /// </summary>
    public class UserNameFree : UserName
    {
        /// <summary>
        /// Minimum length
        /// </summary>
        public const ushort Min = 15;

        /// <summary>
        /// Regular expression for validating.<br/>
        /// Allows letters (a-z, A-Z) and numbers (0-9).<br/>
        /// No spaces allowed.<br/>
        /// Minimum length: 15 characters.<br/>
        /// Maximum length: 30 characters.<br/>
        /// Must contain at least one letter and one number.<br/>
        /// </summary>
        public const string Regex = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]+$";

        /// <summary>
        /// Error message for invalid.<br/>
        /// </summary>
        public const string Message = "User name can only contain letters and numbers. No spaces allowed. Must be between 15 and 30 characters long and contain at least one letter and one number.";
    }

    /// <summary>
    /// User name premium
    /// </summary>
    public class UserNamePremium : UserName
    {
        /// <summary>
        /// Minimum length
        /// </summary>
        public const ushort Min = 5;

        /// <summary>
        /// Regular expression for validating.<br/>
        /// Allows letters (a-z, A-Z) and numbers (0-9).<br/>
        /// No spaces allowed.<br/>
        /// Minimum length: 5 characters.<br/>
        /// Maximum length: 30 characters.<br/>
        /// </summary>
        public const string Regex = @"^[A-Za-z0-9]+$";

        /// <summary>
        /// Error message for invalid.<br/>
        /// </summary>
        public const string Message = "User name can only contain letters and numbers. No spaces allowed. Must be between 5 and 30 characters";
    }

    /// <summary>
    /// Profile name
    /// </summary>
    public class ProfileName
    {
        /// <summary>
        /// Minimum length
        /// </summary>
        public const ushort Min = 6;

        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 50;

        /// <summary>
        /// Regular expression for validating profile names.<br/>
        /// Allows characters A-Z, a-z, numbers, and special characters.<br/>
        /// Special characters are not allowed at the beginning or consecutively.<br/>
        /// Maximum length: 50 characters.<br/>
        /// </summary>
        public const string Regex = @"^(?!.*[^\p{L}\p{N} ]{2})[\p{L}\p{N}](?:[\p{L}\p{N}]|[^\p{L}\p{N} ](?![^\p{L}\p{N} ])| ){0,49}$";

        /// <summary>
        /// Regex message for validating.
        /// </summary>
        public const string Message = "Profile name can only contain letters, numbers, dots, underscores, and hyphens";
    }

    /// <summary>
    /// Hashtag
    /// </summary>
    public class Hashtag
    {
        /// <summary>
        /// Minimum length
        /// </summary>
        public const ushort Min = 1;

        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 34;

        /// <summary>
        /// Regex
        /// </summary>
        public const string Regex = @"#(\w+)";
    }

    /// <summary>
    /// Mention
    /// </summary>
    public class Mention
    {
        /// <summary>
        /// Regex
        /// </summary>
        public const string Regex = @"(?<=@)([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})";
    }

    /// <summary>
    /// Link
    /// </summary>
    public class Link
    {
        /// <summary>
        /// Regex
        /// </summary>
        public const string Regex = @"(http|https):\/\/[^\s/$.?#].[^\s]*";
    }

    /// <summary>
    /// Email
    /// </summary>
    public class Email
    {
        /// <summary>
        /// Minimum length
        /// </summary>
        public const ushort Min = 4;

        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 256;
    }

    /// <summary>
    /// Name
    /// </summary>
    public class Name
    {
        /// <summary>
        /// Minimum length
        /// </summary>
        public const ushort Min = 3;

        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 512;
    }

    /// <summary>
    /// Description
    /// </summary>
    public class Description
    {
        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 1024;
    }

    /// <summary>
    /// Content
    /// </summary>
    public class Content
    {
        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 65535;
    }

    /// <summary>
    /// Caption
    /// </summary>
    public class Caption
    {
        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 512;
    }

    /// <summary>
    /// Address
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 256;
    }

    /// <summary>
    /// Species
    /// </summary>
    public class Species
    {
        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 128;
    }

    /// <summary>
    /// Currency
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 8;
    }

    /// <summary>
    /// Mode
    /// </summary>
    public class Mode
    {
        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 8;
    }

    /// <summary>
    /// UnitPrice
    /// </summary>
    public class UnitPrice
    {
        /// <summary>
        /// Minimum value
        /// </summary>
        public const double Min = 0;

        /// <summary>
        /// Maximum length
        /// </summary>
        public const double Max = 999999999;
    }

    /// <summary>
    /// Password
    /// </summary>
    public class Password
    {
        /// <summary>
        /// Minimum length
        /// </summary>
        public const ushort Min = 8;

        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 128;
    }

    /// <summary>
    /// Purpose
    /// </summary>
    public class Purpose
    {
        /// <summary>
        /// VerifyEmail
        /// </summary>
        public const string VerifyEmail = "VerifyEmail";

        /// <summary>
        /// ForgotPass
        /// </summary>
        public const string ForgotPass = "ForgotPass";

        /// <summary>
        /// SetPass
        /// </summary>
        public const string SetPass = "SetPass";
    }

    /// <summary>
    /// Location
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 200;

        /// <summary>
        /// Regular expression for validating location.<br/>
        /// Allows characters A-Z, a-z, numbers, commas, dots, hyphens, slashes, spaces, and hashes, including Vietnamese characters.<br/>
        /// Maximum length: 200 characters.<br/>
        /// </summary>
        public const string Regex = @"^[\p{L}\p{N},.\-/# ]*$";

        /// <summary>
        /// Regex message for validating 
        /// </summary>
        public const string Message = "Location can contain letters, numbers, commas, dots, hyphens, slashes, spaces, and hashes, including Vietnamese characters.";
    }

    /// <summary>
    /// Title
    /// </summary>
    public class Title
    {
        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 255;
    }

    /// <summary>
    /// Summary
    /// </summary>
    public class Summary
    {
        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 2000;
    }
}
