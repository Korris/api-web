namespace Mcsg.Common.Core.Dtos;

/// <summary>
/// ApiData data transfer object
/// </summary>
public class ApiDataDto
{
    #region -- Properties --

    /// <summary>
    /// Status
    /// </summary>
    public string Status { get; set; } = default!;

    /// <summary>
    /// Data
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Error
    /// </summary>
    public ApiErrorDto Error { get; set; } = default!;

    /// <summary>
    /// Path
    /// </summary>
    public string Path { get; set; } = default!;

    #endregion
}
