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

namespace Mcsg.Common.SeedWork.Exceptions;

using Constants;

/// <summary>
/// UnauthorizedAccess exception
/// </summary>
public class UnauthorizedAccessException : BaseException
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="message">Message</param>
    public UnauthorizedAccessException(string message) : base(Error.E401, message)
    {
    }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="code">Code</param>
    /// <param name="message">Message</param>
    public UnauthorizedAccessException(string code, string message) : base(code, message)
    {
    }

    #endregion
}