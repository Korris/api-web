#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

namespace Mcsg.Common.SeedWork.Responses;

/// <summary>
/// Single response
/// </summary>
public class SingleResponse
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public SingleResponse()
    {
        Succeeded = true;
    }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="message">Error message</param>
    public SingleResponse(string? message)
    {
        Succeeded = false;
        Message = message;
    }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="request">Request</param>
    public SingleResponse(object? request) : this()
    {
        Request = request;
    }

    /// <summary>
    /// Set error
    /// </summary>
    /// <param name="message">Error message</param>
    public void SetError(string? message)
    {
        Succeeded = false;
        Message = message;
    }

    /// <summary>
    /// Set error
    /// </summary>
    /// <param name="errors">Errors</param>
    /// <param name="code">Error code</param>
    /// <param name="message">Error message</param>
    public void SetError(string code, string? message, object? errors)
    {
        Errors = errors;
        SetError(code, message);
    }

    /// <summary>
    /// Set error code
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="message">Error message</param>
    public void SetError(string code, string? message)
    {
        Code = code;
        SetError(message);
    }

    /// <summary>
    /// Set success
    /// </summary>
    /// <param name="data">Data</param>
    public void SetSuccess(object? data)
    {
        Succeeded = true;
        Data = data;
    }

    /// <summary>
    /// Set success
    /// </summary>
    /// <param name="data">Data</param>
    /// <param name="message">Success message</param>
    public void SetSuccess(object? data, string? message)
    {
        Succeeded = true;
        Data = data;
        Message = message;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Succeeded
    /// </summary>
    public bool Succeeded { get; private set; }

    /// <summary>
    /// Message
    /// </summary>
    public string? Message { get; private set; }

    /// <summary>
    /// Code
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Data
    /// </summary>
    public object? Data { get; private set; }

    /// <summary>
    /// Errors
    /// </summary>
    public object? Errors { get; private set; }

    /// <summary>
    /// Request
    /// </summary>
    public object? Request { get; private set; }

    /// <summary>
    /// Return URL
    /// </summary>
    public string? ReturnUrl { get; set; }

    #endregion
}
