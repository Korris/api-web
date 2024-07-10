using Microsoft.AspNetCore.Http;

namespace Mcsg.Common.Core.Dtos;

/// <summary>
/// CompressImage data transfer object
/// </summary>
public class CompressImage : ImageRatio
{
    #region -- Properties --

    /// <summary>
    /// Image
    /// </summary>
    public IFormFile Image { get; set; } = default!;

    #endregion
}
