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
/// Information
/// </summary>
public static class Information
{
    #region -- I0xx --

    /// <summary>
    /// Account login from a third party
    /// </summary>
    public const string I000 = "This is an account login from a third party.";

    /// <summary>
    /// Verification code instructions
    /// </summary>
    public const string I001 = "Concatenate the 4-digit code from the email with the 2-digit code here, then send it back to the server for verification.";

    /// <summary>
    /// Not email template
    /// </summary>
    public const string I002 = "Not email template found for";

    /// <summary>
    /// Cannot DeserializeObject
    /// </summary>
    public const string I003 = "Cannot DeserializeObject message";

    /// <summary>
    /// Data is null
    /// </summary>
    public const string I004 = "Data is null";

    /// <summary>
    /// Id is null or zero
    /// </summary>
    public const string I005 = "Id is null or zero";

    #endregion

    #region -- I1xx --

    /// <summary>
    /// Begin sending messages
    /// </summary>
    public const string I100 = "Begin sending messages to the queue.";

    /// <summary>
    /// Message sent successfully
    /// </summary>
    public const string I101 = "Message sent successfully.";

    /// <summary>
    /// Begin sending message
    /// </summary>
    public const string I102 = "Begin sending message.";

    /// <summary>
    /// Message sending complete
    /// </summary>
    public const string I103 = "Message sending complete.";

    #endregion
}
