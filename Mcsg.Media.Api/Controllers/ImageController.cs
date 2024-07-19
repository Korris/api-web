using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mime;

namespace Mcsg.Media.Api.Controllers;

using Common.Core.Constants;
using Common.Core.Interfaces;
using Common.SeedWork.Responses;
using Interfaces;

/// <summary>
/// Image controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="setting">Setting</param>
    public ImageController(ISetting setting, IStorageClient sc)
    {
        _setting = setting;
        _sc = sc;
    }

    /// <summary>
    /// Get
    /// </summary>
    /// <param name="i">Image</param>
    /// <param name="p">Public</param>
    /// <returns>Return the result</returns>
    [HttpGet]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get([FromQuery] string? i, string? p)
    {
        MemoryStream? ms = null;
        string? objectName = null;

        if (!string.IsNullOrWhiteSpace(p))
        {
            objectName = $"{Setting.MinioFolder.Image}/{p}";
            ms = await _sc.Strategy.GetObject(objectName, null) as MemoryStream;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(i))
            {
                return NoContent();
            }

            objectName = i;
            if (string.IsNullOrWhiteSpace(objectName))
            {
                return NoContent();
            }

            ms = await _sc.Strategy.GetObject(objectName, null) as MemoryStream;
        }

        if (ms == null)
        {
            return NoContent();
        }

        var type = MediaTypeNames.Image.Jpeg;
        var extension = Path.GetExtension(objectName);
        if (extension.Equals(".gif", StringComparison.CurrentCultureIgnoreCase))
        {
            type = MediaTypeNames.Image.Gif;
        }

        Response.Headers["Cache-Control"] = "public, max-age=43800";

        return File(ms.ToArray(), type);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// Storage client
    /// </summary>
    private readonly IStorageClient _sc;

    #endregion
}