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
/// Regular expressions
/// </summary>
public class Regular
{
    /// <summary>
    /// RegexMessage message
    /// </summary>
    public const string RegexMessage = "Profile name can only contain letters, numbers, dots, underscores, and hyphens";

    /// <summary>
    /// <summary>
    /// Vietnamese phone numbers typically have the following formats:<br/>
    /// Mobile numbers: start with 09, 03, 07, 08, or 05 followed by 8 digits<br/>
    /// Landline numbers: start with 02 followed by 9 digits<br/>
    /// ^: Start of the string<br/>
    /// (0[23578]|09|03): The number should start with 02, 03, 05, 07, 08, or 09<br/>
    /// \d{8}$: Followed by exactly 8 digits, and $ denotes the end of the string
    /// </summary>
    public const string PhoneNumber = "^(0[23578]|09|03)\\d{8}$";

    /// <summary>
    /// Tag
    /// </summary>
    public const string Tag = @"^[a-zA-Z0-9_]+$";

    /// <summary>
    /// Regular expression for validating profile names.<br/>
    /// Allows characters A-Z, a-z, numbers, dots, underscores, and hyphens.<br/>
    /// </summary>
    public const string ProfileName = @"^[\p{L}A-Za-z0-9._\- ]+$";
}
