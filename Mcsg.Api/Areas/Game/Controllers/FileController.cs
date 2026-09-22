using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.Game.Controllers;

using Mcsg.Api.Areas.Game.Attributes;
using Mcsg.Api.Areas.Game.Constants;
using Mcsg.Api.Areas.Game.Interfaces;
using Mcsg.Api.Areas.Game.Requests;

/// <summary>
/// Game uploads (api/game/file). Copy of Comic FileController limited to thumbnail + game .html.
/// </summary>
[ApiController]
[Route("api/game/[controller]"), Authorize]
public class FileController : ControllerBase
{
    #region -- Methods --

    public FileController(IGameFileService fileService)
    {
        _fileService = fileService;
    }

    /// <summary>
    /// Upload thumbnail image (same route name as the other areas) → hashId to send as ThumbnailHashId
    /// </summary>
    [DisableRequestSizeLimit]
    [ServiceFilter(typeof(MediaOnlyAttribute))]
    [HttpPost("upload-media")]
    public async Task<IActionResult> UploadThumbnail([FromForm] FileCreateR request)
    {
        request.Analyze(HttpContext);
        var result = await _fileService.UploadThumbnailAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Upload the game .html file → hashId to send as GameHashId.
    /// Request body is capped at 51 MB by Kestrel (50 MB ceiling + multipart overhead); the service then enforces SystemSettings "GameFileSize" (default 30 MB).
    /// Frontend must embed it with &lt;iframe sandbox="allow-scripts"&gt; (no allow-same-origin).
    /// </summary>
    [RequestSizeLimit(GameConfig.GameFileRequestLimitBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = GameConfig.GameFileRequestLimitBytes)]
    [HttpPost("upload-game")]
    public async Task<IActionResult> UploadGame([FromForm] FileCreateR request)
    {
        request.Analyze(HttpContext);
        var result = await _fileService.UploadGameAsync(request);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly IGameFileService _fileService;

    #endregion
}
