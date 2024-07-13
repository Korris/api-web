namespace Mcsg.Common.Core.Dtos;

/// <summary>
/// ApiError data transfer object
/// </summary>
public class ApiErrorDto
{
    #region -- Properties --

    /// <summary>
    /// Code
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Message
    /// </summary>
    public string? Message { get; set; }

    #endregion
}
