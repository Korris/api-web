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
    /// Equal message
    /// </summary>
    public const string Equal = "does not match";

    /// <summary>
    /// User name free
    /// </summary>
    public class UserNameFree
    {
        /// <summary>
        /// Minimum length
        /// </summary>
        public const ushort Min = 15;

        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 30;
    }

    /// <summary>
    /// User name Premium
    /// </summary>
    public class UserNamePremium
    {
        /// <summary>
        /// Minimum length
        /// </summary>
        public const ushort Min = 5;

        /// <summary>
        /// Maximum length
        /// </summary>
        public const ushort Max = 15;
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
}
