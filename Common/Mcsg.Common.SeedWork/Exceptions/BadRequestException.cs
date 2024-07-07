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
/// BadRequest exception
/// </summary>
public class BadRequestException : BaseException
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="message">Message</param>
    public BadRequestException(string message) : base(Error.E400, message)
    {
    }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="code">Code</param>
    /// <param name="message">Message</param>
    public BadRequestException(string code, string message) : base(code, message)
    {
    }

    #endregion
}