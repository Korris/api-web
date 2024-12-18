namespace Mcsg.Common.Core.Dtos;

/// <summary>
/// ImageRatio data transfer object
/// </summary>
public class ImageRatio
{
    #region -- Properties --

    /// <summary>
    /// Width
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Height
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Stream
    /// </summary>
    public Stream? Stream { get; set; }

    /// <summary>
    /// Object name
    /// </summary>
    public string? ObjectName { get; set; }

    #endregion
}
