using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mcsg.Media.Api.Controllers;

using Common.Core.Interfaces;
using Common.SeedWork.Responses;
using Interfaces;

/// <summary>
/// Video controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class VideoController : ControllerBase
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="setting">Setting</param>
    public VideoController(ISetting setting, IStorageClient sc)
    {
        _setting = setting;
        _sc = sc;
    }

    /// <summary>
    /// Get
    /// </summary>
    /// <param name="v">Video</param>
    /// <param name="a">Audio</param>
    /// <returns>Return the result</returns>
    [HttpGet]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> Get([FromQuery] string? v, string? a)
    {
        if (!string.IsNullOrWhiteSpace(a))
        {
            v = a;
        }

        if (string.IsNullOrWhiteSpace(v))
        {
            return NoContent();
        }

        var objectName = v;
        if (string.IsNullOrWhiteSpace(objectName))
        {
            return NoContent();
        }

        var ms = await _sc.GetStrategy().GetObject(objectName, null) as MemoryStream;
        if (ms == null)
        {
            return NoContent();
        }

        var type = "";
        var extension = Path.GetExtension(objectName);
        if (extension.Equals(".mp3", StringComparison.CurrentCultureIgnoreCase))
        {
            type = "audio/mp3";
        }
        else if (extension.Equals(".mp4", StringComparison.CurrentCultureIgnoreCase))
        {
            type = "video/mp4";
        }
        else
        {
            type = "video/webm";
        }

        Response.Headers["Content-Length"] = ms.Length.ToString();
        Response.Headers["Accept-Ranges"] = "bytes";
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