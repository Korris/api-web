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

/// <summary>
/// Base exception
/// </summary>
public class BaseException : Exception
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="code">Code</param>
    /// <param name="message">Message</param>
    public BaseException(string code, string message) : base(message)
    {
        Code = code;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Code
    /// </summary>
    public string Code { get; set; }

    #endregion
}