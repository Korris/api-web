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

namespace Mcsg.Common.Core.Dtos;

/// <summary>
/// NotificationInfo data transfer object
/// </summary>
/// <remarks>
/// Initialize
/// </remarks>
/// <param name="to">To</param>
public class NotificationInfoDto(string? to)
{
    #region -- Properties --

    /// <summary>
    /// To
    /// </summary>
    public string? To { get; set; } = to;

    /// <summary>
    /// Carbon copy
    /// </summary>
    public string? Cc { get; set; }

    /// <summary>
    /// Blind carbon copy
    /// </summary>
    public string? Bcc { get; set; }

    /// <summary>
    /// Subject
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Body
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// Display name associated with address
    /// </summary>
    public string Display { get; set; } = string.Empty;

    /// <summary>
    /// A collection of key-value pairs that will be added to the message as data fields. Keys and the values must not be null
    /// </summary>
    public Dictionary<string, string>? Data { get; set; }

    #endregion
}
